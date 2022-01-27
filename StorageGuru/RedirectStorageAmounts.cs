using Planetbase;
using Redirection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace StorageGuru
{
    public abstract class StorageGuruResourceAmounts : ResourceAmounts
    {
        [RedirectFrom(typeof(ResourceAmounts))]
        public new void serialize(XmlNode parent, string name)
        {
            XmlNode parent2 = Serialization.createNode(parent, name, null);
            Serialization.serializeString(parent2, "container-name", this.mName);
            foreach (ResourceAmount resourceAmount in this.mResourceAmounts)
            {
                resourceAmount.serialize(parent2, "amount");
            }

            // Save storageguru manifest

            StorageGuruMod.StorageController.Serialize(parent2, "storage-guru-manifest");
        }

        [RedirectFrom(typeof(ResourceAmounts))]
        public new void deserialize(XmlNode node)
        {
            this.mName = Serialization.deserializeString(node["container-name"]);
            foreach (object obj in node.ChildNodes)
            {
                XmlNode xmlNode = (XmlNode)obj;
                if (xmlNode.Name == "amount")
                {
                    ResourceAmount resourceAmount = new ResourceAmount();
                    resourceAmount.deserialize(xmlNode);
                    this.mResourceAmounts.Add(resourceAmount);
                }
                else if (xmlNode.Name == "storage-guru-manifest")
                {
                    var manifest = new StorageManifestController();
                    manifest.Deserialize(xmlNode);
                    StorageGuruMod.StorageController = manifest;
                }
            }
        }
    }
}
