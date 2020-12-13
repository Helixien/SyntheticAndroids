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

		public CompAndroid androidComp;
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
			if (!IsFrozen)
			{
				if (Resting)
				{
					float num = lastRestEffectiveness;
					num *= pawn.GetStatValue(StatDefOf.RestRateMultiplier);
					if (num > 0f)
					{
						CurLevel += 0.005714286f * num;
					}
				}
				else
				{
					CurLevel -= EnergyNeedFallPerTick * 150f;
				}
			}
			if (!EmptyEnergy)
			{
				lastChargingTick = Find.TickManager.TicksGame;
			}
			if (this.androidComp == null) 
				this.androidComp = SA_Utils.GetAndroidComp(this.pawn);

			if (CurLevel < 0.0001f)
			{
				ticksAtZero += 150;
				if (!androidComp.disabled)
                {
					androidComp.MakeDisabled();
                }
			}
			else
			{
				ticksAtZero = 0;
				if (androidComp.disabled)
                {
					androidComp.TryMakeEnabled();
                }
			}
			if (ticksAtZero <= 1000 || !pawn.Spawned)
			{
				return;
			}

			float mtb = (ticksAtZero < 15000) ? 0.25f : ((ticksAtZero < 30000) ? 0.125f : ((ticksAtZero >= 45000) ? 0.0625f : 0.0833333358f));
			if (Rand.MTBEventOccurs(mtb, 60000f, 150f) && (pawn.CurJob == null || pawn.CurJob.def != JobDefOf.LayDown))
			{
				pawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.LayDown, pawn.Position), JobCondition.InterruptForced, null, resumeCurJobAfterwards: false, cancelBusyStances: true, null, JobTag.SatisfyingNeeds);
				if (pawn.InMentalState && pawn.MentalStateDef.recoverFromCollapsingExhausted)
				{
					pawn.mindState.mentalStateHandler.CurState.RecoverFromState();
				}
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
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
				lastRestTick = Find.TickManager.TicksGame;
				lastRestEffectiveness = restEffectiveness;
			}
		}
	}
}
