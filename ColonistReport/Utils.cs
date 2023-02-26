using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ColonistReports
{
    public static class Utils
    {
        public static Texture2D GetIcon<T>() where T : Specialization => TypeList<Specialization, SpecializationList>.find<Worker>().getIcon();

        public static float Clamp(float value, float min, float max)
        {
            return value > min ? (value < max ? value : max) : min;
        }
    }

    public static class CharacterActivityExtensions
    {
        public static bool IsSleeping(this Character character)
        {
            if (character.getInteractionComponent() is ConstructionComponent component)
            {
                if (component.getComponentType() == TypeList<ComponentType, ComponentTypeList>.find<Bunk>())
                {
                    return true;
                }
                else if (component.getComponentType() == TypeList<ComponentType, ComponentTypeList>.find<Bed>())
                {
                    return true;
                }
            }
            if (character.getState() == Character.State.Ko)
            {
                return true;
            }

            return false;
        }

        public static bool IsRepairing(this Character character)
        {
            if (character is Bot && character.getInteractionComponent() is ConstructionComponent comp)
            {
                if (comp.getComponentType() == TypeList<ComponentType, ComponentTypeList>.find<BotAutoRepair>())
                {
                    return true;
                }
            }

            return false;
        }

        public static T[] Shift<T>(this T[] array, int count = 1)
        {
            var newArray = new T[array.Length];
            Array.Copy(array, count, newArray, 0, array.Length - count);
            return newArray;
        }
    }
}
