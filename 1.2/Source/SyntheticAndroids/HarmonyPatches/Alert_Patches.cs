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

	[HarmonyPatch(typeof(Alert_NeedDoctor), "Patients", MethodType.Getter)]
	public static class Patients_Patch
	{
		public static bool Prefix(ref List<Pawn> __result)
		{
			__result = Patients();
			return false;
		}

		private static List<Pawn> patientsResult = new List<Pawn>();
		private static List<Pawn> Patients()
		{
				patientsResult.Clear();
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					if (maps[i].IsPlayerHome)
					{
						bool flag = false;
						foreach (Pawn freeColonist in maps[i].mapPawns.FreeColonists)
						{
							if ((freeColonist.Spawned || freeColonist.BrieflyDespawned()) && !freeColonist.Downed && freeColonist.workSettings != null && freeColonist.workSettings.WorkIsActive(WorkTypeDefOf.Doctor))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							foreach (Pawn freeColonist2 in maps[i].mapPawns.FreeColonists)
							{
								if ((freeColonist2.Spawned || freeColonist2.BrieflyDespawned()) && ((freeColonist2.Downed && freeColonist2.needs?.food != null
								&& (int)freeColonist2.needs.food?.CurCategory < 0 
								&& freeColonist2.InBed()) || HealthAIUtility.ShouldBeTendedNowByPlayer(freeColonist2)))
								{
									patientsResult.Add(freeColonist2);
								}
							}
						}
					}
				}
				return patientsResult;
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
