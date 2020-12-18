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
    public class JobDriver_ConsumeNeutroamine : JobDriver
    {
        private Thing IngestibleSource => job.GetTarget(TargetIndex.A).Thing;

        private bool eatingFromInventory;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref eatingFromInventory, "eatingFromInventory", defaultValue: false);
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();
            eatingFromInventory = (pawn.inventory != null && pawn.inventory.Contains(IngestibleSource));
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.Reserve(IngestibleSource, job, 1, job.count, null, errorOnFailed))
            {
                return false;
            }
            return true;
        }
        public override IEnumerable<Toil> MakeNewToils()
        {
            Toil chew = ChewIngestible(pawn, TargetIndex.A, TargetIndex.B)
                    .FailOn((Toil x) => !IngestibleSource.Spawned)
                    .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            foreach (Toil item in PrepareToIngestToils_ToolUser(chew))
            {
                yield return item;
            }
            yield return chew;
            yield return FinalizeIngest(pawn, TargetIndex.A);
        }
        public static Toil ChewIngestible(Pawn chewer, TargetIndex ingestibleInd, TargetIndex eatSurfaceInd = TargetIndex.None)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                Thing thing4 = actor.CurJob.GetTarget(ingestibleInd).Thing;
                toil.actor.pather.StopDead();
                actor.jobs.curDriver.ticksLeftThisToil = Mathf.RoundToInt(actor.CurJob.count * 500);
                if (thing4.Spawned)
                {
                    thing4.Map.physicalInteractionReservationManager.Reserve(chewer, actor.CurJob, thing4);
                }
            };
            toil.tickAction = delegate
            {
                if (chewer != toil.actor)
                {
                    toil.actor.rotationTracker.FaceCell(chewer.Position);
                }
                else
                {
                    Thing thing3 = toil.actor.CurJob.GetTarget(ingestibleInd).Thing;
                    if (thing3 != null && thing3.Spawned)
                    {
                        toil.actor.rotationTracker.FaceCell(thing3.Position);
                    }
                    else if (eatSurfaceInd != 0 && toil.actor.CurJob.GetTarget(eatSurfaceInd).IsValid)
                    {
                        toil.actor.rotationTracker.FaceCell(toil.actor.CurJob.GetTarget(eatSurfaceInd).Cell);
                    }
                }
                toil.actor.GainComfortFromCellIfPossible();
            };
            toil.WithProgressBar(ingestibleInd, delegate
            {
                Thing thing2 = toil.actor.CurJob.GetTarget(ingestibleInd).Thing;
                return (thing2 == null) ? 1f : (1f - (float)toil.actor.jobs.curDriver.ticksLeftThisToil / Mathf.Round((float)toil.actor.CurJob.count * 500));
            });
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.FailOnDestroyedOrNull(ingestibleInd);
            toil.AddFinishAction(delegate
            {
                if (chewer != null && chewer.CurJob != null)
                {
                    Thing thing = chewer.CurJob.GetTarget(ingestibleInd).Thing;
                    if (thing != null && chewer.Map.physicalInteractionReservationManager.IsReservedBy(chewer, thing))
                    {
                        chewer.Map.physicalInteractionReservationManager.Release(chewer, toil.actor.CurJob, thing);
                    }
                }
            });
            toil.handlingFacing = true;
            return toil;
        }

        public static Toil FinalizeIngest(Pawn ingester, TargetIndex ingestibleInd)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                Thing thing = curJob.GetTarget(ingestibleInd).Thing;
                HealthUtility.AdjustSeverity(actor, HediffDefOf.BloodLoss, curJob.count * -0.1f);
                thing.stackCount -= curJob.count;
                if (thing.stackCount <= 0)
                {
                    thing.Destroy();
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            return toil;
        }

        private IEnumerable<Toil> PrepareToIngestToils_ToolUser(Toil chewToil)
        {
            if (eatingFromInventory)
            {
                yield return Toils_Misc.TakeItemFromInventoryToCarrier(pawn, TargetIndex.A);
            }
            else
            {
                Toil gotoToPickup = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
                yield return Toils_Jump.JumpIf(gotoToPickup, () => pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation));
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
                yield return Toils_Jump.Jump(chewToil);
                yield return gotoToPickup;
            }
        }
        public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool behind, ref bool flip)
        {
            IntVec3 cell = job.GetTarget(TargetIndex.B).Cell;
            return ModifyCarriedThingDrawPosWorker(ref drawPos, ref behind, ref flip, cell, pawn);
        }

        public static bool ModifyCarriedThingDrawPosWorker(ref Vector3 drawPos, ref bool behind, ref bool flip, IntVec3 placeCell, Pawn pawn)
        {
            if (pawn.pather.Moving)
            {
                return false;
            }
            Thing carriedThing = pawn.carryTracker.CarriedThing;
            if (carriedThing == null || !carriedThing.IngestibleNow)
            {
                return false;
            }
            if (placeCell.IsValid && placeCell.AdjacentToCardinal(pawn.Position) && placeCell.HasEatSurface(pawn.Map) && carriedThing.def.ingestible.ingestHoldUsesTable)
            {
                drawPos = new Vector3((float)placeCell.x + 0.5f, drawPos.y, (float)placeCell.z + 0.5f);
                return true;
            }
            if (carriedThing.def.ingestible.ingestHoldOffsetStanding != null)
            {
                HoldOffset holdOffset = carriedThing.def.ingestible.ingestHoldOffsetStanding.Pick(pawn.Rotation);
                if (holdOffset != null)
                {
                    drawPos += holdOffset.offset;
                    behind = holdOffset.behind;
                    flip = holdOffset.flip;
                    return true;
                }
            }
            return false;
        }
    }
}