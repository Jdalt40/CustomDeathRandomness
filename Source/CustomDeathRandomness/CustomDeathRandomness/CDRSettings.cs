using Verse;

namespace CustomDeathRandomness
{
    public class CDRSettings : ModSettings
    {
        public static CDRSettings Instance;
        public float animalDeathChance = 0.47f;
        public float pawnDeathChance = 0.67f;

        public CDRSettings()
        {
            CDRSettings.Instance = this;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<float>(ref this.animalDeathChance, "animalDeathChance", 0.47f, false);
            Scribe_Values.Look<float>(ref this.pawnDeathChance, "pawnDeathChance", 0.67f, false);
        }
    }
}
