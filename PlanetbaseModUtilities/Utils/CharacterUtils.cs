using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlanetbaseModUtilities.Utils
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

        public static void SetTarget<T>(this T character, Target target) where T:Character
        {
            CoreUtils.InvokeMethod<Character>("setTarget", character, target);
        }
    }
}
