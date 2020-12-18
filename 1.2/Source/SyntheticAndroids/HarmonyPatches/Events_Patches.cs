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
	[HarmonyPatch]
	public class MechSpawn_Patch
	{
		[HarmonyTargetMethods]
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return typeof(SymbolResolver_RandomMechanoidGroup).GetNestedTypes(AccessTools.all)
																 .First(t => t.GetMethods(AccessTools.all)
																		   .Any(mi => mi.ReturnType == typeof(bool) && mi.GetParameters()[0].ParameterType == typeof(PawnKindDef)))
																 .GetMethods(AccessTools.all)
																 .First(mi => mi.ReturnType == typeof(bool));
		}

		[HarmonyPostfix]
		public static void Postfix(PawnKindDef kind, ref bool __result)
        {
			if (kind == SADefOf.SA_AncientAndroid)
            {
				__result = true;
            }
        }
	}

	[HarmonyPatch(typeof(ThingSetMaker_RefugeePod), "Generate")]
	public static class Generate_Patch
	{
		public static bool Prefix(ThingSetMakerParams parms, ref List<Thing> outThings)
		{
			if (Rand.Chance(0.05f))
            {
				var androidKind = Rand.Bool ? SADefOf.SA_AndroidRefugee : SADefOf.SA_AndroidRefugee_Clothed;
				Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(androidKind, DownedRefugeeQuestUtility.GetRandomFactionForRefugee(), 
					PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, 
					mustBeCapableOfViolence: false, 20f));
				outThings.Add(pawn);
				HealthUtility.DamageUntilDowned(pawn);
				return false;
			}
			return true;
		}
	}
}
