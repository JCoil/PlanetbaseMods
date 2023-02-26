using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
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

        const float timeNeeds = 0.08f; // Time spent on basic needs

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
            DisplayWorkload = Utils.Clamp(Workload / counter, 0, 1);

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

                float timeTotal = TotalCharacters;
                float countIdle = 0f;
                float countEssentials = 0f;

                foreach (var character in specializationCharacters)
                {
                    if (character.getState() == Character.State.Idle)
                    {
                        countIdle++;
                    }
                    else if (PlanetbaseModUtilities.CoreUtils.InvokeMethod<Character, bool>("isBeingRestored", character))
                    {
                        countEssentials++;
                    }
                }

                var timeAvailable = 1 - timeNeeds - (countEssentials / timeTotal); // Time available to work

                var timeWorking = timeAvailable - (countIdle / timeTotal); // Fraction of available time spent working

                return timeWorking / timeAvailable;
            }

            return 0;
        }
    }
}
