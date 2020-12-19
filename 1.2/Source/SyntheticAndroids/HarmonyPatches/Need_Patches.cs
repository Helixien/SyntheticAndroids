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
			else if (nd == SADefOf.SA_Energy)
			{
				return false;
            } 
			return true;
		}
	}
}
