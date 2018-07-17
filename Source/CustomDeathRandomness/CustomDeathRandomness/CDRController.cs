using UnityEngine;
using Verse;

namespace CustomDeathRandomness
{
    public class CDRController : Mod
    {
        public static CDRController Instance;

        public static float CDRA;
        public static float CDRP;

        public override string SettingsCategory()
        {
            return "Custom Death Randomness";
        }

        public CDRController(ModContentPack content) : base(content)
        {
            CDRController.Instance = this;
            CDRSettings.Instance = base.GetSettings<CDRSettings>();
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = canvas.width;
            listing_Standard.Begin(canvas);
            listing_Standard.Gap(12f);
            listing_Standard.Label("CDR_AnimalDeathRate".Translate(new object[]
            {
                CDRSettings.Instance.animalDeathChance
            }), -1f);
            GUI.contentColor = Color.yellow;
            CDRSettings.Instance.animalDeathChance = listing_Standard.Slider(CDRSettings.Instance.animalDeathChance, 0f, 1f);
            Text.Font = GameFont.Tiny;
            listing_Standard.Label("    " + CDRSettings.Instance.animalDeathChance, -1f);
            listing_Standard.Gap(12f);
            GUI.contentColor = Color.white;
            listing_Standard.Label("CDR_PawnDeathRate".Translate(new object[]
            {
                CDRSettings.Instance.pawnDeathChance
            }), -1f);
            GUI.contentColor = Color.yellow;
            CDRSettings.Instance.pawnDeathChance = listing_Standard.Slider(CDRSettings.Instance.pawnDeathChance, 0f, 1f);
            Text.Font = GameFont.Tiny;
            listing_Standard.Label("    " + CDRSettings.Instance.pawnDeathChance, -1f);
            listing_Standard.End();
        }
    }
}
