using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SyntheticAndroids
{

    [HarmonyPatch(typeof(Pawn_AbilityTracker), "AbilitiesTick")]
    public static class AbilitiesTick_Patch
    {
        public static bool Prefix(Pawn_AbilityTracker __instance)
        {
            if (__instance.pawn.IsAndroid())
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_AbilityTracker), "GainAbility")]
    public static class GainAbility_Patch
    {
        public static bool Prefix(Pawn_AbilityTracker __instance)
        {
            if (__instance.pawn.IsAndroid())
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "TryGenerateNewPawnInternal")]
    public static class TryGenerateNewPawnInternal_Patch
    {
        public static void Postfix(ref Pawn __result)
        {
            if (__result != null && __result.IsAndroid())
            {
                if (__result.ageTracker.AgeBiologicalYears > 40)
                {
                    __result.story.hairColor = PawnHairColors.RandomHairColor(__result.story.SkinColor, 40);
                }
            
                if (__result.story.GetBackstory(BackstorySlot.Childhood).identifier != "SA_AndroidCreated45")
                {
                    if (BackstoryDatabase.TryGetWithIdentifier("SA_AndroidCreated45", out Backstory bs))
                    {
                        __result.story.childhood = bs;
                    }
                }
                if (!__result.HasTrait(SADefOf.SA_Sentient))
                {
                    foreach (var skill in __result.skills.skills)
                    {
                        skill.passion = Passion.None;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
    public static class SpawnSetup_Patch
    {
        public static void Postfix(Pawn __instance, Map map, bool respawningAfterLoad)
        {
            if (!respawningAfterLoad && __instance.IsAndroid())
            {
                if (__instance.foodRestriction != null)
                {
                    var foodRestriction = Current.Game.foodRestrictionDatabase.AllFoodRestrictions.Where(x => x.label == "Simple").FirstOrDefault();
                    if (foodRestriction != null)
                    {
                        __instance.foodRestriction.CurrentFoodRestriction = foodRestriction;
                    }
                }

                if (__instance.drugs != null)
                {
                    var drugPolicy = Current.Game.drugPolicyDatabase.AllPolicies.Where(x => x.label == "No drugs").FirstOrDefault();
                    if (drugPolicy != null)
                    {
                        __instance.drugs.CurrentPolicy = drugPolicy;
                    }
                }
            }
        }
    }
}