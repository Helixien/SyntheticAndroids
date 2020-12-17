using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SyntheticAndroids
{
	[HarmonyPatch(typeof(HealthCardUtility), "DrawOverviewTab")]
	public static class DrawOverviewTab_Patch
	{
		public static bool Prefix(ref float __result, Rect leftRect, Pawn pawn, float curY)
		{
			if (pawn.IsAndroid())
			{
				__result = DrawOverviewTabAndroid(leftRect, pawn, curY);
				return false;
			}
			return true;
		}
		private static float DrawOverviewTabAndroid(Rect leftRect, Pawn pawn, float curY)
		{
			curY += 4f;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = new Color(0.9f, 0.9f, 0.9f);
			string str = (pawn.gender == Gender.None) ? ((string)"PawnSummary".Translate(pawn.Named("PAWN"))) : ((string)"PawnSummaryWithGender".Translate(pawn.Named("PAWN")));
			Rect rect = new Rect(0f, curY, leftRect.width, 34f);
			Widgets.Label(rect, str.CapitalizeFirst());
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, () => pawn.ageTracker.AgeTooltipString, 73412);
				Widgets.DrawHighlight(rect);
			}
			GUI.color = Color.white;
			curY += 34f;
			bool flag = pawn.RaceProps.IsFlesh && (pawn.Faction == Faction.OfPlayer || pawn.HostFaction == Faction.OfPlayer || (pawn.NonHumanlikeOrWildMan() && pawn.InBed() && pawn.CurrentBed().Faction == Faction.OfPlayer));
			if (pawn.foodRestriction != null && pawn.foodRestriction.Configurable)
			{
				Rect rect2 = new Rect(0f, curY, leftRect.width * 0.42f, 23f);
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(rect2, "FoodRestriction".Translate());
				GenUI.ResetLabelAlign();
				if (Widgets.ButtonText(new Rect(rect2.width, curY, leftRect.width - rect2.width, 23f), pawn.foodRestriction.CurrentFoodRestriction.label))
				{
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					List<FoodRestriction> allFoodRestrictions = Current.Game.foodRestrictionDatabase.AllFoodRestrictions;
					for (int i = 0; i < allFoodRestrictions.Count; i++)
					{
						FoodRestriction localRestriction = allFoodRestrictions[i];
						list.Add(new FloatMenuOption(localRestriction.label, delegate
						{
							pawn.foodRestriction.CurrentFoodRestriction = localRestriction;
						}));
					}
					list.Add(new FloatMenuOption("ManageFoodRestrictions".Translate(), delegate
					{
						Find.WindowStack.Add(new Dialog_ManageFoodRestrictions(null));
					}));
					Find.WindowStack.Add(new FloatMenu(list));
				}
				curY += 23f;
			}
			if (pawn.IsColonist && !pawn.Dead)
			{
				bool selfTend = pawn.playerSettings.selfTend;
				Rect rect3 = new Rect(0f, curY, leftRect.width, 24f);
				Widgets.CheckboxLabeled(rect3, "SelfTend".Translate(), ref pawn.playerSettings.selfTend);
				if (pawn.playerSettings.selfTend && !selfTend)
				{
					if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
					{
						pawn.playerSettings.selfTend = false;
						Messages.Message("MessageCannotSelfTendEver".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.RejectInput, historical: false);
					}
					else if (pawn.workSettings.GetPriority(WorkTypeDefOf.Doctor) == 0)
					{
						Messages.Message("MessageSelfTendUnsatisfied".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.CautionInput, historical: false);
					}
				}
				if (Mouse.IsOver(rect3))
				{
					TooltipHandler.TipRegion(rect3, "SelfTendTip".Translate(Faction.OfPlayer.def.pawnsPlural, 0.7f.ToStringPercent()).CapitalizeFirst());
				}
				curY += 28f;
			}
			if (flag && pawn.playerSettings != null && !pawn.Dead && Current.ProgramState == ProgramState.Playing)
			{
				MedicalCareUtility.MedicalCareSetter(new Rect(0f, curY, 140f, 28f), ref pawn.playerSettings.medCare);
				if (Widgets.ButtonText(new Rect(leftRect.width - 70f, curY, 70f, 28f), "MedGroupDefaults".Translate()))
				{
					Find.WindowStack.Add(new Dialog_MedicalDefaults());
				}
				curY += 32f;
			}
			Text.Font = GameFont.Small;
			//if (pawn.def.race.IsFlesh)
			//{
			//	Pair<string, Color> painLabel = HealthCardUtility.GetPainLabel(pawn);
			//	string painTip = HealthCardUtility.GetPainTip(pawn);
			//	curY = DrawLeftRow(leftRect, curY, "PainLevel".Translate(), painLabel.First, painLabel.Second, painTip);
			//}
			if (!pawn.Dead)
			{
				IEnumerable<PawnCapacityDef> source = GetAndroidCapacities();
				{
					foreach (PawnCapacityDef item in source.OrderBy((PawnCapacityDef act) => act.listOrder))
					{
						if (PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, item))
						{
							PawnCapacityDef activityLocal = item;
							Pair<string, Color> efficiencyLabel = HealthCardUtility.GetEfficiencyLabel(pawn, item);
							Func<string> textGetter = () => (!pawn.Dead) ? HealthCardUtility.GetPawnCapacityTip(pawn, activityLocal) : "";
							curY = DrawLeftRow(leftRect, curY, item.GetLabelFor(pawn.RaceProps.IsFlesh, pawn.RaceProps.Humanlike).CapitalizeFirst(), efficiencyLabel.First, efficiencyLabel.Second, new TipSignal(textGetter, pawn.thingIDNumber ^ item.index));
						}
					}
					return curY;
				}
			}
			return curY;
		}

		public static IEnumerable<PawnCapacityDef> GetAndroidCapacities()
        {
			var list = new List<PawnCapacityDef>();
			return DefDatabase<PawnCapacityDef>.AllDefs.Where((PawnCapacityDef x) => x.showOnHumanlikes);

		}
		private static float DrawLeftRow(Rect leftRect, float curY, string leftLabel, string rightLabel, Color rightLabelColor, TipSignal tipSignal)
		{
			Rect rect = new Rect(0f, curY, leftRect.width, 20f);
			if (Mouse.IsOver(rect))
			{
				GUI.color = HighlightColor;
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
			GUI.color = Color.white;
			Widgets.Label(new Rect(0f, curY, leftRect.width * 0.65f, 30f), leftLabel);
			GUI.color = rightLabelColor;
			Widgets.Label(new Rect(leftRect.width * 0.65f, curY, leftRect.width * 0.35f, 30f), rightLabel);
			Rect rect2 = new Rect(0f, curY, leftRect.width, 20f);
			if (Mouse.IsOver(rect2))
			{
				TooltipHandler.TipRegion(rect2, tipSignal);
			}
			curY += 20f;
			return curY;
		}

		private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);
	}

	[HarmonyPatch(typeof(HealthCardUtility))]
	[HarmonyPatch("DrawHediffRow")]
	[StaticConstructorOnStartup]
	public static class HealthCardUtility_DrawHediffRow
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo bleedingIconField = AccessTools.Field(typeof(HealthCardUtility), "BleedingIcon");
			MethodInfo iconHelper = AccessTools.Method(typeof(HealthCardUtility_DrawHediffRow), "TransformIconColorBlueIfFemale", null, null);
			androidBleedingIcon = ContentFinder<Texture2D>.Get("UI/Icons/Medical/Leaking", true);
			foreach (CodeInstruction code in instructions)
			{
				yield return code;
				if (code.opcode == OpCodes.Ldsfld && code.operand == bleedingIconField)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_1, null);
					yield return new CodeInstruction(OpCodes.Call, iconHelper);
				}
			}
			yield break;
		}

		public static Texture2D TransformIconColorBlueIfFemale(Texture2D original, Pawn pawn)
		{
			Texture2D result;
			if (pawn.IsAndroid())
			{
				result = androidBleedingIcon;
			}
			else
			{
				result = original;
			}
			return result;
		}
		private static Texture2D androidBleedingIcon;
	}

	[HarmonyPatch(typeof(Pawn_HealthTracker), "Downed", MethodType.Getter)]
	public static class Downed_Patch
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

	[HarmonyPatch(typeof(HediffUtility), "CanHealNaturally")]
	public static class CanHealNaturally_Patch
	{
		public static bool Prefix(Hediff_Injury hd)
		{
			if (hd.pawn.IsAndroid())
			{
				return false;
			}
			return true;
		}
	}


	[HarmonyPatch(typeof(HediffSet), "CalculatePain")]
	public static class CalculatePain_Patch
	{
		public static bool Prefix(HediffSet __instance, ref float __result)
		{
			if (__instance.pawn.IsAndroid())
			{
				__result = 0f;
				return false;
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
