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
	public class CompProperties_NeutroamineConsumableAndroid : CompProperties
	{
		public CompProperties_NeutroamineConsumableAndroid()
		{
			this.compClass = typeof(CompNeutroamineConsumableAndroid);
		}
	}

	public class CompNeutroamineConsumableAndroid : ThingComp
    {
		public CompProperties_NeutroamineConsumableAndroid Props => this.props as CompProperties_NeutroamineConsumableAndroid;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
			if (!selPawn.IsAndroid())
            {
				yield break;
            }
			if (!ReachabilityUtility.CanReach(selPawn, this.parent, PathEndMode.ClosestTouch, Danger.Deadly, false, 0))
			{
				yield return new FloatMenuOption(Translator.Translate("CannotUseNoPath"), null, MenuOptionPriority.Default, null, null, 0f, null, null);
				yield break;
			}
			var bloodLossHediff = selPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
			if (bloodLossHediff == null || bloodLossHediff.severityInt < 0.01f)
            {
				yield return new FloatMenuOption(Translator.Translate("SA.CannotUseNeutroamineNoBloodloss"), null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			else
            {
				yield return new FloatMenuOption("SA.ConsumeNeutroamine".Translate(parent.LabelShort, parent), delegate
				{
					Job job = JobMaker.MakeJob(SADefOf.SA_ConsumeNeutroamine, parent);
					selPawn.jobs.TryTakeOrderedJob(job);
				});
			}
        }
    }
}