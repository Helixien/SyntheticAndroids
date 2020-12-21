using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace SyntheticAndroids
{
    [HarmonyPatch(typeof(SkillRecord), "Learn")]
    public static class Learn_Patch
    {
        public static void Postfix(SkillRecord __instance, Pawn ___pawn, float xp, bool direct = false)
        {
            if (___pawn.IsAndroid() && !___pawn.HasTrait(SADefOf.SA_Sentient) && __instance.Level > SyntheticAndroidsMod.settings.skillCapStates[__instance.def.defName])
            {
                __instance.Level = SyntheticAndroidsMod.settings.skillCapStates[__instance.def.defName];
            }
        }
    }

    [HarmonyPatch(typeof(InspirationHandler), "TryStartInspiration_NewTemp")]
    public static class TryStartInspiration_NewTemp_Patch
    {
        public static bool Prefix(InspirationHandler __instance)
        {
            if (__instance.pawn.IsAndroid())
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    public static class TryStartMentalState_Patch
    {
        public static bool Prefix(MentalStateHandler __instance, MentalStateDef stateDef, Pawn ___pawn)
        {
            if (___pawn.IsAndroid() && !SA_Utils.IsAndroidMentalState(stateDef))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AutoUndrafter), "ShouldAutoUndraft")]
    public static class ShouldAutoUndraft_Patch
    {
        public static bool Prefix(AutoUndrafter __instance, Pawn ___pawn)
        {
            if (___pawn.IsAndroid())
            {
                var energy = ___pawn.needs.TryGetNeed<Need_Energy>();
                if (energy.CurLevel > 0.10f)
                {
                    return false;
                }
            }
            return true;
        }
    }
}