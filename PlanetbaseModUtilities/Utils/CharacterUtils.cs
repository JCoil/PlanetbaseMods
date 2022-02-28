using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlanetbaseModUtilities
{
    /// <summary>
    /// Helper methods and members for Characters, Colonist and Bots
    /// </summary>
    public static class CharacterUtils
    {
        public static List<Character> GetAllCharacters()
        {
            return CoreUtils.GetMember<Character, List<Character>>("mCharacters");
        }

        public static void SetTarget(this Character character, Target target)
        {
            CoreUtils.InvokeMethod("setTarget", character, target);
        }
    }
}
