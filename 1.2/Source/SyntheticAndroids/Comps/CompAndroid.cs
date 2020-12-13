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
	public class CompProperties_Android : CompProperties
	{
		public CompProperties_Android()
		{
			this.compClass = typeof(CompAndroid);
		}
	}

	public class CompAndroid : ThingComp
    {
		public CompProperties_Android Props => this.props as CompProperties_Android;

		public bool disabled;
		public bool downedState;
		public bool manhuntingState;
		public bool dazedState;
		public Pawn Android => this.parent as Pawn;
		public void MakeDazed()
        {
			if (this.Android.mindState.mentalStateHandler.TryStartMentalState(SADefOf.SA_Wander_DazedAndroid))
            {
				dazedState = true;
            }
		}
		public void EndDazedState()
        {
			var dazedMentalState = this.Android.MentalState as MentalState_Wander_DazedAndroid;
			if (dazedMentalState != null)
            {
				dazedMentalState.forceRecoverAfterTicks = 0;
            }
			this.dazedState = false;
		}

		public void MakeDowned()
		{
			downedState = true;
		}
		public void EndDownedState()
		{
			downedState = false;
		}

		public void MakeDisabled()
		{
			disabled = true;
		}
		public void TryMakeEnabled()
		{
			if (Android.health.hediffSet.GetNotMissingParts().Select(x => x.def).Contains(SADefOf.SA_PowerCell))
            {
				disabled = false;
            }
		}

		public void MakeManhunting()
		{
			if (this.Android.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent))
            {
				manhuntingState = true;
            }
		}
		public void EndManhunting()
		{
			var manhuntingMentalState = this.Android.MentalState as MentalState_Manhunter;
			if (manhuntingMentalState != null)
            {
				manhuntingMentalState.forceRecoverAfterTicks = 0;
            }
			this.manhuntingState = false;
		}

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
			SA_Utils.cachedAndroidComps[Android] = this;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
			Scribe_Values.Look(ref disabled, "disabled");
			Scribe_Values.Look(ref downedState, "downedState");
			Scribe_Values.Look(ref manhuntingState, "manhuntingState");
			Scribe_Values.Look(ref dazedState, "dazedState");
		}
    }
}