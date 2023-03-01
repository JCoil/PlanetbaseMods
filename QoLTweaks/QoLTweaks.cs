using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;

namespace QoLTweaks
{
    public class QoLTweaks : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new QoLTweaks(), modEntry, "QoLTweaks");

        public override void OnInitialized(ModEntry modEntry)
        {
            // Change base behaviour of game elements
            MedicalCabinetTweaks.UpdateMedicalCabinetCapacity();
            MedicalCabinetTweaks.AllowMedicalCabinetsInLabs();
        }

        public override void OnGameStart(GameStateGame gameStateGame)
        {
            // Update existing elements to match new base behavious
            MedicalCabinetTweaks.UpdateExistingMedicalCabinetCapacity();
            MedicalCabinetTweaks.UpdateExistingAllowMedicalCabinetsInLabs();
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing required here for now
        }
    }
}
