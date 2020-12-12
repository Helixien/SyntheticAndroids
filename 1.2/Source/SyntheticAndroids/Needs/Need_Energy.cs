using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

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
		}

		public override void NeedInterval()
		{
			if (!IsFrozen)
			{
				CurLevel -= EnergyNeedFallPerTick * 150f;
			}
			if (!EmptyEnergy)
			{
				lastChargingTick = Find.TickManager.TicksGame;
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
	}
}
