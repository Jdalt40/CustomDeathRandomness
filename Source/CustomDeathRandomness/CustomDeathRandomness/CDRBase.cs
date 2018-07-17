using Harmony;
using System.Reflection;
using Verse;
using Verse.AI;
using RimWorld;

namespace CustomDeathRandomness
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.torann.Pawn_HealthTracker.CustomDeathRandomness");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

        }
    }  

    [HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
    public static class CheckForStateChangePatch
    {
        public static FieldInfo pawn = typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
        public static MethodBase MakeDowned = typeof(Pawn_HealthTracker).GetMethod("MakeDowned", BindingFlags.Instance | BindingFlags.NonPublic);
        public static MethodBase MakeUnDowned = typeof(Pawn_HealthTracker).GetMethod("MakeUnDowned", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool Prefix(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff )
        {
            var verbPHT = Traverse.Create(__instance);
            Pawn pawn_ = (Pawn)CheckForStateChangePatch.pawn.GetValue(__instance);
            CDRSettingsRef CDR = new CDRSettingsRef();

            if (pawn_ != null && dinfo != null && hediff != null)
            {
                //-------------------------//
                if (!__instance.Dead)
                {

                    if (verbPHT.Method("ShouldBeDead").GetValue<bool>() && pawn != null)
                    {
                        if (!pawn_.Destroyed)
                        {
                            pawn_.Kill(dinfo, hediff);
                        }
                        return false;
                    }
                    if (!__instance.Downed)
                    {
                        if (verbPHT.Method("ShouldBeDowned").GetValue<bool>() && pawn != null)
                        {
                            float num = (!pawn_.RaceProps.Animal) ? CDR.cdrp : CDR.cdra; //pawn : animal
                            if (!__instance.forceIncap && dinfo.HasValue && dinfo.Value.Def.externalViolence && (pawn_.Faction == null || !pawn_.Faction.IsPlayer) && !pawn_.IsPrisonerOfColony && pawn_.RaceProps.IsFlesh && Rand.Value < num)
                            {
                                pawn_.Kill(dinfo, null);
                                return false;
                            }
                            __instance.forceIncap = false;
                            CheckForStateChangePatch.MakeDowned.Invoke(__instance, new object[]
                            {
                            dinfo,
                            hediff
                            });
                            return false;
                        }
                        else if (!__instance.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                        {
                            if (pawn_.carryTracker != null && pawn_.carryTracker.CarriedThing != null && pawn_.jobs != null && pawn_.CurJob != null)
                            {
                                pawn_.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
                            }
                            if (pawn_.equipment != null && pawn_.equipment.Primary != null)
                            {
                                if (pawn_.InContainerEnclosed)
                                {
                                    pawn_.equipment.TryTransferEquipmentToContainer(pawn_.equipment.Primary, pawn_.holdingOwner);
                                }
                                else if (pawn_.SpawnedOrAnyParentSpawned)
                                {
                                    ThingWithComps thingWithComps;
                                    pawn_.equipment.TryDropEquipment(pawn_.equipment.Primary, out thingWithComps, pawn_.PositionHeld, true);
                                }
                                else
                                {
                                    pawn_.equipment.DestroyEquipment(pawn_.equipment.Primary);
                                }
                            }
                        }
                    }
                    else if (!verbPHT.Method("ShouldBeDowned").GetValue<bool>() && pawn != null)
                    {
                        CheckForStateChangePatch.MakeUnDowned.Invoke(__instance, null);
                        return false;
                    }

                }
            }
            else
            {
                return true;
            }
            return false;
        }
    }    
}