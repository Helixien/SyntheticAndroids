using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
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
	[HarmonyPatch(typeof(SymbolResolver_RandomMechanoidGroup), "Resolve")]
	public static class Resolve_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> instructionList = instructions.ToList<CodeInstruction>();
			MethodInfo getAllDefsListForReadingInfo = AccessTools.Property(typeof(DefDatabase<ResearchProjectDef>), "AllDefsListForReading").GetGetMethod();
			MethodInfo getAllPawnKindsInfo = AccessTools.Method(typeof(Resolve_Patch), "GetAllPawnKinds", null, null);
			for (int i = 0; i < instructionList.Count; i++)
			{
				CodeInstruction instruction = instructionList[i];
				if (instruction.opcode == OpCodes.Call && CodeInstructionExtensions.OperandIs(instruction, getAllDefsListForReadingInfo))
				{
					yield return instruction;
					instruction = new CodeInstruction(OpCodes.Call, getAllPawnKindsInfo);
				}
				yield return instruction;
			}
			yield break;
		}

		private static List<PawnKindDef> GetAllPawnKinds()
		{
			return DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef kind) => kind.RaceProps.IsMechanoid).ToList();
		}
	}

	public class SymbolResolver_RandomMechanoidGroup_Patch
	{
		[HarmonyPatch(typeof(SymbolResolver_RandomMechanoidGroup), "Resolve")]
		public class Resolve
		{
			internal static void Postfix()
			{
				CheckStackItemRecursive();
			}

			private static void CheckStackItemRecursive()
			{
				if (BaseGen.symbolStack.Empty)
				{
					return;
				}
				SymbolStack.Element element = BaseGen.symbolStack.Pop();
				if (element.symbol == "pawn" && element.resolveParams.faction == Faction.OfMechanoids)
				{
					element.resolveParams.singlePawnKindDef = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef kind) 
							=> kind.RaceProps.IsMechanoid).RandomElementByWeight((PawnKindDef kind) => 1f / kind.combatPower);
					CheckStackItemRecursive();
				}
				BaseGen.symbolStack.Push(element.symbol, element.resolveParams, element.symbolPath);
			}
		}
	}
}
