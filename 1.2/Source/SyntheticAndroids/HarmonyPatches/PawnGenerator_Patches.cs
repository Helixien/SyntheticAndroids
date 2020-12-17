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
			if (p.def.HasModExtension<PawnSpawnOptions>())
            {
				var options = p.def.GetModExtension<PawnSpawnOptions>();
				return from tr in DefDatabase<TraitDef>.AllDefs where !options.disallowedTraits.Contains(tr) select tr;
			}
			else
            {
				return DefDatabase<TraitDef>.AllDefsListForReading;
			}
		}
	}

	[StaticConstructorOnStartup]
	public static class GenerateAlienTraits_Patch
	{
		static GenerateAlienTraits_Patch()
        {
			if (ModLister.HasActiveModWithName("Humanoid Alien Races 2.0"))
            {
				HarmonyInit.harmonyInstance.Patch(AccessTools.Method(typeof(AlienRace.HarmonyPatches), "GenerateTraitsValidator", null, null), null, new HarmonyMethod(typeof(GenerateAlienTraits_Patch),
					"GenerateTraitsValidator", null), null, null);
			}
		}
		public static void GenerateTraitsValidator(ref IEnumerable<TraitDef> __result, Pawn p)
		{
			var list = __result.ToList();
			if (p.def.HasModExtension<PawnSpawnOptions>())
			{
				var options = p.def.GetModExtension<PawnSpawnOptions>();
				list = list.Where(x => !options.disallowedTraits.Contains(x)).ToList();
				__result = list;
			}
		}
	}
}
