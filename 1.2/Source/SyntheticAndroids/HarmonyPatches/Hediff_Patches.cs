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

	[HarmonyPatch(typeof(Hediff), "CauseDeathNow")]
	public static class Hediff_CauseDeathNow
	{
		public static void Postfix(Hediff __instance, ref bool __result)
		{
			if (__result && (__instance.pawn?.IsAndroid() ?? false) && __instance.def == HediffDefOf.BloodLoss)
			{
				__result = false;
				var androidComp = SA_Utils.GetAndroidComp(__instance.pawn);
				androidComp.MakeDisabled();
			}
		}
	}


	[HarmonyPatch(typeof(Hediff), "PostAdd")]
	public static class Hediff_PostAdd_Patch
	{
		public static void Postfix(Hediff __instance, DamageInfo? dinfo)
		{
			if (__instance.Part != null && (__instance.pawn?.IsAndroid() ?? false) && __instance is Hediff_MissingPart)
            {
				if (__instance.Part.def == SADefOf.SA_PersonalityMatrix)
				{
					var androidComp = __instance.pawn.GetAndroidComp();
					if (Rand.Chance(0.5f))
					{
						androidComp.MakeDazed();
					}
					else
					{
						if (Rand.Chance(0.5f))
						{
							androidComp.MakeDowned();
						}
						else
						{
							androidComp.MakeManhunting();
						}
					}
				}

				else if (__instance.Part.def == SADefOf.SA_CentralProcessor && !__instance.pawn.Dead)
				{
					if (dinfo.HasValue)
					{
						__instance.pawn.Kill(new DamageInfo(dinfo.Value.Def, dinfo.Value.Amount, instigator: dinfo.Value.Instigator), __instance);
					}
					else
					{
						__instance.pawn.Kill(null, __instance);
					}
				}

				else if (__instance.Part.def == SADefOf.SA_PowerCell)
                {
					var androidComp = __instance.pawn.GetAndroidComp();
					androidComp.MakeDisabled();
				}
			}

		}
	}

	[HarmonyPatch(typeof(Hediff), nameof(Hediff.PostRemoved))]
	public static class Hediff_MissingPart_PostRemoved_Patch
	{
		public static void Postfix(Hediff __instance)
		{
			if (__instance.pawn.IsAndroid() && __instance is Hediff_MissingPart)
            {
				if (__instance.Part.def == SADefOf.SA_PersonalityMatrix)
				{
					var androidComp = __instance.pawn.GetAndroidComp();
					if (androidComp.dazedState)
					{
						androidComp.EndDazedState();
					}
					if (androidComp.manhuntingState)
					{
						androidComp.EndManhunting();
					}
					if (androidComp.downedState)
					{
						androidComp.EndDownedState();
					}
				}
				else if (__instance.Part.def == SADefOf.SA_PowerCell)
                {
					var androidComp = __instance.pawn.GetAndroidComp();
					androidComp.TryMakeEnabled();
				}
			}
		}
	}

}
