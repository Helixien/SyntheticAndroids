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

        public static bool IsAndroidMentalState(this MentalStateDef mentalStateDef)
        {
            return false;
        }
    }
}