using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

namespace ColonistReports
{
    public class SpecializationWorkload
    {
        public Specialization Specialization { get; private set; }
        public int CharacterCount { get; private set; }

        public readonly string Name;
        public readonly string StatName;
        public Texture2D Icon => Specialization.getIcon();

        private float AverageWorkload { get; set; }

        private int counter;
        public bool IsSaturated => counter > 100; // Kinda arbitrary, but makes sure we build up a decent average before polling

        public SpecializationWorkload(Specialization s)
        {
            counter = 0;
            Specialization = s;
            Name = Specialization.getName();
            StatName = Name + "Workload";
        }

        public void Update()
        {
            // Calculate cumulative average
            if (Character.getSpecializationCharacters(Specialization) is List<Character> specializationCharacters && specializationCharacters.Count > 0)
            {
                CharacterCount = specializationCharacters.Count;
                float countNotIdle = specializationCharacters.Sum(x => x.getState() == Character.State.Idle ? 0 : 1);

                var currentWorkload = countNotIdle / CharacterCount;
                AverageWorkload = (currentWorkload + AverageWorkload * counter) / (counter + 1);
            }

            counter++;
        }

        public float PollAverageWorkload()
        {
            counter = 0;
            return AverageWorkload;
        }
    }
}
