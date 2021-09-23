using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorageGuru
{
    public class ResourceTypeComparer : IEqualityComparer<ResourceType>
    {
        public static ResourceTypeComparer Comparer { get; private set; } = new ResourceTypeComparer();

        public bool Equals(ResourceType x, ResourceType y)
        {
            return x.getName() == y.getName();
        }

        public int GetHashCode(ResourceType obj)
        {
            return obj.GetHashCode();
        }
    }
}
