using System.Linq;
using System.Xml.Linq;

namespace Cake.BitDiffer
{
    public class BitDifferResult
    {
        public XDocument RawResult { get; set; }

        public string GetChangeMessage()
        {
            if (RawResult == null)
            {
                return "No result file generated";
            }

            var errorNode = RawResult
                .Element("AssemblyComparison")
                .Element("Groups")
                .Descendants("Group")
                .FirstOrDefault(w => w.Attribute("Change")?.Value != "None" || w.Attribute("HasErrors") != null);

            return errorNode?.Attribute("ErrorDetail")?.Value ??
                   errorNode?.Attribute("Change")?.Value;
        }

        public bool HasChanges()
        {
            if (RawResult == null)
            {
                return true;
            }

            return RawResult
                .Element("AssemblyComparison")
                .Element("Groups")
                .Descendants("Group")
                .Any(w => w.Attribute("Change")?.Value != "None" || w.Attribute("HasErrors") != null);
        }
    }
}