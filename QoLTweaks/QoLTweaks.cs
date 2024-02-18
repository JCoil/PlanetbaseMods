using Planetbase;
using PlanetbaseModUtilities;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace QoLTweaks
{
    public class QoLTweaksSettings : ModSettings, IDrawable
    {
        [Draw("Improved number formatting")] public bool ImprovedNumberFormatting = true; // Vanilla = false, Default = true 

        [Draw("Allow Benches in Bio-Domes")] public bool BenchesInBioDomes = true; // Vanilla = false, Default = true 
        [Draw("Allow Drinking Fountains in Bio-Domes")] public bool DrinkingFountainsInBioDomes = true; // Vanilla = false, Default = true 
        [Draw("Allow Plants in Bio-Domes")] public bool DecorativePlantsInBioDomes = true; // Vanilla = false, Default = true 

        [Draw("Decrease Bunk wall separation")] public bool DecreaseBunkWallSeparation = true; // Vanilla = false, Default = true 

        [Draw("Allow Medical Cabinets in Labs")] public bool MedicalCabinetsInLabs = true; // Vanilla = false, Default = true
        [Draw("Medical Cabinet capacity")] public int MedicalCabinetCapacity = 12; // Vanilla = 4, Default = 12

        public override void Save(ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        void IDrawable.OnChange()
        {
        }
    }

    public class QoLTweaks : ModBase
    {
        public static QoLTweaksSettings Settings;

        public static new void Init(ModEntry modEntry) => InitializeMod(new QoLTweaks(), modEntry, "QoLTweaks");

        public override void OnInitialized(ModEntry modEntry)
        {
            Settings = ModSettings.Load<QoLTweaksSettings>(modEntry);

            modEntry.OnGUI = (ModEntry entry) => Settings.Draw(modEntry);
            modEntry.OnSaveGUI = (ModEntry entry) => Settings.Save(modEntry);

            RegisterStrings();

            // One-off changes to base behaviour of game elements
            MedicalCabinetTweaks.UpdateMedicalCabinetCapacity();
            MedicalCabinetTweaks.AllowMedicalCabinetsInLabs();

            BioDomeTweaks.AllowComponentsInBioDomes();

            DormTweaks.DecreaseBunkWallSeparation();
        }

        private static void RegisterStrings()
        {
            //StringUtils.RegisterString("fill_with_bunks", "Fill With Bunks");
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
