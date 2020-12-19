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
	[HarmonyPatch(typeof(EquipmentUtility), "GetPersonaWeaponConfirmationText")]
	public static class GetPersonaWeaponConfirmationText_Patch
	{
		public static bool Prefix(Thing item, Pawn p)
		{
			if (p.IsAndroid() && !p.HasTrait(SADefOf.SA_Sentient))
			{
				return false;
			}
			return true;
		}
	}


	[HarmonyPatch(typeof(CompBladelinkWeapon), "BondToPawn")]
	public static class BondToPawn_Patch
	{
		public static bool Prefix(Pawn pawn)
		{
			if (pawn.IsAndroid() && !pawn.HasTrait(SADefOf.SA_Sentient))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn_RoyaltyTracker), "GainFavor")]
	public static class GainFavor_Patch
	{
		public static bool Prefix(Pawn_RoyaltyTracker __instance)
		{
			if (__instance.pawn.IsAndroid() && !__instance.pawn.HasTrait(SADefOf.SA_Sentient))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn_RoyaltyTracker), "SetTitle")]
	public static class SetTitle_Patch
	{
		public static bool Prefix(Pawn_RoyaltyTracker __instance)
		{
			if (__instance.pawn.IsAndroid() && !__instance.pawn.HasTrait(SADefOf.SA_Sentient))
			{
				return false;
			}
			return true;
		}
	}
}
