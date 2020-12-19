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
    //[HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    //public static class GenerateTraits_Patch
    //{
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
    //        MethodInfo defListInfo = AccessTools.Property(typeof(DefDatabase<TraitDef>), "AllDefsListForReading").GetGetMethod();
    //        MethodInfo validatorInfo = AccessTools.Method(typeof(GenerateTraits_Patch), "GenerateTraitsValidator", null, null);
    //        foreach (CodeInstruction instruction in list)
    //        {
    //            if (instruction.opcode == OpCodes.Call && CodeInstructionExtensions.OperandIs(instruction, defListInfo))
    //            {
    //                yield return new CodeInstruction(OpCodes.Ldarg_0, null);
    //                instruction.operand = validatorInfo;
    //            }
    //            yield return instruction;
    //        }
    //        yield break;
    //    }
    //
    //    public static IEnumerable<TraitDef> GenerateTraitsValidator(Pawn p)
    //    {
    //        return GenerateTraits(p, DefDatabase<TraitDef>.AllDefsListForReading);
    //    }
    //
    //    public static IEnumerable<TraitDef> GenerateTraits(Pawn p, List<TraitDef> traitDefs)
    //    {
    //        var options = p.def.GetModExtension<AndroidOptions>();
    //        if (options != null)
    //        {
    //            traitDefs = traitDefs.Where(x => !options.disallowedTraits.Contains(x)).ToList();
    //            var options2 = p.kindDef.GetModExtension<AndroidOptions>();
    //            if (options2 != null)
    //            {
    //                traitDefs = traitDefs.Where(x => !options.disallowedTraits.Contains(x)).ToList();
    //            }
    //            traitDefs.RemoveAll(x => x is AndroidTraitDef androidTraitDef && androidTraitDef.allowedPawnKindDefs?.Count > 0 && !androidTraitDef.allowedPawnKindDefs.Contains(p.kindDef));
    //        }
    //        return traitDefs;
    //    }
    //}
    //
    //[StaticConstructorOnStartup]
    //public static class GenerateAlienTraits_Patch
    //{
    //    static GenerateAlienTraits_Patch()
    //    {
    //        if (ModLister.HasActiveModWithName("Humanoid Alien Races 2.0"))
    //        {
    //            HarmonyInit.harmonyInstance.Patch(AccessTools.Method(AccessTools.TypeByName("AlienRace.HarmonyPatches"), "GenerateTraitsValidator", null, null), null,
    //                new HarmonyMethod(typeof(GenerateAlienTraits_Patch), "GenerateTraitsValidator", null), null, null);
    //        }
    //    }
    //    public static void GenerateTraitsValidator(ref IEnumerable<TraitDef> __result, Pawn p)
    //    {
    //        __result = GenerateTraits_Patch.GenerateTraits(p, __result.ToList());
    //    }
    //}

    //[HarmonyPatch(typeof(TraitSet), "GainTrait")]
    //public static class GainTrait_Patch
    //{
//public static bool Prefix(Pawn ___pawn)
//{
//    if (___pawn.IsAndroid() && !___pawn.HasTrait(SADefOf.SA_Sentient) && ___pawn.story.traits.allTraits.Count() >= 2)
//    {
//        return false;
//    }
//    return true;
//}


    //[HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    //internal static class GenerateTraits_Patch2
    //{
    //    [HarmonyDebug]
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    //    {
    //        List<CodeInstruction> instructionList = instructions.ToList();
    //        int index = -1;
    //        bool found = false;
    //        for (int i = 0; i < instructionList.Count; i++)
    //        {
    //            if (!found && instructionList[i].opcode == OpCodes.Ldfld && instructionList[i + 6].opcode == OpCodes.Blt)
    //            {
    //                found = true;
    //                index = i + 6;
    //            }
    //
    //            if (index != i)
    //            {
    //                yield return new CodeInstruction(instructionList[i]);
    //            }
    //            else
    //            {
    //                Log.Message("TEST: " + instructionList[i]);
    //                yield return new CodeInstruction(OpCodes.Clt);
    //                yield return new CodeInstruction(OpCodes.Ldarg_0);
    //                yield return new CodeInstruction(OpCodes.Call, typeof(GenerateTraits_Patch2).GetMethod("AndroidCheck"));
    //                yield return new CodeInstruction(OpCodes.And);
    //                yield return new CodeInstruction(OpCodes.Brtrue).MoveLabelsFrom(instructionList[i]);
    //            }
    //        }
    //        yield break;
    //    }
    //    public static bool AndroidCheck(Pawn pawn)
    //    {
    //        Log.Message("Check" + pawn);
    //        return false;
    //    }
    //}
}