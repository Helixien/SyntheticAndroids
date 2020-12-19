using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SyntheticAndroids
{

	[HarmonyPatch(typeof(IndividualThoughtToAdd), "Add")]
	public static class Add_Patch
	{
		public static bool Prefix(IndividualThoughtToAdd __instance, Pawn ___otherPawn)
		{
			if (!ShouldGetThought(__instance.thought.def, __instance.addTo))
            {
				return false;
            }
			return true;
		}

		public static bool ShouldGetThought(ThoughtDef thoughtDef, Pawn pawn)
        {
			if (pawn.IsAndroid())
            {
				var options = pawn.def.GetModExtension<AndroidOptions>();
				if (options != null && options.disallowedThoughts.Contains(thoughtDef))
				{
					return false;
				}

				var options2 = pawn.kindDef.GetModExtension<AndroidOptions>();
				if (options2 != null && options2.disallowedThoughts.Contains(thoughtDef))
				{
					return false;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(SituationalThoughtHandler), "TryCreateThought")]
	public static class TryCreateThought_Patch
	{
		public static bool Prefix(SituationalThoughtHandler __instance, Thought_Situational __result, ThoughtDef def)
		{
			if (!Add_Patch.ShouldGetThought(def, __instance.pawn))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(SituationalThoughtHandler), "TryCreateSocialThought")]
	public static class TryCreateSocialThought_Patch
	{
		public static bool Prefix(SituationalThoughtHandler __instance, Thought_SituationalSocial __result, ThoughtDef def, Pawn otherPawn)
		{
			if (!Add_Patch.ShouldGetThought(def, __instance.pawn))
			{
				return false;
			}
			return true;
		}
	}


	[HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemory", new Type[]
	{
		typeof(Thought_Memory),
		typeof(Pawn)
	})]
	public static class TryGainMemory_Patch
	{
		private static bool Prefix(MemoryThoughtHandler __instance, ref Thought_Memory newThought, Pawn otherPawn)
		{
			if (!Add_Patch.ShouldGetThought(newThought.def, __instance.pawn))
			{
				return false;
			}
			return true;
		}
	}
}
