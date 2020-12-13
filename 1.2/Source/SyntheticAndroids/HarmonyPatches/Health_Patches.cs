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
	[HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDowned")]
	public static class ShouldBeDowned_Patch
	{
		public static bool Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, ref bool __result)
		{
			if (___pawn.IsAndroid())
			{
				var androidComp = ___pawn.GetAndroidComp();
				if (androidComp.downedState || androidComp.disabled)
                {
					__result = true;
					return false;
                }
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PawnDownedWiggler), "WigglerTick")]
	public static class WigglerTick_Patch
	{
		public static bool Prefix(PawnDownedWiggler __instance, Pawn ___pawn)
		{
			if (___pawn.IsAndroid())
			{
				var androidComp = ___pawn.GetAndroidComp();
				if (androidComp.disabled)
				{
					__instance.ticksToIncapIcon--;
					if (__instance.ticksToIncapIcon <= 0)
					{
						MoteMaker.ThrowMetaIcon(___pawn.Position, ___pawn.Map, SADefOf.SA_Mote_AndroidDisabledIcon);
						__instance.ticksToIncapIcon = 200;
					}
					return false;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(HealthUtility), "GetGeneralConditionLabel")]
	public static class GetGeneralConditionLabel_Patch
	{
		public static bool Prefix(ref string __result, Pawn pawn, bool shortVersion = false)
		{
			if (pawn.IsAndroid())
			{
				__result = GetGeneralConditionLabel(pawn, shortVersion);
			}
			return true;
		}
		public static string GetGeneralConditionLabel(Pawn pawn, bool shortVersion = false)
		{
			if (pawn.health.Dead)
			{
				return "SA.Destroyed".Translate();
			}
			if (!pawn.health.capacities.CanBeAwake)
			{
				return "Unconscious".Translate();
			}
			if (pawn.health.InPainShock)
			{
				return (shortVersion && "PainShockShort".CanTranslate()) ? "PainShockShort".Translate() : "PainShock".Translate();
			}
			if (pawn.Downed)
			{
				return "Incapacitated".Translate();
			}
			bool flag = false;
			for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury = pawn.health.hediffSet.hediffs[i] as Hediff_Injury;
				if (hediff_Injury != null && !hediff_Injury.IsPermanent())
				{
					flag = true;
				}
			}
			if (flag)
			{
				return "Injured".Translate();
			}
			if (pawn.health.hediffSet.PainTotal > 0.3f)
			{
				return "InPain".Translate();
			}
			return "Healthy".Translate();
		}
	}
}
