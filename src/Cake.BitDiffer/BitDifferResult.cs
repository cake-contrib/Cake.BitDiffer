using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Cake.BitDiffer
{
    /// <summary>
    ///     Result object for the comparison
    /// </summary>
    public class BitDifferResult
    {
        /// <summary>
        ///     Constructor for the result
        /// </summary>
        /// <param name="xmlResult">Raw XML result of comparison</param>
        /// <param name="output">Standard output content</param>
        public BitDifferResult(IEnumerable<string> output, XDocument xmlResult = null)
        {
            RawResult = xmlResult;
            ExecutionResult = output?.ToArray();
        }

        /// <summary>
        ///     Origin raw result as XML
        /// </summary>
        public XDocument RawResult { get; }

        /// <summary>
        ///     Standard output content of the BitDiffer execution
        /// </summary>
        public string[] ExecutionResult { get; }

        /// <summary>
        ///     Get the change message, if any (change type or error message)
        /// </summary>
        /// <returns></returns>
        public string GetChangeMessage()
        {
            var executionResult = GetExecutionErrorMessage();
            var xmlResult = GetXmlChangeMessage();

            return string.IsNullOrWhiteSpace(xmlResult)
                ? executionResult
                : xmlResult;
        }

        private string GetXmlChangeMessage()
        {
            if (RawResult == null)
            {
                return null;
            }

            var errorNode = RawResult
                ?.Element("AssemblyComparison")
                ?.Element("Groups")
                ?.Descendants("Group")
                .FirstOrDefault(w => w.Attribute("Change")?.Value != "None" || w.Attribute("HasErrors") != null);

            return errorNode?.Attribute("ErrorDetail")?.Value ??
                   errorNode?.Attribute("Change")?.Value;
        }

        private string GetExecutionErrorMessage()
        {
            if (ExecutionResult == null)
            {
                return null;
            }

            return string.Join(Environment.NewLine,
                ExecutionResult.Where(w => w.StartsWith("error", StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        ///     Check, if the result contains detected changes
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            return HasXmlChanges() || HasExecutionErrors();
        }

        private bool HasExecutionErrors()
        {
            if (ExecutionResult == null)
            {
                return false;
            }

            return ExecutionResult.Any(s => s.StartsWith("error", StringComparison.InvariantCultureIgnoreCase));
        }

        private bool HasXmlChanges()
        {
            if (RawResult == null)
            {
                return false;
            }

            return RawResult
                       ?.Element("AssemblyComparison")
                       ?.Element("Groups")
                       ?.Descendants("Group")
                       .Any(w => w.Attribute("Change")?.Value != "None" || w.Attribute("HasErrors") != null) ?? false;
        }
    }
}