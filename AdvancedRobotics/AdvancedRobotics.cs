using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;

namespace AdvancedRobotics
{
    public class AdvancedRobotics : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new AdvancedRobotics(), modEntry, "AdvancedRobotics");

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        private static void RegisterStrings()
        {
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing required here for now
        }
    }
}

