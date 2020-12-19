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

namespace SyntheticAndroids
{


    [HarmonyPatch(typeof(SkillRecord), "Learn")]
    public static class Learn_Patch
    {
        public static bool Prefix(SkillRecord __instance, Pawn ___pawn, float xp, bool direct = false)
        {
            if (___pawn.IsAndroid() && !___pawn.HasTrait(SADefOf.SA_Sentient) && __instance.Level <= 8)
            {
                var xpLocal = xp;
                xpLocal *= __instance.LearnRateFactor(direct);
                var xpSinceLastLevel = __instance.xpSinceLastLevel + xpLocal;
                if (xpSinceLastLevel >= __instance.XpRequiredForLevelUp)
                {
                    return false;
                }
            }
            return true;
        }
    }
}