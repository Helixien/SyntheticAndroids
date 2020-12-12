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

	[HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
	public static class ShouldHaveNeed_Patch
	{
		public static bool Prefix(Pawn ___pawn, NeedDef nd, ref bool __result)
		{
			if (___pawn.IsAndroid())
			{
				if (nd == NeedDefOf.Food || nd == NeedDefOf.Rest)
                {
					return false;
				}
				else if (nd == SADefOf.SA_Energy)
                {
					__result = true;
					return false;
                }
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(MeditationUtility), "CanMeditateNow")]
	public class Patch_CanMeditateNow
	{
		private static bool Prefix(Pawn pawn, ref bool __result)
		{
			if (pawn.IsAndroid())
			{
				__result = CanMeditateNow(pawn);
				return false;
			}
			return true;
		}

		public static bool CanMeditateNow(Pawn pawn)
		{
			if (pawn.needs.rest != null && (int)pawn.needs.rest.CurCategory >= 2)
			{
				return false;
			}
			if (pawn.needs.TryGetNeed<Need_Energy>().EmptyEnergy)
			{
				return false;
			}
			if (!pawn.Awake())
			{
				return false;
			}
			if (pawn.health.hediffSet.BleedRateTotal > 0f || (HealthAIUtility.ShouldSeekMedicalRest(pawn) && pawn.timetable?.CurrentAssignment != TimeAssignmentDefOf.Meditate) || HealthAIUtility.ShouldSeekMedicalRestUrgent(pawn))
			{
				return false;
			}
			return true;
		}
	}
}
