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
        public static Dictionary<Pawn, CompAndroid> cachedAndroidComps = new Dictionary<Pawn, CompAndroid>();
        public static CompAndroid GetAndroidComp(this Pawn pawn)
        {
            if (cachedAndroidComps.TryGetValue(pawn, out CompAndroid comp))
            {
                return comp;
            }
            else
            {
                var androidComp = pawn.TryGetComp<CompAndroid>();
                if (androidComp != null)
                {
                    cachedAndroidComps[pawn] = androidComp;
                }
                return androidComp;
            }
        }

        public static bool HasTrait(this Pawn pawn, TraitDef traitDef)
        {
            if (traitDef != null && (pawn?.story?.traits?.HasTrait(traitDef) ?? false))
            {
                return true;
            }
            return false;
        }
        public static bool IsAndroid(this Pawn pawn)
        {
            if (pawn.def == SADefOf.SA_Android)
            {
                return true;
            }
            return false;
        }

        private static HashSet<Thing> filtered = new HashSet<Thing>();
        private static List<Pawn> tmpPredatorCandidates = new List<Pawn>();
        public static bool TryFindBestFoodSourceFor(Pawn getter, Pawn eater, bool desperate, out Thing foodSource, out ThingDef foodDef, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false, bool ignoreReservations = false, FoodPreferability minPrefOverride = FoodPreferability.Undefined)
        {
            bool flag = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            bool allowDrug = !eater.IsTeetotaler();
            Thing thing = null;
            if (canUseInventory)
            {
                if (flag)
                {
                    thing = FoodUtility.BestFoodInInventory(getter, eater, (minPrefOverride == FoodPreferability.Undefined) ? FoodPreferability.MealAwful : minPrefOverride);
                }
                if (thing != null)
                {
                    if (getter.Faction != Faction.OfPlayer)
                    {
                        foodSource = thing;
                        foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                    CompRottable compRottable = thing.TryGetComp<CompRottable>();
                    if (compRottable != null && compRottable.Stage == RotStage.Fresh && compRottable.TicksUntilRotAtCurrentTemp < 30000)
                    {
                        foodSource = thing;
                        foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                }
            }
            bool allowPlant = getter == eater;
            bool allowForbidden2 = allowForbidden;
            ThingDef foodDef2;
            Thing thing2 = BestFoodSourceOnMap(getter, eater, desperate, out foodDef2, FoodPreferability.MealLavish, allowPlant, allowDrug, allowCorpse, allowDispenserFull: true, canRefillDispenser, allowForbidden2, allowSociallyImproper, allowHarvest, forceScanWholeMap, ignoreReservations, minPrefOverride);
            if (thing != null || thing2 != null)
            {
                if (thing == null && thing2 != null)
                {
                    foodSource = thing2;
                    foodDef = foodDef2;
                    return true;
                }
                ThingDef finalIngestibleDef = FoodUtility.GetFinalIngestibleDef(thing);
                if (thing2 == null)
                {
                    foodSource = thing;
                    foodDef = finalIngestibleDef;
                    return true;
                }
                float num = FoodUtility.FoodOptimality(eater, thing2, foodDef2, (getter.Position - thing2.Position).LengthManhattan);
                float num2 = FoodUtility.FoodOptimality(eater, thing, finalIngestibleDef, 0f);
                num2 -= 32f;
                if (num > num2)
                {
                    foodSource = thing2;
                    foodDef = foodDef2;
                    return true;
                }
                foodSource = thing;
                foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                return true;
            }
            if (canUseInventory && flag)
            {
                thing = FoodUtility.BestFoodInInventory(getter, eater, FoodPreferability.DesperateOnly, FoodPreferability.MealLavish, 0f, allowDrug);
                if (thing != null)
                {
                    foodSource = thing;
                    foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                    return true;
                }
            }
            if (thing2 == null && getter == eater && (getter.RaceProps.predator || (getter.IsWildMan() && !getter.IsPrisoner && !getter.WorkTypeIsDisabled(WorkTypeDefOf.Hunting))))
            {
                Pawn pawn = BestPawnToHuntForPredator(getter, forceScanWholeMap);
                if (pawn != null)
                {
                    foodSource = pawn;
                    foodDef = FoodUtility.GetFinalIngestibleDef(foodSource);
                    return true;
                }
            }
            foodSource = null;
            foodDef = null;
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
            if (predator.meleeVerbs.TryGetMeleeVerb(null) == null)
            {
                return null;
            }
            bool flag = false;
            if (predator.health.summaryHealth.SummaryHealthPercent < 0.25f)
            {
                flag = true;
            }
            tmpPredatorCandidates.Clear();
            if (GetMaxRegionsToScan(predator, forceScanWholeMap) < 0)
            {
                tmpPredatorCandidates.AddRange(predator.Map.mapPawns.AllPawnsSpawned);
            }
            else
            {
                TraverseParms traverseParms = TraverseParms.For(predator);
                RegionTraverser.BreadthFirstTraverse(predator.Position, predator.Map, (Region from, Region to) => to.Allows(traverseParms, isDestination: true), delegate (Region x)
                {
                    List<Thing> list = x.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
                    for (int j = 0; j < list.Count; j++)
                    {
                        tmpPredatorCandidates.Add((Pawn)list[j]);
                    }
                    return false;
                });
            }
            Pawn pawn = null;
            float num = 0f;
            bool tutorialMode = TutorSystem.TutorialMode;
            for (int i = 0; i < tmpPredatorCandidates.Count; i++)
            {
                Pawn pawn2 = tmpPredatorCandidates[i];
                if (predator.GetRoom() == pawn2.GetRoom() && predator != pawn2 && (!flag || pawn2.Downed) && FoodUtility.IsAcceptablePreyFor(predator, pawn2) && predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly) && !pawn2.IsForbidden(predator) && (!tutorialMode || pawn2.Faction != Faction.OfPlayer))
                {
                    float preyScoreFor = FoodUtility.GetPreyScoreFor(predator, pawn2);
                    if (preyScoreFor > num || pawn == null)
                    {
                        num = preyScoreFor;
                        pawn = pawn2;
                    }
                }
            }
            tmpPredatorCandidates.Clear();
            return pawn;
        }
        public static Thing BestFoodSourceOnMap(Pawn getter, Pawn eater, bool desperate, out ThingDef foodDef, FoodPreferability maxPref = FoodPreferability.MealLavish, bool allowPlant = true, bool allowDrug = true, bool allowCorpse = true, bool allowDispenserFull = true, bool allowDispenserEmpty = true, bool allowForbidden = false, bool allowSociallyImproper = false, bool allowHarvest = false, bool forceScanWholeMap = false, bool ignoreReservations = false, FoodPreferability minPrefOverride = FoodPreferability.Undefined)
        {
            foodDef = null;
            bool getterCanManipulate = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            if (!getterCanManipulate && getter != eater)
            {
                Log.Error(string.Concat(getter, " tried to find food to bring to ", eater, " but ", getter, " is incapable of Manipulation."));
                return null;
            }
            FoodPreferability minPref;
            if (minPrefOverride == FoodPreferability.Undefined)
            {
                if (eater.NonHumanlikeOrWildMan())
                {
                    minPref = FoodPreferability.NeverForNutrition;
                }
                else if (desperate)
                {
                    minPref = FoodPreferability.DesperateOnly;
                }
                else
                {
                    minPref = (((int)eater.needs.TryGetNeed<Need_Energy>().CurCategory >= 2) ? FoodPreferability.RawBad : FoodPreferability.MealAwful);
                }
            }
            else
            {
                minPref = minPrefOverride;
            }
            Predicate<Thing> foodValidator = delegate (Thing t)
            {
                Building_NutrientPasteDispenser building_NutrientPasteDispenser = t as Building_NutrientPasteDispenser;
                if (building_NutrientPasteDispenser != null)
                {
                    if (!allowDispenserFull || !getterCanManipulate || (int)ThingDefOf.MealNutrientPaste.ingestible.preferability < (int)minPref || (int)ThingDefOf.MealNutrientPaste.ingestible.preferability > (int)maxPref || !eater.WillEat(ThingDefOf.MealNutrientPaste, getter) || (t.Faction != getter.Faction && t.Faction != getter.HostFaction) || (!allowForbidden && t.IsForbidden(getter)) || !building_NutrientPasteDispenser.powerComp.PowerOn || (!allowDispenserEmpty && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers()) || !t.InteractionCell.Standable(t.Map) || !IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) || !getter.Map.reachability.CanReachNonLocal(getter.Position, new TargetInfo(t.InteractionCell, t.Map), PathEndMode.OnCell, TraverseParms.For(getter, Danger.Some)))
                    {
                        return false;
                    }
                }
                else
                {
                    int stackCount = 1;
                    if (FoodUtility.bestFoodSourceOnMap_minNutrition_NewTemp.HasValue)
                    {
                        float statValue = t.GetStatValue(StatDefOf.Nutrition);
                        stackCount = FoodUtility.StackCountForNutrition(FoodUtility.bestFoodSourceOnMap_minNutrition_NewTemp.Value, statValue);
                    }
                    if ((int)t.def.ingestible.preferability < (int)minPref || (int)t.def.ingestible.preferability > (int)maxPref || !eater.WillEat(t, getter) || !t.def.IsNutritionGivingIngestible || !t.IngestibleNow || (!allowCorpse && t is Corpse) || (!allowDrug && t.def.IsDrug) || (!allowForbidden && t.IsForbidden(getter)) || (!desperate && t.IsNotFresh()) || t.IsDessicated() || !IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) || (!getter.AnimalAwareOf(t) && !forceScanWholeMap) || (!ignoreReservations && !getter.CanReserve(t, 10, stackCount)))
                    {
                        return false;
                    }
                }
                return true;
            };
            ThingRequest thingRequest = (!((eater.RaceProps.foodType & (FoodTypeFlags.Plant | FoodTypeFlags.Tree)) != 0 && allowPlant)) ? ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree) : ThingRequest.ForGroup(ThingRequestGroup.FoodSource);
            Thing bestThing;
            if (getter.RaceProps.Humanlike)
            {
                bestThing = SpawnedFoodSearchInnerScan(eater, getter.Position, getter.Map.listerThings.ThingsMatching(thingRequest), PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, foodValidator);
                if (allowHarvest && getterCanManipulate)
                {
                    Thing thing = GenClosest.ClosestThingReachable(searchRegionsMax: (!forceScanWholeMap || bestThing != null) ? 30 : (-1), root: getter.Position, map: getter.Map, thingReq: ThingRequest.ForGroup(ThingRequestGroup.HarvestablePlant), peMode: PathEndMode.Touch, traverseParams: TraverseParms.For(getter), maxDistance: 9999f, validator: delegate (Thing x)
                    {
                        Plant plant = (Plant)x;
                        if (!plant.HarvestableNow)
                        {
                            return false;
                        }
                        ThingDef harvestedThingDef = plant.def.plant.harvestedThingDef;
                        if (!harvestedThingDef.IsNutritionGivingIngestible)
                        {
                            return false;
                        }
                        if (!eater.WillEat(harvestedThingDef, getter))
                        {
                            return false;
                        }
                        if (!getter.CanReserve(plant))
                        {
                            return false;
                        }
                        if (!allowForbidden && plant.IsForbidden(getter))
                        {
                            return false;
                        }
                        return (bestThing == null || (int)FoodUtility.GetFinalIngestibleDef(bestThing).ingestible.preferability < (int)harvestedThingDef.ingestible.preferability) ? true : false;
                    });
                    if (thing != null)
                    {
                        bestThing = thing;
                        foodDef = FoodUtility.GetFinalIngestibleDef(thing, harvest: true);
                    }
                }
                if (foodDef == null && bestThing != null)
                {
                    foodDef = FoodUtility.GetFinalIngestibleDef(bestThing);
                }
            }
            else
            {
                int maxRegionsToScan = GetMaxRegionsToScan(getter, forceScanWholeMap);
                filtered.Clear();
                foreach (Thing item in GenRadial.RadialDistinctThingsAround(getter.Position, getter.Map, 2f, useCenter: true))
                {
                    Pawn pawn = item as Pawn;
                    if (pawn != null && pawn != getter && pawn.RaceProps.Animal && pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Ingest && pawn.CurJob.GetTarget(TargetIndex.A).HasThing)
                    {
                        filtered.Add(pawn.CurJob.GetTarget(TargetIndex.A).Thing);
                    }
                }
                bool ignoreEntirelyForbiddenRegions = !allowForbidden && ForbidUtility.CaresAboutForbidden(getter, cellTarget: true) && getter.playerSettings != null && getter.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap != null;
                Predicate<Thing> validator = delegate (Thing t)
                {
                    if (!foodValidator(t))
                    {
                        return false;
                    }
                    if (filtered.Contains(t))
                    {
                        return false;
                    }
                    if (!(t is Building_NutrientPasteDispenser) && (int)t.def.ingestible.preferability <= 2)
                    {
                        return false;
                    }
                    return (!t.IsNotFresh()) ? true : false;
                };
                bestThing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, validator, null, 0, maxRegionsToScan, forceAllowGlobalSearch: false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions);
                filtered.Clear();
                if (bestThing == null)
                {
                    desperate = true;
                    bestThing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, foodValidator, null, 0, maxRegionsToScan, forceAllowGlobalSearch: false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions);
                }
                if (bestThing != null)
                {
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
            if (!allowSociallyImproper)
            {
                bool animalsCare = !getter.RaceProps.Animal;
                if (!t.IsSociallyProper(getter) && !t.IsSociallyProper(eater, eater.IsPrisonerOfColony, animalsCare))
                {
                    return false;
                }
            }
            return true;
        }

        private static Thing SpawnedFoodSearchInnerScan(Pawn eater, IntVec3 root, List<Thing> searchSet, PathEndMode peMode, TraverseParms traverseParams, float maxDistance = 9999f, Predicate<Thing> validator = null)
        {
            if (searchSet == null)
            {
                return null;
            }
            Pawn pawn = traverseParams.pawn ?? eater;
            int num = 0;
            int num2 = 0;
            Thing result = null;
            float num3 = 0f;
            float num4 = float.MinValue;
            for (int i = 0; i < searchSet.Count; i++)
            {
                Thing thing = searchSet[i];
                num2++;
                float num5 = (root - thing.Position).LengthManhattan;
                if (!(num5 > maxDistance))
                {
                    num3 = FoodUtility.FoodOptimality(eater, thing, FoodUtility.GetFinalIngestibleDef(thing), num5);
                    if (!(num3 < num4) && pawn.Map.reachability.CanReach(root, thing, peMode, traverseParams) && thing.Spawned && (validator == null || validator(thing)))
                    {
                        result = thing;
                        num4 = num3;
                        num++;
                    }
                }
            }
            return result;
        }
        private static int GetMaxRegionsToScan(Pawn getter, bool forceScanWholeMap)
        {
            if (getter.RaceProps.Humanlike)
            {
                return -1;
            }
            if (forceScanWholeMap)
            {
                return -1;
            }
            if (getter.Faction == Faction.OfPlayer)
            {
                return 100;
            }
            return 30;
        }
    }
}