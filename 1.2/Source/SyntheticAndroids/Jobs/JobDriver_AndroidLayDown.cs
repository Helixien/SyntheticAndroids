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
    public class JobDriver_AndroidLayDown : JobDriver
    {
        public const TargetIndex BedOrRestSpotIndex = TargetIndex.A;

        public Building_Bed Bed => (Building_Bed)job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Log.Message("JobDriver_AndroidLayDown : JobDriver - TryMakePreToilReservations - if (job.GetTarget(TargetIndex.A).HasThing && !pawn.Reserve(Bed, job, Bed.SleepingSlotsCount, 0, null, errorOnFailed)) - 1", true);
            if (job.GetTarget(TargetIndex.A).HasThing && !pawn.Reserve(Bed, job, Bed.SleepingSlotsCount, 0, null, errorOnFailed))
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - TryMakePreToilReservations - return false; - 2", true);
                return false;
            }
            return true;
        }

        public override bool CanBeginNowWhileLyingDown()
        {
            Log.Message("JobDriver_AndroidLayDown : JobDriver - CanBeginNowWhileLyingDown - return JobInBedUtility.InBedOrRestSpotNow(pawn, job.GetTarget(TargetIndex.A)); - 4", true);
            return JobInBedUtility.InBedOrRestSpotNow(pawn, job.GetTarget(TargetIndex.A));
        }
        public override IEnumerable<Toil> MakeNewToils()
        {
            Log.Message("JobDriver_AndroidLayDown : JobDriver - MakeNewToils - bool hasBed = job.GetTarget(TargetIndex.A).HasThing; - 5", true);
            bool hasBed = job.GetTarget(TargetIndex.A).HasThing;
            Log.Message("JobDriver_AndroidLayDown : JobDriver - MakeNewToils - if (hasBed) - 6", true);
            if (hasBed)
            {
                yield return new Toil { initAction = delegate () { Log.Message("JobDriver_AndroidLayDown : JobDriver - MakeNewToils - yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.A); - 7", true); } };
                yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.A);
                yield return new Toil { initAction = delegate () { Log.Message("JobDriver_AndroidLayDown : JobDriver - MakeNewToils - yield return Toils_Bed.GotoBed(TargetIndex.A); - 8", true); } };
                yield return Toils_Bed.GotoBed(TargetIndex.A);
            }
            else
            {
                yield return new Toil { initAction = delegate () { Log.Message("JobDriver_AndroidLayDown : JobDriver - MakeNewToils - yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell); - 9", true); } };
                yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            }
            yield return LayDown(TargetIndex.A, hasBed, lookForOtherJobs: true);
        }

        public static Toil LayDown(TargetIndex bedOrRestSpotIndex, bool hasBed, bool lookForOtherJobs, bool canSleep = true, bool gainRestAndHealth = true)
        {
            Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - Toil layDown = new Toil(); - 11", true);
            Toil layDown = new Toil();
            layDown.initAction = delegate
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - Pawn actor3 = layDown.actor; - 12", true);
                Pawn actor3 = layDown.actor;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - actor3.pather.StopDead(); - 13", true);
                actor3.pather.StopDead();
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - JobDriver curDriver3 = actor3.jobs.curDriver; - 14", true);
                JobDriver curDriver3 = actor3.jobs.curDriver;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (hasBed) - 15", true);
                if (hasBed)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (!((Building_Bed)actor3.CurJob.GetTarget(bedOrRestSpotIndex).Thing).OccupiedRect().Contains(actor3.Position)) - 16", true);
                    if (!((Building_Bed)actor3.CurJob.GetTarget(bedOrRestSpotIndex).Thing).OccupiedRect().Contains(actor3.Position))
                    {
                        Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - Log.Error(\"Can't start LayDown toil because pawn is not in the bed. pawn=\" + actor3); - 17", true);
                        Log.Error("Can't start LayDown toil because pawn is not in the bed. pawn=" + actor3);
                        Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - actor3.jobs.EndCurrentJob(JobCondition.Errored); - 18", true);
                        actor3.jobs.EndCurrentJob(JobCondition.Errored);
                        Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - return; - 19", true);
                        return;
                    }
                    actor3.jobs.posture = PawnPosture.LayingInBed;
                }
                else
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - actor3.jobs.posture = PawnPosture.LayingOnGroundNormal; - 21", true);
                    actor3.jobs.posture = PawnPosture.LayingOnGroundNormal;
                }
                curDriver3.asleep = false;
                if (actor3.mindState.applyBedThoughtsTick == 0)
                {
                    actor3.mindState.applyBedThoughtsTick = Find.TickManager.TicksGame + Rand.Range(2500, 10000);
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - actor3.mindState.applyBedThoughtsOnLeave = false; - 25", true);
                    actor3.mindState.applyBedThoughtsOnLeave = false;
                }
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (actor3.ownership != null && actor3.CurrentBed() != actor3.ownership.OwnedBed) - 26", true);
                if (actor3.ownership != null && actor3.CurrentBed() != actor3.ownership.OwnedBed)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - ThoughtUtility.RemovePositiveBedroomThoughts(actor3); - 27", true);
                    ThoughtUtility.RemovePositiveBedroomThoughts(actor3);
                }
                actor3.GetComp<CompCanBeDormant>()?.ToSleep();
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - }; - 29", true);
            };
            layDown.tickAction = delegate
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - Pawn actor2 = layDown.actor; - 30", true);
                Pawn actor2 = layDown.actor;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - Job curJob = actor2.CurJob; - 31", true);
                Job curJob = actor2.CurJob;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - JobDriver curDriver2 = actor2.jobs.curDriver; - 32", true);
                JobDriver curDriver2 = actor2.jobs.curDriver;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - Building_Bed building_Bed = (Building_Bed)curJob.GetTarget(bedOrRestSpotIndex).Thing; - 33", true);
                Building_Bed building_Bed = (Building_Bed)curJob.GetTarget(bedOrRestSpotIndex).Thing;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - actor2.GainComfortFromCellIfPossible(); - 34", true);
                actor2.GainComfortFromCellIfPossible();
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (!curDriver2.asleep) - 35", true);
                if (!curDriver2.asleep)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (canSleep && ((actor2.needs.TryGetNeed<Need_Energy>() != null && actor2.needs.TryGetNeed<Need_Energy>().CurLevel < RestUtility.FallAsleepMaxLevel(actor2)) - 36", true);
                    if (canSleep && ((actor2.needs.TryGetNeed<Need_Energy>() != null && actor2.needs.TryGetNeed<Need_Energy>().CurLevel < RestUtility.FallAsleepMaxLevel(actor2))
                    || curJob.forceSleep))
                    {
                        Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - curDriver2.asleep = true; - 37", true);
                        curDriver2.asleep = true;
                    }
                }
                else if (!canSleep)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - curDriver2.asleep = false; - 39", true);
                    curDriver2.asleep = false;
                }
                else if ((actor2.needs.TryGetNeed<Need_Energy>() == null || actor2.needs.TryGetNeed<Need_Energy>().CurLevel >= RestUtility.WakeThreshold(actor2)) && !curJob.forceSleep)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - curDriver2.asleep = false; - 41", true);
                    curDriver2.asleep = false;
                }
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (curDriver2.asleep && gainRestAndHealth && actor2.needs.TryGetNeed<Need_Energy>() != null) - 42", true);
                if (curDriver2.asleep && gainRestAndHealth && actor2.needs.TryGetNeed<Need_Energy>() != null)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - float restEffectiveness = (building_Bed == null || !building_Bed.def.statBases.StatListContains(StatDefOf.BedRestEffectiveness)) ? StatDefOf.BedRestEffectiveness.valueIfMissing : building_Bed.GetStatValue(StatDefOf.BedRestEffectiveness); - 43", true);
                    float restEffectiveness = (building_Bed == null || !building_Bed.def.statBases.StatListContains(StatDefOf.BedRestEffectiveness)) ? StatDefOf.BedRestEffectiveness.valueIfMissing : building_Bed.GetStatValue(StatDefOf.BedRestEffectiveness);
                    actor2.needs.TryGetNeed<Need_Energy>().TickResting(restEffectiveness);
                }
                if (actor2.mindState.applyBedThoughtsTick != 0 && actor2.mindState.applyBedThoughtsTick <= Find.TickManager.TicksGame)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - ApplyBedThoughts(actor2); - 46", true);
                    ApplyBedThoughts(actor2);
                    actor2.mindState.applyBedThoughtsTick += 60000;
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - actor2.mindState.applyBedThoughtsOnLeave = true; - 48", true);
                    actor2.mindState.applyBedThoughtsOnLeave = true;
                }
                if (actor2.IsHashIntervalTick(100) && !actor2.Position.Fogged(actor2.Map))
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (curDriver2.asleep) - 50", true);
                    if (curDriver2.asleep)
                    {
                        Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - MoteMaker.ThrowMetaIcon(actor2.Position, actor2.Map, ThingDefOf.Mote_SleepZ); - 51", true);
                        MoteMaker.ThrowMetaIcon(actor2.Position, actor2.Map, ThingDefOf.Mote_SleepZ);
                    }
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (gainRestAndHealth && actor2.health.hediffSet.GetNaturallyHealingInjuredParts().Any()) - 52", true);
                    if (gainRestAndHealth && actor2.health.hediffSet.GetNaturallyHealingInjuredParts().Any())
                    {
                        Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - MoteMaker.ThrowMetaIcon(actor2.Position, actor2.Map, ThingDefOf.Mote_HealingCross); - 53", true);
                        MoteMaker.ThrowMetaIcon(actor2.Position, actor2.Map, ThingDefOf.Mote_HealingCross);
                    }
                }
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (actor2.ownership != null && building_Bed != null && !building_Bed.Medical && !building_Bed.OwnersForReading.Contains(actor2)) - 54", true);
                if (actor2.ownership != null && building_Bed != null && !building_Bed.Medical && !building_Bed.OwnersForReading.Contains(actor2))
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (actor2.Downed) - 55", true);
                    if (actor2.Downed)
                    {
                        Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - actor2.Position = CellFinder.RandomClosewalkCellNear(actor2.Position, actor2.Map, 1); - 56", true);
                        actor2.Position = CellFinder.RandomClosewalkCellNear(actor2.Position, actor2.Map, 1);
                    }
                    actor2.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                else if (lookForOtherJobs && actor2.IsHashIntervalTick(211))
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - actor2.jobs.CheckForJobOverride(); - 59", true);
                    actor2.jobs.CheckForJobOverride();
                }
            };
            Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - layDown.defaultCompleteMode = ToilCompleteMode.Never; - 61", true);
            layDown.defaultCompleteMode = ToilCompleteMode.Never;
            Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (hasBed) - 62", true);
            if (hasBed)
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - layDown.FailOnBedNoLongerUsable(bedOrRestSpotIndex); - 63", true);
                layDown.FailOnBedNoLongerUsable(bedOrRestSpotIndex);
            }
            layDown.AddFinishAction(delegate
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - Pawn actor = layDown.actor; - 64", true);
                Pawn actor = layDown.actor;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - JobDriver curDriver = actor.jobs.curDriver; - 65", true);
                JobDriver curDriver = actor.jobs.curDriver;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - if (actor.mindState.applyBedThoughtsOnLeave) - 66", true);
                if (actor.mindState.applyBedThoughtsOnLeave)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - ApplyBedThoughts(actor); - 67", true);
                    ApplyBedThoughts(actor);
                }
                curDriver.asleep = false;
                Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - }); - 69", true);
            });
            Log.Message("JobDriver_AndroidLayDown : JobDriver - LayDown - return layDown; - 70", true);
            return layDown;
        }

        private static void ApplyBedThoughts(Pawn actor)
        {
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (actor.needs.mood == null) - 71", true);
            if (actor.needs.mood == null)
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - return; - 72", true);
                return;
            }
            Building_Bed building_Bed = actor.CurrentBed();
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBedroom); - 74", true);
            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBedroom);
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBarracks); - 75", true);
            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInBarracks);
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOutside); - 76", true);
            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOutside);
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOnGround); - 77", true);
            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptOnGround);
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInCold); - 78", true);
            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInCold);
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInHeat); - 79", true);
            actor.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.SleptInHeat);
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (actor.GetRoom().PsychologicallyOutdoors) - 80", true);
            if (actor.GetRoom().PsychologicallyOutdoors)
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SleptOutside); - 81", true);
                actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SleptOutside);
            }
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (building_Bed == null || building_Bed.CostListAdjusted().Count == 0) - 82", true);
            if (building_Bed == null || building_Bed.CostListAdjusted().Count == 0)
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SleptOnGround); - 83", true);
                actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SleptOnGround);
            }
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (actor.AmbientTemperature < actor.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin)) - 84", true);
            if (actor.AmbientTemperature < actor.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin))
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SleptInCold); - 85", true);
                actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SleptInCold);
            }
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (actor.AmbientTemperature > actor.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax)) - 86", true);
            if (actor.AmbientTemperature > actor.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax))
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SleptInHeat); - 87", true);
                actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.SleptInHeat);
            }
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (building_Bed == null || building_Bed != actor.ownership.OwnedBed || building_Bed.ForPrisoners || actor.story.traits.HasTrait(TraitDefOf.Ascetic)) - 88", true);
            if (building_Bed == null || building_Bed != actor.ownership.OwnedBed || building_Bed.ForPrisoners || actor.story.traits.HasTrait(TraitDefOf.Ascetic))
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - return; - 89", true);
                return;
            }
            ThoughtDef thoughtDef = null;
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (building_Bed.GetRoom().Role == RoomRoleDefOf.Bedroom) - 91", true);
            if (building_Bed.GetRoom().Role == RoomRoleDefOf.Bedroom)
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - thoughtDef = ThoughtDefOf.SleptInBedroom; - 92", true);
                thoughtDef = ThoughtDefOf.SleptInBedroom;
            }
            else if (building_Bed.GetRoom().Role == RoomRoleDefOf.Barracks)
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - thoughtDef = ThoughtDefOf.SleptInBarracks; - 94", true);
                thoughtDef = ThoughtDefOf.SleptInBarracks;
            }
            Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (thoughtDef != null) - 95", true);
            if (thoughtDef != null)
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(building_Bed.GetRoom().GetStat(RoomStatDefOf.Impressiveness)); - 96", true);
                int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(building_Bed.GetRoom().GetStat(RoomStatDefOf.Impressiveness));
                Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - if (thoughtDef.stages[scoreStageIndex] != null) - 97", true);
                if (thoughtDef.stages[scoreStageIndex] != null)
                {
                    Log.Message("JobDriver_AndroidLayDown : JobDriver - ApplyBedThoughts - actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, scoreStageIndex)); - 98", true);
                    actor.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(thoughtDef, scoreStageIndex));
                }
            }
        }

        public override string GetReport()
        {
            Log.Message("JobDriver_AndroidLayDown : JobDriver - GetReport - if (asleep) - 99", true);
            if (asleep)
            {
                Log.Message("JobDriver_AndroidLayDown : JobDriver - GetReport - return \"ReportSleeping\".Translate(); - 100", true);
                return "ReportSleeping".Translate();
            }
            return "ReportResting".Translate();
        }
    }
}
