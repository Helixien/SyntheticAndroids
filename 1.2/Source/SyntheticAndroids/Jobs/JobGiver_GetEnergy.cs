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
                Log.Message(" - GetPriority - return 0f; - 3", true);
                return 0f;
            }
            if ((int)energyNeed.CurCategory < 3 && FoodUtility.ShouldBeFedBySomeone(pawn))
            {
                Log.Message(" - GetPriority - return 0f; - 5", true);
                return 0f;
            }
            if ((int)energyNeed.CurCategory < (int)minCategory)
            {
                Log.Message(" - GetPriority - return 0f; - 7", true);
                return 0f;
            }
            if (energyNeed.CurLevelPercentage > maxLevelPercentage)
            {
                Log.Message(" - GetPriority - return 0f; - 9", true);
                return 0f;
            }
            Log.Message(pawn + " - " + energyNeed.CurLevelPercentage);
            if (energyNeed.CurLevelPercentage < 0.25f)
            {
                Log.Message(" - GetPriority - return 9.5f; - 11", true);
                return 9.5f;
            }
            return 0f;
        }
        public override Job TryGiveJob(Pawn pawn)
        {
            Log.Message(" - TryGiveJob - Need_Energy energyNeed = pawn.needs.TryGetNeed<Need_Energy>(); ; - 13", true);
            Need_Energy energyNeed = pawn.needs.TryGetNeed<Need_Energy>(); ;
            Log.Message(" - TryGiveJob - if (energyNeed == null || (int)energyNeed.CurCategory < (int)minCategory || energyNeed.CurLevelPercentage > maxLevelPercentage) - 14", true);
            if (energyNeed == null || (int)energyNeed.CurCategory < (int)minCategory || energyNeed.CurLevelPercentage > maxLevelPercentage)
            {
                Log.Message(" - TryGiveJob - return null; - 15", true);
                return null;
            }
            bool desperate = false;
            Log.Message(" - TryGiveJob - if (!FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, desperate, out Thing foodSource, out ThingDef foodDef, canRefillDispenser: true, - 17", true);
            if (!SA_Utils.TryFindBestFoodSourceFor(pawn, pawn, desperate, out Thing foodSource, out ThingDef foodDef, canRefillDispenser: true,
                    canUseInventory: true, allowForbidden: false, false, allowSociallyImproper: false, pawn.IsWildMan(), forceScanWholeMap))
            {
                Log.Message(" - TryGiveJob - return null; - 18", true);
                return null;
            }
            Pawn pawn2 = foodSource as Pawn;
            Log.Message(" - TryGiveJob - if (pawn2 != null) - 20", true);
            if (pawn2 != null)
            {
                Log.Message(" - TryGiveJob - Job job = JobMaker.MakeJob(JobDefOf.PredatorHunt, pawn2); - 21", true);
                Job job = JobMaker.MakeJob(JobDefOf.PredatorHunt, pawn2);
                Log.Message(" - TryGiveJob - job.killIncappedTarget = true; - 22", true);
                job.killIncappedTarget = true;
                Log.Message(" - TryGiveJob - return job; - 23", true);
                return job;
            }
            Log.Message(" - TryGiveJob - if (foodSource is Plant && foodSource.def.plant.harvestedThingDef == foodDef) - 24", true);
            if (foodSource is Plant && foodSource.def.plant.harvestedThingDef == foodDef)
            {
                Log.Message(" - TryGiveJob - return JobMaker.MakeJob(JobDefOf.Harvest, foodSource); - 25", true);
                return JobMaker.MakeJob(JobDefOf.Harvest, foodSource);
            }
            Building_NutrientPasteDispenser building_NutrientPasteDispenser = foodSource as Building_NutrientPasteDispenser;
            Log.Message(" - TryGiveJob - if (building_NutrientPasteDispenser != null && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers()) - 27", true);
            if (building_NutrientPasteDispenser != null && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
            {
                Log.Message(" - TryGiveJob - Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn); - 28", true);
                Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
                Log.Message(" - TryGiveJob - if (building != null) - 29", true);
                if (building != null)
                {
                    Log.Message(" - TryGiveJob - ISlotGroupParent hopperSgp = building as ISlotGroupParent; - 30", true);
                    ISlotGroupParent hopperSgp = building as ISlotGroupParent;
                    Log.Message(" - TryGiveJob - Job job2 = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp); - 31", true);
                    Job job2 = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
                    Log.Message(" - TryGiveJob - if (job2 != null) - 32", true);
                    if (job2 != null)
                    {
                        Log.Message(" - TryGiveJob - return job2; - 33", true);
                        return job2;
                    }
                }
                foodSource = SA_Utils.BestFoodSourceOnMap(pawn, pawn, desperate, out foodDef, FoodPreferability.MealLavish, allowPlant: false, !pawn.IsTeetotaler(), allowCorpse: false, 
                    allowDispenserFull: false, allowDispenserEmpty: false, allowForbidden: false, allowSociallyImproper: false, 
                    allowHarvest: false, forceScanWholeMap);
                Log.Message(" - TryGiveJob - if (foodSource == null) - 35", true);
                if (foodSource == null)
                {
                    Log.Message(" - TryGiveJob - return null; - 36", true);
                    return null;
                }
            }
            float nutrition = FoodUtility.GetNutrition(foodSource, foodDef);
            Log.Message(" - TryGiveJob - Job job3 = JobMaker.MakeJob(SADefOf.SA_AndroidIngest, foodSource); - 38", true);
            Job job3 = JobMaker.MakeJob(SADefOf.SA_AndroidIngest, foodSource);
            Log.Message(" - TryGiveJob - job3.count = FoodUtility.WillIngestStackCountOf(pawn, foodDef, nutrition); - 39", true);
            job3.count = SA_Utils.WillIngestStackCountOf(pawn, foodDef, nutrition);
            Log.Message(" - TryGiveJob - return job3; - 40", true);
            return job3;
        }

	}
}
