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
    public static class SA_Utils
    {
        public static bool IsAndroid(this Pawn pawn)
        {
            Log.Message("SA_Utils - IsAndroid - if (pawn.def == SADefOf.SA_Android) - 1", true);
            if (pawn.def == SADefOf.SA_Android)
            {
                Log.Message("SA_Utils - IsAndroid - return true; - 2", true);
                return true;
            }
            return false;
        }

        private static HashSet<Thing> filtered = new HashSet<Thing>();
        private static List<Pawn> tmpPredatorCandidates = new List<Pawn>();
        public static bool TryFindBestFoodSourceFor(Pawn getter, Pawn eater, bool desperate, out Thing foodSource, out ThingDef foodDef, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false, bool ignoreReservations = false, FoodPreferability minPrefOverride = FoodPreferability.Undefined)
        {
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - bool flag = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation); - 6", true);
            bool flag = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - bool allowDrug = !eater.IsTeetotaler(); - 7", true);
            bool allowDrug = !eater.IsTeetotaler();
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - Thing thing = null; - 8", true);
            Thing thing = null;
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (canUseInventory) - 9", true);
            if (canUseInventory)
            {
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (flag) - 10", true);
                if (flag)
                {
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - thing = FoodUtility.BestFoodInInventory(getter, eater, (minPrefOverride == FoodPreferability.Undefined) ? FoodPreferability.MealAwful : minPrefOverride); - 11", true);
                    thing = FoodUtility.BestFoodInInventory(getter, eater, (minPrefOverride == FoodPreferability.Undefined) ? FoodPreferability.MealAwful : minPrefOverride);
                }
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (thing != null) - 12", true);
                if (thing != null)
                {
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (getter.Faction != Faction.OfPlayer) - 13", true);
                    if (getter.Faction != Faction.OfPlayer)
                    {
                        Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodSource = thing; - 14", true);
                        foodSource = thing;
                        Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = FoodUtility.GetFinalIngestibleDef(foodSource); - 15", true);
                        foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                        Log.Message("SA_Utils - TryFindBestFoodSourceFor - return true; - 16", true);
                        return true;
                    }
                    CompRottable compRottable = thing.TryGetComp<CompRottable>();
                    if (compRottable != null && compRottable.Stage == RotStage.Fresh && compRottable.TicksUntilRotAtCurrentTemp < 30000)
                    {
                        Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodSource = thing; - 19", true);
                        foodSource = thing;
                        Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = FoodUtility.GetFinalIngestibleDef(foodSource); - 20", true);
                        foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                        Log.Message("SA_Utils - TryFindBestFoodSourceFor - return true; - 21", true);
                        return true;
                    }
                }
            }
            bool allowPlant = getter == eater;
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - bool allowForbidden2 = allowForbidden; - 23", true);
            bool allowForbidden2 = allowForbidden;
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - ThingDef foodDef2; - 24", true);
            ThingDef foodDef2;
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - Thing thing2 = BestFoodSourceOnMap(getter, eater, desperate, out foodDef2, FoodPreferability.MealLavish, allowPlant, allowDrug, allowCorpse, allowDispenserFull: true, canRefillDispenser, allowForbidden2, allowSociallyImproper, allowHarvest, forceScanWholeMap, ignoreReservations, minPrefOverride); - 25", true);
            Thing thing2 = BestFoodSourceOnMap(getter, eater, desperate, out foodDef2, FoodPreferability.MealLavish, allowPlant, allowDrug, allowCorpse, allowDispenserFull: true, canRefillDispenser, allowForbidden2, allowSociallyImproper, allowHarvest, forceScanWholeMap, ignoreReservations, minPrefOverride);
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (thing != null || thing2 != null) - 26", true);
            if (thing != null || thing2 != null)
            {
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (thing == null && thing2 != null) - 27", true);
                if (thing == null && thing2 != null)
                {
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodSource = thing2; - 28", true);
                    foodSource = thing2;
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = foodDef2; - 29", true);
                    foodDef = foodDef2;
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - return true; - 30", true);
                    return true;
                }
                ThingDef finalIngestibleDef = FoodUtility.GetFinalIngestibleDef(thing);
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (thing2 == null) - 32", true);
                if (thing2 == null)
                {
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodSource = thing; - 33", true);
                    foodSource = thing;
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = finalIngestibleDef; - 34", true);
                    foodDef = finalIngestibleDef;
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - return true; - 35", true);
                    return true;
                }
                float num = FoodUtility.FoodOptimality(eater, thing2, foodDef2, (getter.Position - thing2.Position).LengthManhattan);
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - float num2 = FoodUtility.FoodOptimality(eater, thing, finalIngestibleDef, 0f); - 37", true);
                float num2 = FoodUtility.FoodOptimality(eater, thing, finalIngestibleDef, 0f);
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - num2 -= 32f; - 38", true);
                num2 -= 32f;
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (num > num2) - 39", true);
                if (num > num2)
                {
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodSource = thing2; - 40", true);
                    foodSource = thing2;
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = foodDef2; - 41", true);
                    foodDef = foodDef2;
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - return true; - 42", true);
                    return true;
                }
                foodSource = thing;
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = FoodUtility.GetFinalIngestibleDef(foodSource); - 44", true);
                foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - return true; - 45", true);
                return true;
            }
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (canUseInventory && flag) - 46", true);
            if (canUseInventory && flag)
            {
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - thing = FoodUtility.BestFoodInInventory(getter, eater, FoodPreferability.DesperateOnly, FoodPreferability.MealLavish, 0f, allowDrug); - 47", true);
                thing = FoodUtility.BestFoodInInventory(getter, eater, FoodPreferability.DesperateOnly, FoodPreferability.MealLavish, 0f, allowDrug);
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (thing != null) - 48", true);
                if (thing != null)
                {
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodSource = thing; - 49", true);
                    foodSource = thing;
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = FoodUtility.GetFinalIngestibleDef(foodSource); - 50", true);
                    foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - return true; - 51", true);
                    return true;
                }
            }
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (thing2 == null && getter == eater && (getter.RaceProps.predator || (getter.IsWildMan() && !getter.IsPrisoner && !getter.WorkTypeIsDisabled(WorkTypeDefOf.Hunting)))) - 52", true);
            if (thing2 == null && getter == eater && (getter.RaceProps.predator || (getter.IsWildMan() && !getter.IsPrisoner && !getter.WorkTypeIsDisabled(WorkTypeDefOf.Hunting))))
            {
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - Pawn pawn = BestPawnToHuntForPredator(getter, forceScanWholeMap); - 53", true);
                Pawn pawn = BestPawnToHuntForPredator(getter, forceScanWholeMap);
                Log.Message("SA_Utils - TryFindBestFoodSourceFor - if (pawn != null) - 54", true);
                if (pawn != null)
                {
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodSource = pawn; - 55", true);
                    foodSource = pawn;
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = FoodUtility.GetFinalIngestibleDef(foodSource); - 56", true);
                    foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                    Log.Message("SA_Utils - TryFindBestFoodSourceFor - return true; - 57", true);
                    return true;
                }
            }
            foodSource = null;
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - foodDef = null; - 59", true);
            foodDef = null;
            Log.Message("SA_Utils - TryFindBestFoodSourceFor - return false; - 60", true);
            return false;
        }

        public static bool WillEat(this Pawn p, Thing food, Pawn getter = null, bool careIfNotAcceptableForTitle = true)
        {
            if (!p.RaceProps.CanEverEat(food))
            {
                return false;
            }
            if (p.foodRestriction != null)
            {
                FoodRestriction currentRespectedRestriction = p.foodRestriction.GetCurrentRespectedRestriction(getter);
                if (currentRespectedRestriction != null && !currentRespectedRestriction.Allows(food) && (food.def.IsWithinCategory(ThingCategoryDefOf.Foods) || food.def.IsWithinCategory(ThingCategoryDefOf.Corpses)))
                {
                    return false;
                }
            }
            if (careIfNotAcceptableForTitle && InappropriateForTitle(food.def, p, allowIfStarving: true))
            {
                return false;
            }
            return true;
        }

        public static bool WillEat(this Pawn p, ThingDef food, Pawn getter = null, bool careIfNotAcceptableForTitle = true)
        {
            if (!p.RaceProps.CanEverEat(food))
            {
                return false;
            }
            if (p.foodRestriction != null)
            {
                FoodRestriction currentRespectedRestriction = p.foodRestriction.GetCurrentRespectedRestriction(getter);
                if (currentRespectedRestriction != null && !currentRespectedRestriction.Allows(food) && food.IsWithinCategory(currentRespectedRestriction.filter.DisplayRootCategory.catDef))
                {
                    return false;
                }
            }
            if (careIfNotAcceptableForTitle && InappropriateForTitle(food, p, allowIfStarving: true))
            {
                return false;
            }
            return true;
        }
        public static bool InappropriateForTitle(ThingDef food, Pawn p, bool allowIfStarving)
        {
            if ((allowIfStarving && p.needs.TryGetNeed<Need_Energy>().EmptyEnergy) || (p.story != null && p.story.traits.HasTrait(TraitDefOf.Ascetic)) || p.IsPrisoner || (food.ingestible.joyKind != null && food.ingestible.joy > 0f))
            {
                return false;
            }
            RoyalTitle royalTitle = p.royalty?.MostSeniorTitle;
            if (royalTitle != null && royalTitle.conceited && royalTitle.def.foodRequirement.Defined)
            {
                return !royalTitle.def.foodRequirement.Acceptable(food);
            }
            return false;
        }

        private static Pawn BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
        {
            Log.Message("SA_Utils - BestPawnToHuntForPredator - if (predator.meleeVerbs.TryGetMeleeVerb(null) == null) - 61", true);
            if (predator.meleeVerbs.TryGetMeleeVerb(null) == null)
            {
                Log.Message("SA_Utils - BestPawnToHuntForPredator - return null; - 62", true);
                return null;
            }
            bool flag = false;
            Log.Message("SA_Utils - BestPawnToHuntForPredator - if (predator.health.summaryHealth.SummaryHealthPercent < 0.25f) - 64", true);
            if (predator.health.summaryHealth.SummaryHealthPercent < 0.25f)
            {
                Log.Message("SA_Utils - BestPawnToHuntForPredator - flag = true; - 65", true);
                flag = true;
            }
            tmpPredatorCandidates.Clear();
            Log.Message("SA_Utils - BestPawnToHuntForPredator - if (GetMaxRegionsToScan(predator, forceScanWholeMap) < 0) - 67", true);
            if (GetMaxRegionsToScan(predator, forceScanWholeMap) < 0)
            {
                Log.Message("SA_Utils - BestPawnToHuntForPredator - tmpPredatorCandidates.AddRange(predator.Map.mapPawns.AllPawnsSpawned); - 68", true);
                tmpPredatorCandidates.AddRange(predator.Map.mapPawns.AllPawnsSpawned);
            }
            else
            {
                Log.Message("SA_Utils - BestPawnToHuntForPredator - TraverseParms traverseParms = TraverseParms.For(predator); - 69", true);
                TraverseParms traverseParms = TraverseParms.For(predator);
                RegionTraverser.BreadthFirstTraverse(predator.Position, predator.Map, (Region from, Region to) => to.Allows(traverseParms, isDestination: true), delegate (Region x)
                {
                    Log.Message("SA_Utils - BestPawnToHuntForPredator - List<Thing> list = x.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn); - 70", true);
                    List<Thing> list = x.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
                    for (int j = 0; j < list.Count; j++)
                    {
                        Log.Message("SA_Utils - BestPawnToHuntForPredator - tmpPredatorCandidates.Add((Pawn)list[j]); - 71", true);
                        tmpPredatorCandidates.Add((Pawn)list[j]);
                    }
                    return false;
                    Log.Message("SA_Utils - BestPawnToHuntForPredator - }); - 73", true);
                });
            }
            Pawn pawn = null;
            Log.Message("SA_Utils - BestPawnToHuntForPredator - float num = 0f; - 75", true);
            float num = 0f;
            Log.Message("SA_Utils - BestPawnToHuntForPredator - bool tutorialMode = TutorSystem.TutorialMode; - 76", true);
            bool tutorialMode = TutorSystem.TutorialMode;
            for (int i = 0; i < tmpPredatorCandidates.Count; i++)
            {
                Log.Message("SA_Utils - BestPawnToHuntForPredator - Pawn pawn2 = tmpPredatorCandidates[i]; - 77", true);
                Pawn pawn2 = tmpPredatorCandidates[i];
                Log.Message("SA_Utils - BestPawnToHuntForPredator - if (predator.GetRoom() == pawn2.GetRoom() && predator != pawn2 && (!flag || pawn2.Downed) && FoodUtility.IsAcceptablePreyFor(predator, pawn2) && predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly) && !pawn2.IsForbidden(predator) && (!tutorialMode || pawn2.Faction != Faction.OfPlayer)) - 78", true);
                if (predator.GetRoom() == pawn2.GetRoom() && predator != pawn2 && (!flag || pawn2.Downed) && FoodUtility.IsAcceptablePreyFor(predator, pawn2) && predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly) && !pawn2.IsForbidden(predator) && (!tutorialMode || pawn2.Faction != Faction.OfPlayer))
                {
                    Log.Message("SA_Utils - BestPawnToHuntForPredator - float preyScoreFor = FoodUtility.GetPreyScoreFor(predator, pawn2); - 79", true);
                    float preyScoreFor = FoodUtility.GetPreyScoreFor(predator, pawn2);
                    Log.Message("SA_Utils - BestPawnToHuntForPredator - if (preyScoreFor > num || pawn == null) - 80", true);
                    if (preyScoreFor > num || pawn == null)
                    {
                        Log.Message("SA_Utils - BestPawnToHuntForPredator - num = preyScoreFor; - 81", true);
                        num = preyScoreFor;
                        Log.Message("SA_Utils - BestPawnToHuntForPredator - pawn = pawn2; - 82", true);
                        pawn = pawn2;
                    }
                }
            }
            tmpPredatorCandidates.Clear();
            Log.Message("SA_Utils - BestPawnToHuntForPredator - return pawn; - 84", true);
            return pawn;
        }
        public static Thing BestFoodSourceOnMap(Pawn getter, Pawn eater, bool desperate, out ThingDef foodDef, FoodPreferability maxPref = FoodPreferability.MealLavish, bool allowPlant = true, bool allowDrug = true, bool allowCorpse = true, bool allowDispenserFull = true, bool allowDispenserEmpty = true, bool allowForbidden = false, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false, bool ignoreReservations = false, FoodPreferability minPrefOverride = FoodPreferability.Undefined)
        {
            Log.Message("SA_Utils - BestFoodSourceOnMap - foodDef = null; - 85", true);
            foodDef = null;
            Log.Message("SA_Utils - BestFoodSourceOnMap - bool getterCanManipulate = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation); - 86", true);
            bool getterCanManipulate = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            Log.Message("SA_Utils - BestFoodSourceOnMap - if (!getterCanManipulate && getter != eater) - 87", true);
            if (!getterCanManipulate && getter != eater)
            {
                Log.Message("SA_Utils - BestFoodSourceOnMap - Log.Error(string.Concat(getter, \" tried to find food to bring to \", eater, \" but \", getter, \" is incapable of Manipulation.\")); - 88", true);
                Log.Error(string.Concat(getter, " tried to find food to bring to ", eater, " but ", getter, " is incapable of Manipulation."));
                Log.Message("SA_Utils - BestFoodSourceOnMap - return null; - 89", true);
                return null;
            }
            FoodPreferability minPref;
            Log.Message("SA_Utils - BestFoodSourceOnMap - if (minPrefOverride == FoodPreferability.Undefined) - 91", true);
            if (minPrefOverride == FoodPreferability.Undefined)
            {
                Log.Message("SA_Utils - BestFoodSourceOnMap - if (eater.NonHumanlikeOrWildMan()) - 92", true);
                if (eater.NonHumanlikeOrWildMan())
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - minPref = FoodPreferability.NeverForNutrition; - 93", true);
                    minPref = FoodPreferability.NeverForNutrition;
                }
                else if (desperate)
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - minPref = FoodPreferability.DesperateOnly; - 95", true);
                    minPref = FoodPreferability.DesperateOnly;
                }
                else
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - minPref = (((int)eater.needs.TryGetNeed<Need_Energy>().CurCategory >= 2) ? FoodPreferability.RawBad : FoodPreferability.MealAwful); - 96", true);
                    minPref = (((int)eater.needs.TryGetNeed<Need_Energy>().CurCategory >= 2) ? FoodPreferability.RawBad : FoodPreferability.MealAwful);
                }
            }
            else
            {
                Log.Message("SA_Utils - BestFoodSourceOnMap - minPref = minPrefOverride; - 97", true);
                minPref = minPrefOverride;
            }
            Predicate<Thing> foodValidator = delegate (Thing t)
            {
                Log.Message("SA_Utils - BestFoodSourceOnMap - Building_NutrientPasteDispenser building_NutrientPasteDispenser = t as Building_NutrientPasteDispenser; - 98", true);
                Building_NutrientPasteDispenser building_NutrientPasteDispenser = t as Building_NutrientPasteDispenser;
                Log.Message("SA_Utils - BestFoodSourceOnMap - if (building_NutrientPasteDispenser != null) - 99", true);
                if (building_NutrientPasteDispenser != null)
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - if (!allowDispenserFull || !getterCanManipulate || (int)ThingDefOf.MealNutrientPaste.ingestible.preferability < (int)minPref || (int)ThingDefOf.MealNutrientPaste.ingestible.preferability > (int)maxPref || !eater.WillEat(ThingDefOf.MealNutrientPaste, getter) || (t.Faction != getter.Faction && t.Faction != getter.HostFaction) || (!allowForbidden && t.IsForbidden(getter)) || !building_NutrientPasteDispenser.powerComp.PowerOn || (!allowDispenserEmpty && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers()) || !t.InteractionCell.Standable(t.Map) || !IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) || !getter.Map.reachability.CanReachNonLocal(getter.Position, new TargetInfo(t.InteractionCell, t.Map), PathEndMode.OnCell, TraverseParms.For(getter, Danger.Some))) - 100", true);
                    if (!allowDispenserFull || !getterCanManipulate || (int)ThingDefOf.MealNutrientPaste.ingestible.preferability < (int)minPref || (int)ThingDefOf.MealNutrientPaste.ingestible.preferability > (int)maxPref || !eater.WillEat(ThingDefOf.MealNutrientPaste, getter) || (t.Faction != getter.Faction && t.Faction != getter.HostFaction) || (!allowForbidden && t.IsForbidden(getter)) || !building_NutrientPasteDispenser.powerComp.PowerOn || (!allowDispenserEmpty && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers()) || !t.InteractionCell.Standable(t.Map) || !IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) || !getter.Map.reachability.CanReachNonLocal(getter.Position, new TargetInfo(t.InteractionCell, t.Map), PathEndMode.OnCell, TraverseParms.For(getter, Danger.Some)))
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 101", true);
                        return false;
                    }
                }
                else
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - int stackCount = 1; - 102", true);
                    int stackCount = 1;
                    Log.Message("SA_Utils - BestFoodSourceOnMap - if (FoodUtility.bestFoodSourceOnMap_minNutrition_NewTemp.HasValue) - 103", true);
                    if (FoodUtility.bestFoodSourceOnMap_minNutrition_NewTemp.HasValue)
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - float statValue = t.GetStatValue(StatDefOf.Nutrition); - 104", true);
                        float statValue = t.GetStatValue(StatDefOf.Nutrition);
                        Log.Message("SA_Utils - BestFoodSourceOnMap - stackCount = FoodUtility.StackCountForNutrition(FoodUtility.bestFoodSourceOnMap_minNutrition_NewTemp.Value, statValue); - 105", true);
                        stackCount = FoodUtility.StackCountForNutrition(FoodUtility.bestFoodSourceOnMap_minNutrition_NewTemp.Value, statValue);
                    }
                    Log.Message("SA_Utils - BestFoodSourceOnMap - if ((int)t.def.ingestible.preferability < (int)minPref || (int)t.def.ingestible.preferability > (int)maxPref || !eater.WillEat(t, getter) || !t.def.IsNutritionGivingIngestible || !t.IngestibleNow || (!allowCorpse && t is Corpse) || (!allowDrug && t.def.IsDrug) || (!allowForbidden && t.IsForbidden(getter)) || (!desperate && t.IsNotFresh()) || t.IsDessicated() || !IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) || (!getter.AnimalAwareOf(t) && !forceScanWholeMap) || (!ignoreReservations && !getter.CanReserve(t, 10, stackCount))) - 106", true);
                    if ((int)t.def.ingestible.preferability < (int)minPref || (int)t.def.ingestible.preferability > (int)maxPref || !eater.WillEat(t, getter) || !t.def.IsNutritionGivingIngestible || !t.IngestibleNow || (!allowCorpse && t is Corpse) || (!allowDrug && t.def.IsDrug) || (!allowForbidden && t.IsForbidden(getter)) || (!desperate && t.IsNotFresh()) || t.IsDessicated() || !IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) || (!getter.AnimalAwareOf(t) && !forceScanWholeMap) || (!ignoreReservations && !getter.CanReserve(t, 10, stackCount)))
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 107", true);
                        return false;
                    }
                }
                return true;
                Log.Message("SA_Utils - BestFoodSourceOnMap - }; - 109", true);
            };
            Log.Message("SA_Utils - BestFoodSourceOnMap - ThingRequest thingRequest = (!((eater.RaceProps.foodType & (FoodTypeFlags.Plant | FoodTypeFlags.Tree)) != 0 && allowPlant)) ? ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree) : ThingRequest.ForGroup(ThingRequestGroup.FoodSource); - 110", true);
            ThingRequest thingRequest = (!((eater.RaceProps.foodType & (FoodTypeFlags.Plant | FoodTypeFlags.Tree)) != 0 && allowPlant)) ? ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree) : ThingRequest.ForGroup(ThingRequestGroup.FoodSource);
            Log.Message("SA_Utils - BestFoodSourceOnMap - Thing bestThing; - 111", true);
            Thing bestThing;
            Log.Message("SA_Utils - BestFoodSourceOnMap - if (getter.RaceProps.Humanlike) - 112", true);
            if (getter.RaceProps.Humanlike)
            {
                Log.Message("SA_Utils - BestFoodSourceOnMap - bestThing = SpawnedFoodSearchInnerScan(eater, getter.Position, getter.Map.listerThings.ThingsMatching(thingRequest), PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, foodValidator); - 113", true);
                bestThing = SpawnedFoodSearchInnerScan(eater, getter.Position, getter.Map.listerThings.ThingsMatching(thingRequest), PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, foodValidator);
                Log.Message("SA_Utils - BestFoodSourceOnMap - if (allowHarvest && getterCanManipulate) - 114", true);
                if (allowHarvest && getterCanManipulate)
                {
                    Thing thing = GenClosest.ClosestThingReachable(searchRegionsMax: (!forceScanWholeMap || bestThing != null) ? 30 : (-1), root: getter.Position, map: getter.Map, thingReq: ThingRequest.ForGroup(ThingRequestGroup.HarvestablePlant), peMode: PathEndMode.Touch, traverseParams: TraverseParms.For(getter), maxDistance: 9999f, validator: delegate (Thing x)
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - Plant plant = (Plant)x; - 115", true);
                        Plant plant = (Plant)x;
                        Log.Message("SA_Utils - BestFoodSourceOnMap - if (!plant.HarvestableNow) - 116", true);
                        if (!plant.HarvestableNow)
                        {
                            Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 117", true);
                            return false;
                        }
                        ThingDef harvestedThingDef = plant.def.plant.harvestedThingDef;
                        Log.Message("SA_Utils - BestFoodSourceOnMap - if (!harvestedThingDef.IsNutritionGivingIngestible) - 119", true);
                        if (!harvestedThingDef.IsNutritionGivingIngestible)
                        {
                            Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 120", true);
                            return false;
                        }
                        Log.Message("SA_Utils - BestFoodSourceOnMap - if (!eater.WillEat(harvestedThingDef, getter)) - 121", true);
                        if (!eater.WillEat(harvestedThingDef, getter))
                        {
                            Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 122", true);
                            return false;
                        }
                        Log.Message("SA_Utils - BestFoodSourceOnMap - if (!getter.CanReserve(plant)) - 123", true);
                        if (!getter.CanReserve(plant))
                        {
                            Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 124", true);
                            return false;
                        }
                        Log.Message("SA_Utils - BestFoodSourceOnMap - if (!allowForbidden && plant.IsForbidden(getter)) - 125", true);
                        if (!allowForbidden && plant.IsForbidden(getter))
                        {
                            Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 126", true);
                            return false;
                        }
                        return (bestThing == null || (int)FoodUtility.GetFinalIngestibleDef(bestThing).ingestible.preferability < (int)harvestedThingDef.ingestible.preferability) ? true : false;
                        Log.Message("SA_Utils - BestFoodSourceOnMap - }); - 128", true);
                    });
                    Log.Message("SA_Utils - BestFoodSourceOnMap - if (thing != null) - 129", true);
                    if (thing != null)
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - bestThing = thing; - 130", true);
                        bestThing = thing;
                        Log.Message("SA_Utils - BestFoodSourceOnMap - foodDef = FoodUtility.GetFinalIngestibleDef(thing, harvest: true); - 131", true);
                        foodDef = FoodUtility.GetFinalIngestibleDef(thing, harvest: true);
                    }
                }
                Log.Message("SA_Utils - BestFoodSourceOnMap - if (foodDef == null && bestThing != null) - 132", true);
                if (foodDef == null && bestThing != null)
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - foodDef = FoodUtility.GetFinalIngestibleDef(bestThing); - 133", true);
                    foodDef = FoodUtility.GetFinalIngestibleDef(bestThing);
                }
            }
            else
            {
                Log.Message("SA_Utils - BestFoodSourceOnMap - int maxRegionsToScan = GetMaxRegionsToScan(getter, forceScanWholeMap); - 134", true);
                int maxRegionsToScan = GetMaxRegionsToScan(getter, forceScanWholeMap);
                Log.Message("SA_Utils - BestFoodSourceOnMap - filtered.Clear(); - 135", true);
                filtered.Clear();
                Log.Message("SA_Utils - BestFoodSourceOnMap - foreach (Thing item in GenRadial.RadialDistinctThingsAround(getter.Position, getter.Map, 2f, useCenter: true)) - 136", true);
                foreach (Thing item in GenRadial.RadialDistinctThingsAround(getter.Position, getter.Map, 2f, useCenter: true))
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - Pawn pawn = item as Pawn; - 137", true);
                    Pawn pawn = item as Pawn;
                    Log.Message("SA_Utils - BestFoodSourceOnMap - if (pawn != null && pawn != getter && pawn.RaceProps.Animal && pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Ingest && pawn.CurJob.GetTarget(TargetIndex.A).HasThing) - 138", true);
                    if (pawn != null && pawn != getter && pawn.RaceProps.Animal && pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Ingest && pawn.CurJob.GetTarget(TargetIndex.A).HasThing)
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - filtered.Add(pawn.CurJob.GetTarget(TargetIndex.A).Thing); - 139", true);
                        filtered.Add(pawn.CurJob.GetTarget(TargetIndex.A).Thing);
                    }
                }
                bool ignoreEntirelyForbiddenRegions = !allowForbidden && ForbidUtility.CaresAboutForbidden(getter, cellTarget: true) && getter.playerSettings != null && getter.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap != null;
                Predicate<Thing> validator = delegate (Thing t)
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - if (!foodValidator(t)) - 141", true);
                    if (!foodValidator(t))
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 142", true);
                        return false;
                    }
                    Log.Message("SA_Utils - BestFoodSourceOnMap - if (filtered.Contains(t)) - 143", true);
                    if (filtered.Contains(t))
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 144", true);
                        return false;
                    }
                    Log.Message("SA_Utils - BestFoodSourceOnMap - if (!(t is Building_NutrientPasteDispenser) && (int)t.def.ingestible.preferability <= 2) - 145", true);
                    if (!(t is Building_NutrientPasteDispenser) && (int)t.def.ingestible.preferability <= 2)
                    {
                        Log.Message("SA_Utils - BestFoodSourceOnMap - return false; - 146", true);
                        return false;
                    }
                    return (!t.IsNotFresh()) ? true : false;
                    Log.Message("SA_Utils - BestFoodSourceOnMap - }; - 148", true);
                };
                Log.Message("SA_Utils - BestFoodSourceOnMap - bestThing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, validator, null, 0, maxRegionsToScan, forceAllowGlobalSearch: false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions); - 149", true);
                bestThing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, validator, null, 0, maxRegionsToScan, forceAllowGlobalSearch: false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions);
                Log.Message("SA_Utils - BestFoodSourceOnMap - filtered.Clear(); - 150", true);
                filtered.Clear();
                Log.Message("SA_Utils - BestFoodSourceOnMap - if (bestThing == null) - 151", true);
                if (bestThing == null)
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - desperate = true; - 152", true);
                    desperate = true;
                    Log.Message("SA_Utils - BestFoodSourceOnMap - bestThing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, foodValidator, null, 0, maxRegionsToScan, forceAllowGlobalSearch: false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions); - 153", true);
                    bestThing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, foodValidator, null, 0, maxRegionsToScan, forceAllowGlobalSearch: false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions);
                }
                Log.Message("SA_Utils - BestFoodSourceOnMap - if (bestThing != null) - 154", true);
                if (bestThing != null)
                {
                    Log.Message("SA_Utils - BestFoodSourceOnMap - foodDef = FoodUtility.GetFinalIngestibleDef(bestThing); - 155", true);
                    foodDef = FoodUtility.GetFinalIngestibleDef(bestThing);
                }
            }
            return bestThing;
        }

        public static int WillIngestStackCountOf(Pawn ingester, ThingDef def, float singleFoodNutrition)
        {
            int num = Mathf.Min(def.ingestible.maxNumToIngestAtOnce, FoodUtility.StackCountForNutrition(ingester.needs.TryGetNeed<Need_Energy>().EnergyWanted, singleFoodNutrition));
            if (num < 1)
            {
                num = 1;
            }
            return num;
        }
        private static bool IsFoodSourceOnMapSociallyProper(Thing t, Pawn getter, Pawn eater, bool allowSociallyImproper)
        {
            Log.Message("SA_Utils - IsFoodSourceOnMapSociallyProper - if (!allowSociallyImproper) - 157", true);
            if (!allowSociallyImproper)
            {
                Log.Message("SA_Utils - IsFoodSourceOnMapSociallyProper - bool animalsCare = !getter.RaceProps.Animal; - 158", true);
                bool animalsCare = !getter.RaceProps.Animal;
                Log.Message("SA_Utils - IsFoodSourceOnMapSociallyProper - if (!t.IsSociallyProper(getter) && !t.IsSociallyProper(eater, eater.IsPrisonerOfColony, animalsCare)) - 159", true);
                if (!t.IsSociallyProper(getter) && !t.IsSociallyProper(eater, eater.IsPrisonerOfColony, animalsCare))
                {
                    Log.Message("SA_Utils - IsFoodSourceOnMapSociallyProper - return false; - 160", true);
                    return false;
                }
            }
            return true;
        }

        private static Thing SpawnedFoodSearchInnerScan(Pawn eater, IntVec3 root, List<Thing> searchSet, PathEndMode peMode, TraverseParms traverseParams, float maxDistance = 9999f, Predicate<Thing> validator = null)
        {
            Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - if (searchSet == null) - 162", true);
            if (searchSet == null)
            {
                Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - return null; - 163", true);
                return null;
            }
            Pawn pawn = traverseParams.pawn ?? eater;
            Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - int num = 0; - 165", true);
            int num = 0;
            Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - int num2 = 0; - 166", true);
            int num2 = 0;
            Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - Thing result = null; - 167", true);
            Thing result = null;
            Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - float num3 = 0f; - 168", true);
            float num3 = 0f;
            Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - float num4 = float.MinValue; - 169", true);
            float num4 = float.MinValue;
            for (int i = 0; i < searchSet.Count; i++)
            {
                Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - Thing thing = searchSet[i]; - 170", true);
                Thing thing = searchSet[i];
                Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - num2++; - 171", true);
                num2++;
                Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - float num5 = (root - thing.Position).LengthManhattan; - 172", true);
                float num5 = (root - thing.Position).LengthManhattan;
                Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - if (!(num5 > maxDistance)) - 173", true);
                if (!(num5 > maxDistance))
                {
                    Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - num3 = FoodUtility.FoodOptimality(eater, thing, FoodUtility.GetFinalIngestibleDef(thing), num5); - 174", true);
                    num3 = FoodUtility.FoodOptimality(eater, thing, FoodUtility.GetFinalIngestibleDef(thing), num5);
                    Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - if (!(num3 < num4) && pawn.Map.reachability.CanReach(root, thing, peMode, traverseParams) && thing.Spawned && (validator == null || validator(thing))) - 175", true);
                    if (!(num3 < num4) && pawn.Map.reachability.CanReach(root, thing, peMode, traverseParams) && thing.Spawned && (validator == null || validator(thing)))
                    {
                        Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - result = thing; - 176", true);
                        result = thing;
                        Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - num4 = num3; - 177", true);
                        num4 = num3;
                        Log.Message("SA_Utils - SpawnedFoodSearchInnerScan - num++; - 178", true);
                        num++;
                    }
                }
            }
            return result;
        }
        private static int GetMaxRegionsToScan(Pawn getter, bool forceScanWholeMap)
        {
            Log.Message("SA_Utils - GetMaxRegionsToScan - if (getter.RaceProps.Humanlike) - 180", true);
            if (getter.RaceProps.Humanlike)
            {
                Log.Message("SA_Utils - GetMaxRegionsToScan - return -1; - 181", true);
                return -1;
            }
            Log.Message("SA_Utils - GetMaxRegionsToScan - if (forceScanWholeMap) - 182", true);
            if (forceScanWholeMap)
            {
                Log.Message("SA_Utils - GetMaxRegionsToScan - return -1; - 183", true);
                return -1;
            }
            Log.Message("SA_Utils - GetMaxRegionsToScan - if (getter.Faction == Faction.OfPlayer) - 184", true);
            if (getter.Faction == Faction.OfPlayer)
            {
                Log.Message("SA_Utils - GetMaxRegionsToScan - return 100; - 185", true);
                return 100;
            }
            return 30;
        }
    }
}