using System;
using System.Collections.Generic;
using System.Text;
using Planetbase;

namespace PlanetbaseModUtilities
{
    public static class GameStateUtils
    {
        public static int GetModeValue(this GameStateGame game)
        {
            return CoreUtils.GetMember<GameStateGame, int>("mMode", game);
        }

        public static int GetCurrentModuleSize(this GameStateGame game)
        {
            return CoreUtils.GetMember<GameStateGame, int>("mCurrentModuleSize", game);
        }

        public static void SetCurrentModuleSize(this GameStateGame game, int size)
        {
            CoreUtils.SetMember("mCurrentModuleSize", game, size);
        }

        public static ModuleType GetPlacedModuleType(this GameStateGame game)
        {
            return CoreUtils.GetMember<GameStateGame, ModuleType>("mPlacedModuleType", game);
        }
    }
}
