using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

namespace ColonistReports
{
    public class ColonistsWorkload
    {
        public Specialization Specialization { get; private set; }

        public int TotalCharacters { get; private set; }

        public float DisplayWorkload { get; private set; }
        private float Workload { get; set; }

        public string Name => Specialization.getName();
        public Texture2D Icon => Specialization.getIcon();

        private float counter = 1f;

        const float SmoothingFactor = 0.1f;

        public ColonistsWorkload(Specialization s)
        {
            Specialization = s;
        }

        public void Update()
        {
            Workload += CalculateInstantaneousWorkload();

            counter++;
        }

        public void Refresh()
        {
            DisplayWorkload *= 1 - SmoothingFactor;
            DisplayWorkload += SmoothingFactor * Utils.Clamp(Workload / counter, 0, 1);

            Workload = 0f;
            counter = 1f;
        }

        /// <summary>
        /// Calculates the current fraction of available colonists(that aren't sleeping, eating etc.) that are not Idle
        /// </summary>
        public float CalculateInstantaneousWorkload()
        {
            if (Character.getSpecializationCharacters(Specialization) is List<Character> specializationCharacters
                && specializationCharacters.Count > 0)
            {
                TotalCharacters = specializationCharacters.Count;
                float countIdle = specializationCharacters.Sum(x => x.getState() == Character.State.Idle ? 1 : 0);

                return 1 - (countIdle / TotalCharacters);
            }

            return 0;
        }
    }
}
