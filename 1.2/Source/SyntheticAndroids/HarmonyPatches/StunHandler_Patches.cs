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

	[HarmonyPatch(typeof(StunHandler), "EMPAdaptationTicksDuration", MethodType.Getter)]
	public static class EMPAdaptationTicksDuration_Patch
	{
		public static bool Prefix(StunHandler __instance, ref int __result)
		{
			if (__instance.parent is Pawn pawn && pawn.IsAndroid())
			{
				__result = 2200;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(StunHandler), "AffectedByEMP", MethodType.Getter)]
	public class AffectedByEMP_Patch
	{
		private static bool Prefix(StunHandler __instance, ref bool __result)
		{
			if (__instance.parent is Pawn pawn && pawn.IsAndroid())
			{
				__result = true;
				return false;
			}
			return true;
		}
	}
}
