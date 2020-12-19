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
}
