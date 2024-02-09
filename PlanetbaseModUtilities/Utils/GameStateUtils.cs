using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlanetbaseModUtilities
{
    public static class GameStateUtils
    {
        #region Getters

        public static int GetCurrentModuleSize(this GameStateGame game)
        {
            return CoreUtils.GetMember<GameStateGame, int>("mCurrentModuleSize", game);
        }

        public static ModuleType GetPlacedModuleType(this GameStateGame game)
        {
            return CoreUtils.GetMember<GameStateGame, ModuleType>("mPlacedModuleType", game);
        }

        #endregion

        #region Setters

        public static void SetCurrentModuleSize(this GameStateGame game, int size)
        {
            CoreUtils.SetMember("mCurrentModuleSize", game, size);
        }

        #endregion

        #region GameStateGame.Mode

        /// <summary>
        /// Get the current mMode from a GameStateGame.
        /// Unless the actually mode value is needed, recommend using IsMode for comparisons
        /// </summary>
        public static Mode GetMode(this GameStateGame game)
        {
            // The Type of mMode (Planetbase.GameStateGame+Mode) is not accessible, 
            // so we have to do some wrangling to get the Type out and then evaluate the mMode field
            var memberName = "mMode";
            var enumName = $"{game.GetType().FullName}+Mode"; // Planetbase.GameStateGame+Mode

            // Get mMode from game. This has underlying type Planetbase.GameStateGame+Enum which we can't use because it's not part of our assembly, so we keep it boxed as object
            var mMode = CoreUtils.GetEnumValue<GameStateGame, object>(memberName, game, enumName, typeof(GameStateGame).Assembly);

            return (Mode)mMode;
        }

        public static bool IsMode(this GameStateGame game, Mode mode)
        {
            return game.GetMode() == mode;
        }

        /// <summary>
        /// Set the value of mMode on a GameStateGame.
        /// Be careful using this - it is NOT the same as the actual GameStateGame.Mode, even though the mappings match. 
        /// </summary>
        public static void SetMode(this GameStateGame game, Mode mode)
        {
            var enumName = $"{game.GetType().FullName}+Mode";
            var enumType = CoreUtils.GetEnum(enumName, typeof(GameStateGame).Assembly);

            CoreUtils.SetEnumValue("mMode", game, Enum.Parse(enumType, mode.ToString()));
        }

        /// <summary>
        /// Mirror for private enum GameStateGame.Mode.
        /// Be careful using this - it is NOT the same as the actual GameStateGame.Mode, even though the mappings match.
        /// Recommend only using this for the helper methods GetMode and IsMode.
        /// </summary>
        public enum Mode
        {
            Idle = 0,
            PlacingModule = 1,
            EditingModule = 2,
            PlacingComponent = 3,
            CloseCamera = 4
        }

        #endregion
    }
}
