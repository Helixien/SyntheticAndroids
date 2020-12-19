using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SyntheticAndroids
{
	[HarmonyPatch(typeof(RelationsUtility), "TryDevelopBondRelation")]
	public static class TryDevelopBondRelation_Patch
	{
		public static bool Prefix(Pawn humanlike, Pawn animal, ref float baseChance)
		{
			if (humanlike.IsAndroid() && !humanlike.HasTrait(SADefOf.SA_Sentient))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight")]
	public static class InteractionWorker_RomanceAttempt_RandomSelectionWeight_Patch
	{
		public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
		{
			if (initiator.IsAndroid() && !initiator.HasTrait(SADefOf.SA_Sentient))
			{
				__result = 0f;
			}
		}
	}
}
