using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ColonistReport
{
    public class ColonistsWorkload
    {
        public Specialization Specialization { get; private set; }

        public int TotalCharacters { get; private set; }

        public float Workload { get; private set; }

        public string Name => Specialization.getName();
        public Texture2D Icon => Specialization.getIcon();

        float timeIdle;
        float timeNeeds; // Time required to fulfil needs - sleeping, eating etc.
        float timeMoving; // Time required for moving between activities - varies per base
        float timeAvailable; // Time available for working

        private float currentSmoothing = 1f; // We will do the first calculation without any smoothing
        const float DefaultSmoothing = 256f;

        public ColonistsWorkload(Specialization s)
        {
            Specialization = s;
            SetTimeConfig();
        }

        public void Calculate()
        {
            if (Character.getSpecializationCharacters(Specialization) is List<Character> specializationCharacters
                && specializationCharacters.Count > 0)
            {
                var total = specializationCharacters.Count;
                var idle = 0f;

                foreach(var character in specializationCharacters)
                {
                    idle += character.getState() == Character.State.Idle ? 1f : 0f;
                }

                TotalCharacters = total;
                timeIdle += (idle / total - timeIdle) / currentSmoothing;

                Workload = CalculateWorkload();

                currentSmoothing = DefaultSmoothing; // Set smoothing to default after first pass
            }
        }

        /// <summary>
        /// Workload is the percent of Available Time that is Working Time
        /// Working Time is Available Time minus Idle Time
        /// Available Time is Total Time minus Needs Time minus Moving Time
        /// </summary>
        private float CalculateWorkload()
        {
            var workload = (timeAvailable - timeIdle) / timeAvailable;
            return workload > 0 ? (workload < 1 ? workload : 1) : 0;
        }

        private void SetTimeConfig()
        {
            timeNeeds = 0.2f;
            timeMoving = 0.3f;
            timeAvailable = 1f - (timeNeeds + timeMoving);
        }
    }
}
