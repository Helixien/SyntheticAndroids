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
    [HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    public static class GenerateTraits_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
            MethodInfo defListInfo = AccessTools.Property(typeof(DefDatabase<TraitDef>), "AllDefsListForReading").GetGetMethod();
            MethodInfo validatorInfo = AccessTools.Method(typeof(GenerateTraits_Patch), "GenerateTraitsValidator", null, null);
            foreach (CodeInstruction instruction in list)
            {
                if (instruction.opcode == OpCodes.Call && CodeInstructionExtensions.OperandIs(instruction, defListInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0, null);
                    instruction.operand = validatorInfo;
                }
                yield return instruction;
            }
            yield break;
        }

        public static IEnumerable<TraitDef> GenerateTraitsValidator(Pawn p)
        {
            return GenerateTraits(p, DefDatabase<TraitDef>.AllDefsListForReading);
        }

        public static IEnumerable<TraitDef> GenerateTraits(Pawn p, List<TraitDef> traitDefs)
        {
            var options = p.def.GetModExtension<PawnSpawnOptions>();
            var list = traitDefs.Where(x => !options.disallowedTraits.Contains(x)).ToList();
            if (p.kindDef.HasModExtension<PawnSpawnOptions>())
            {
                var options2 = p.kindDef.GetModExtension<PawnSpawnOptions>();
                list = list.Where(x => !options.disallowedTraits.Contains(x)).ToList();
            }
            list.RemoveAll(x => x is AndroidTraitDef androidTraitDef && androidTraitDef.allowedPawnKindDefs?.Count > 0 && !androidTraitDef.allowedPawnKindDefs.Contains(p.kindDef));
            return list;
        }
    }

    [StaticConstructorOnStartup]
    public static class GenerateAlienTraits_Patch
    {
        static GenerateAlienTraits_Patch()
        {
            if (ModLister.HasActiveModWithName("Humanoid Alien Races 2.0"))
            {
                HarmonyInit.harmonyInstance.Patch(AccessTools.Method(AccessTools.TypeByName("AlienRace.HarmonyPatches"), "GenerateTraitsValidator", null, null), null,
                    new HarmonyMethod(typeof(GenerateAlienTraits_Patch), "GenerateTraitsValidator", null), null, null);
            }
        }
        public static void GenerateTraitsValidator(ref IEnumerable<TraitDef> __result, Pawn p)
        {
            __result = GenerateTraits_Patch.GenerateTraits(p, __result.ToList());
        }
    }



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