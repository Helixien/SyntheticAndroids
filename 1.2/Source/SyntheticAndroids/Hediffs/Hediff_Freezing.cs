using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SyntheticAndroids
{
    public class Hediff_Freezing : Hediff
    {
        private CompAndroid androidComp;
        public override void Tick()
        {
            base.Tick();
            if (androidComp == null)
                androidComp = SA_Utils.GetAndroidComp(this.pawn);
            if (this.severityInt >= 0.99f)
            {
                if (!androidComp.disabled)
                {
                    androidComp.MakeDisabled();
                }
            }
        }
        public override void PostRemoved()
        {
            base.PostRemoved();
            if (androidComp == null)
                androidComp = SA_Utils.GetAndroidComp(this.pawn);
            if (androidComp.disabled)
            {
                androidComp.TryMakeEnabled();
            }
        }
    }
}
