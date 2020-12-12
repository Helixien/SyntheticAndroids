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
	public enum EnergyCategory : byte
	{
		Full,
		EnoughEnergy,
		NeedEnergy,
		Empty
	}
	public class Need_Energy : Need
	{
		private int lastRestTick = -999;

		private float lastRestEffectiveness = 1f;

		private int ticksAtZero;
		public int TicksAtZero => ticksAtZero;
		private bool Resting => Find.TickManager.TicksGame < lastRestTick + 2;

		private int lastChargingTick = -99999;
		public bool EmptyEnergy => CurCategory == EnergyCategory.Empty;
		public EnergyCategory CurCategory
		{
			get
			{
				if (base.CurLevelPercentage <= 0f)
				{
					return EnergyCategory.Empty;
				}
				if (base.CurLevelPercentage < 0.25f)
				{
					return EnergyCategory.NeedEnergy;
				}
				if (base.CurLevelPercentage < 0.75f)
				{
					return EnergyCategory.EnoughEnergy;
				}
				return EnergyCategory.Full;
			}
		}

		public float EnergyNeedFallPerTick => 2.66666666E-05f;

		public override int GUIChangeArrow => -1;

		public override float MaxLevel => 1f;
		public float EnergyWanted => MaxLevel - CurLevel;
		public int TicksNeedEnergy => Mathf.Max(0, Find.TickManager.TicksGame - lastChargingTick);
		public Need_Energy(Pawn pawn)
			: base(pawn)
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref lastChargingTick, "lastChargingTick", -99999);
			Scribe_Values.Look(ref ticksAtZero, "ticksAtZero", 0);
		}

		public override void NeedInterval()
		{
			Log.Message(" - NeedInterval - if (!IsFrozen) - 1", true);
			if (!IsFrozen)
			{
				Log.Message(" - NeedInterval - if (Resting) - 2", true);
				if (Resting)
				{
					Log.Message(" - NeedInterval - float num = lastRestEffectiveness; - 3", true);
					float num = lastRestEffectiveness;
					Log.Message(" - NeedInterval - num *= pawn.GetStatValue(StatDefOf.RestRateMultiplier); - 4", true);
					num *= pawn.GetStatValue(StatDefOf.RestRateMultiplier);
					Log.Message(" - NeedInterval - if (num > 0f) - 5", true);
					if (num > 0f)
					{
						Log.Message(" - NeedInterval - CurLevel += 0.005714286f * num; - 6", true);
						CurLevel += 0.005714286f * num;
					}
				}
				else
				{
					Log.Message(" - CurLevel -= EnergyNeedFallPerTick * 150f - 2", true);
					CurLevel -= EnergyNeedFallPerTick * 150f;
				}
			}
			Log.Message(" - NeedInterval - if (!EmptyEnergy) - 8", true);
			if (!EmptyEnergy)
			{
				lastChargingTick = Find.TickManager.TicksGame;
			}
			Log.Message(" - NeedInterval - if (CurLevel < 0.0001f) - 10", true);
			if (CurLevel < 0.0001f)
			{
				Log.Message(" - NeedInterval - ticksAtZero += 150; - 11", true);
				ticksAtZero += 150;
			}
			else
			{
				Log.Message(" - NeedInterval - ticksAtZero = 0; - 12", true);
				ticksAtZero = 0;
			}
			Log.Message(" - NeedInterval - if (ticksAtZero <= 1000 || !pawn.Spawned) - 13", true);
			if (ticksAtZero <= 1000 || !pawn.Spawned)
			{
				Log.Message(" - NeedInterval - return; - 14", true);
				return;
			}

			float mtb = (ticksAtZero < 15000) ? 0.25f : ((ticksAtZero < 30000) ? 0.125f : ((ticksAtZero >= 45000) ? 0.0625f : 0.0833333358f));
			Log.Message(" - NeedInterval - if (Rand.MTBEventOccurs(mtb, 60000f, 150f) && (pawn.CurJob == null || pawn.CurJob.def != JobDefOf.LayDown)) - 16", true);
			if (Rand.MTBEventOccurs(mtb, 60000f, 150f) && (pawn.CurJob == null || pawn.CurJob.def != JobDefOf.LayDown))
			{
				Log.Message(" - NeedInterval - pawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.LayDown, pawn.Position), JobCondition.InterruptForced, null, resumeCurJobAfterwards: false, cancelBusyStances: true, null, JobTag.SatisfyingNeeds); - 17", true);
				pawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.LayDown, pawn.Position), JobCondition.InterruptForced, null, resumeCurJobAfterwards: false, cancelBusyStances: true, null, JobTag.SatisfyingNeeds);
				Log.Message(" - NeedInterval - if (pawn.InMentalState && pawn.MentalStateDef.recoverFromCollapsingExhausted) - 18", true);
				if (pawn.InMentalState && pawn.MentalStateDef.recoverFromCollapsingExhausted)
				{
					Log.Message(" - NeedInterval - pawn.mindState.mentalStateHandler.CurState.RecoverFromState(); - 19", true);
					pawn.mindState.mentalStateHandler.CurState.RecoverFromState();
				}
				Log.Message(" - NeedInterval - if (PawnUtility.ShouldSendNotificationAbout(pawn)) - 20", true);
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Log.Message(" - NeedInterval - Messages.Message(\"MessageInvoluntarySleep\".Translate(pawn.LabelShort, pawn), pawn, MessageTypeDefOf.NegativeEvent); - 21", true);
					Messages.Message("MessageInvoluntarySleep".Translate(pawn.LabelShort, pawn), pawn, MessageTypeDefOf.NegativeEvent);
				}
				TaleRecorder.RecordTale(TaleDefOf.Exhausted, pawn);
			}
		}


		public override void SetInitialLevel()
		{
			base.CurLevelPercentage = Rand.Range(0.5f, 0.9f);
			if (Current.ProgramState == ProgramState.Playing)
			{
				lastChargingTick = Find.TickManager.TicksGame;
			}
		}

		public override string GetTipString()
		{
			return base.LabelCap + ": " + base.CurLevelPercentage.ToStringPercent() + " (" + CurLevel.ToString("0.##") + " / " + MaxLevel.ToString("0.##") + ")\n" + def.description;
		}



		public void TickResting(float restEffectiveness)
		{
			if (!(restEffectiveness <= 0f))
			{
				Log.Message("Last rest tick:" + lastRestTick);
				lastRestTick = Find.TickManager.TicksGame;
				lastRestEffectiveness = restEffectiveness;
			}
		}
	}
}
