using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;

namespace PowerMonitor
{
    public class PowerMonitor : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new PowerMonitor(), modEntry, "PowerMonitor");

        public override void OnInitialized(ModEntry modEntry)
        {
            RegisterStrings();
        }

        private static void RegisterStrings()
        {
            StringUtils.RegisterString("tooltip_power_monitor", "Power Overview");
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
