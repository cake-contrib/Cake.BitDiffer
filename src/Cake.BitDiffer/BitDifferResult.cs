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
        ///     Origin raw result as XML
        /// </summary>
        public XDocument RawResult { get; set; }

        /// <summary>
        ///     Get the change message, if any (change type or error message)
        /// </summary>
        /// <returns></returns>
        public string GetChangeMessage()
        {
            if (RawResult == null)
            {
                return "No result file generated";
            }

            XElement errorNode = RawResult
                ?.Element("AssemblyComparison")
                ?.Element("Groups")
                ?.Descendants("Group")
                .FirstOrDefault(w => w.Attribute("Change")?.Value != "None" || w.Attribute("HasErrors") != null);

            return errorNode?.Attribute("ErrorDetail")?.Value ??
                   errorNode?.Attribute("Change")?.Value;
        }

        /// <summary>
        ///     Check, if the result contains detected changes
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            if (RawResult == null)
            {
                return true;
            }

            return RawResult
                       ?.Element("AssemblyComparison")
                       ?.Element("Groups")
                       ?.Descendants("Group")
                       .Any(w => w.Attribute("Change")?.Value != "None" || w.Attribute("HasErrors") != null) ?? false;
        }
    }
}