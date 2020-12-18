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
    public class JobGiver_FillAndroidBloodUrgent : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.IsAndroid())
            {
                return null;
            }
            var bloodLossHediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLossHediff != null && bloodLossHediff.Severity > 0.10f)
            {
                Log.Message("bloodLossHediff: " + bloodLossHediff.Severity);
                Thing neutroamine = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(SADefOf.Neutroamine),
                    PathEndMode.InteractionCell, TraverseParms.For(pawn, Danger.Some), 9999f, (Thing thing) => pawn.CanReserve(thing));
                if (neutroamine != null)
                {
                    int amount = (int)(bloodLossHediff.severityInt * 10f);
                    if (amount > neutroamine.stackCount)
                    {
                        amount = neutroamine.stackCount;
                    }
                    Job job = JobMaker.MakeJob(SADefOf.SA_ConsumeNeutroamine, neutroamine);
                    job.count = amount;
                    Log.Message(job + " - " + amount + " - " + bloodLossHediff.severityInt, true);
                    return job;
                }
            }
            return null;
        }
    }
}
