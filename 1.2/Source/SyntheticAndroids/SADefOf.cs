using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SyntheticAndroids
{
    [DefOf]
    public static class SADefOf
    {
        public static NeedDef SA_Energy;
        public static ThingDef SA_Android;
        public static ThingDef Neutroamine;
        public static JobDef SA_AndroidIngest;
        public static JobDef SA_AndroidLayDown;
        public static JobDef SA_ConsumeNeutroamine;

        public static BodyPartDef SA_PersonalityMatrix;
        public static BodyPartDef SA_CentralProcessor;
        public static BodyPartDef SA_BreathingSimulator;

        public static BodyPartDef SA_PowerCell;
        public static BodyPartDef SA_FluidPump;
        public static BodyPartDef SA_FluidProcessor;

        public static BodyPartTagDef SA_FluidFiltrationSource;
        public static BodyPartTagDef SA_MatterProcessing;
        public static BodyPartTagDef SA_EnergySource;
        public static BodyPartTagDef SA_FluidPumpingSource;
        public static BodyPartTagDef SA_MemorySource;
        public static BodyPartTagDef SA_BreathingSource;
        public static BodyPartTagDef SA_CoolingSource;


        public static MentalStateDef SA_Wander_DazedAndroid;

        public static ThingDef SA_Mote_AndroidDisabledIcon;
    }
}
