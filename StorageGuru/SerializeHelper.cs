using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StorageGuru
{
    public class SerializeHelper
    {
        private const string moduleSeperator = ":";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private const string resourceWrapperL = "{";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private const string resourceWrapperR = "}";
        private const char resourceDelim = ',';
        private const string targetNamespace = "Planetbase.";
        private const string targetAssembly = ", Assembly-CSharp";

        public string SerializeManifest(TextWriter writer, Dictionary<Module, List<Type>> manifest)
        {
            //this is mostly to remove the "remove unused parameter if its not part of a shipped public API" message
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            StringBuilder sb = new StringBuilder();

            foreach(var entry in manifest)
            {
                sb.Append(entry.Key.getId());
                sb.Append(moduleSeperator + "[");

                int i = 0;
                foreach(var res in entry.Value)
                {
                    sb.Append(res.ToString().Replace(targetNamespace, ""));
                    i++;
                    sb.Append(i == entry.Value.Count ? "" : resourceDelim.ToString());
                }

                sb.Append("]");
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public Dictionary<int, List<string>> DeserializeManifest(string[] contents)
        {
            var manifest = new Dictionary<int, List<string>>();

            foreach (var line in contents)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    int id = Int32.Parse(line.Remove(line.IndexOf(moduleSeperator)));
                    var strResources = line.Substring(line.IndexOf(moduleSeperator) + 1).Replace("[", "")
                        .Replace("]", "").Split(resourceDelim).ToList();
                    strResources = strResources.Where(x => !String.IsNullOrEmpty(x)).ToList();
                    strResources = strResources.Select(x => targetNamespace + x.Trim() + targetAssembly).ToList();
                    manifest.Add(id, strResources);
                }
            }

            return manifest;
        }
    }
}

