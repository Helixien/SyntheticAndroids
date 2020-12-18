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

	[HarmonyPatch(typeof(GenHostility), "HostileTo", new Type[] 
    {
        typeof(Thing),
        typeof(Thing)
    })]
	public static class HostileTo_Patch
	{
		public static void Postfix(Thing a, Thing b, ref bool __result)
		{
			if (__result && a is Pawn pawn1 && b is Pawn pawn2)
            {
				if (pawn1.RaceProps.IsMechanoid && pawn2.IsAndroid())
                {
					if (AreNeutral(pawn2, pawn1))
                    {
						__result = false;
						return;
                    }
                }
				else if (pawn1.IsAndroid() && pawn2.RaceProps.IsMechanoid)
                {
					if (AreNeutral(pawn1, pawn2))
					{
						__result = false;
						return;
					}
				}
            }
		}

		private static bool AreNeutral(Pawn android, Pawn mechanoid)
        {
			if (mechanoid.mindState?.meleeThreat?.IsAndroid() ?? false)
			{
				return false;
			}
			else if (mechanoid.mindState.enemyTarget is Pawn pawn && pawn.IsAndroid())
            {
				return false;
            }
            foreach (var log in Find.BattleLog.Battles)
            {
                foreach (var entry in log.Entries)
                {
                    if (entry.GetConcerns().Contains(mechanoid))
                    {
                        foreach (var p in entry.GetConcerns())
                        {
                            if (p != mechanoid && p is Pawn pawn && pawn.IsAndroid() && pawn.Faction == android.Faction)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
	}
}
