using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ColonistReport
{
    public static class Utils
    {
        public static Texture2D GetIcon<T>() where T : Specialization => TypeList<Specialization, SpecializationList>.find<Worker>().getIcon();
    }
}
