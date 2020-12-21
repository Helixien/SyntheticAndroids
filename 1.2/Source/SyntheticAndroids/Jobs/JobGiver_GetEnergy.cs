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
    public class JobGiver_GetEnergy : ThinkNode_JobGiver
    {
        private EnergyCategory minCategory;

        private float maxLevelPercentage = 1f;

        public bool forceScanWholeMap;
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_GetEnergy obj = (JobGiver_GetEnergy)base.DeepCopy(resolve);
            obj.minCategory = minCategory;
            obj.maxLevelPercentage = maxLevelPercentage;
            obj.forceScanWholeMap = forceScanWholeMap;
            return obj;
        }

        public override float GetPriority(Pawn pawn)
        {
            Need_Energy energyNeed = pawn.needs.TryGetNeed<Need_Energy>(); ;
            if (energyNeed == null)
            {
                return 0f;
            }
            if ((int)energyNeed.CurCategory < 3 && FoodUtility.ShouldBeFedBySomeone(pawn))
            {
                return 0f;
            }
            if ((int)energyNeed.CurCategory < (int)minCategory)
            {
                return 0f;
            }
            if (energyNeed.CurLevelPercentage > maxLevelPercentage)
            {
                return 0f;
            }
            if (energyNeed.CurLevelPercentage < 0.25f)
            {
                return 9.5f;
            }
            return 0f;
        }
        public override Job TryGiveJob(Pawn pawn)
        {
            Need_Energy energyNeed = pawn.needs.TryGetNeed<Need_Energy>(); ;
            if (energyNeed == null || (int)energyNeed.CurCategory < (int)minCategory || energyNeed.CurLevelPercentage > maxLevelPercentage)
            {
                return null;
            }
            bool desperate = false;
            if (!AndroidFoodUtility.TryFindBestFoodSourceFor(pawn, pawn, desperate, out Thing foodSource, out ThingDef foodDef, canRefillDispenser: true,
                    canUseInventory: true, allowForbidden: false, false, allowSociallyImproper: false, pawn.IsWildMan(), forceScanWholeMap))
            {
                return null;
            }
            Pawn pawn2 = foodSource as Pawn;
            if (pawn2 != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.PredatorHunt, pawn2);
                job.killIncappedTarget = true;
                return job;
            }
            if (foodSource is Plant && foodSource.def.plant.harvestedThingDef == foodDef)
            {
                return JobMaker.MakeJob(JobDefOf.Harvest, foodSource);
            }
            Building_NutrientPasteDispenser building_NutrientPasteDispenser = foodSource as Building_NutrientPasteDispenser;
            if (building_NutrientPasteDispenser != null && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
            {
                Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
                if (building != null)
                {
                    ISlotGroupParent hopperSgp = building as ISlotGroupParent;
                    Job job2 = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
                    if (job2 != null)
                    {
                        return job2;
                    }
                }
                foodSource = AndroidFoodUtility.BestFoodSourceOnMap(pawn, pawn, desperate, out foodDef, FoodPreferability.MealLavish, allowPlant: false, !pawn.IsTeetotaler(), allowCorpse: false,
                    allowDispenserFull: false, allowDispenserEmpty: false, allowForbidden: false, allowSociallyImproper: false,
                    allowHarvest: false, forceScanWholeMap);
                if (foodSource == null)
                {
                    return null;
                }
            }
            float nutrition = FoodUtility.GetNutrition(foodSource, foodDef);
            Job job3 = JobMaker.MakeJob(SADefOf.SA_AndroidIngest, foodSource);
            job3.count = AndroidFoodUtility.WillIngestStackCountOf(pawn, foodDef, nutrition);
            return job3;
        }

    }
}
