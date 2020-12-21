using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SyntheticAndroids
{
    class SyntheticAndroidsSettings : ModSettings
    {
        public Dictionary<string, bool> skillStates = new Dictionary<string, bool>();
        public Dictionary<string, int> skillCapStates = new Dictionary<string, int>();
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref skillStates, "skillStates", LookMode.Value, LookMode.Value, ref skillKeys1, ref boolValues);
            Scribe_Collections.Look(ref skillCapStates, "skillCapStates", LookMode.Value, LookMode.Value, ref skillKeys2, ref floatValues);
        }

        private List<string> skillKeys1;
        private List<bool> boolValues;

        private List<string> skillKeys2;
        private List<int> floatValues;
        public void DoSettingsWindowContents(Rect inRect)
        {
            var keys = skillStates.Keys.ToList().OrderByDescending(x => x).ToList();
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect rect2 = new Rect(0f, 0f, inRect.width - 30f, keys.Count * 74);
            Widgets.BeginScrollView(rect, ref scrollPosition, rect2, true);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect2);
            for (int num = keys.Count - 1; num >= 0; num--)
            {
                var test = skillStates[keys[num]];
                var def = DefDatabase<SkillDef>.GetNamed(keys[num]);
                listingStandard.CheckboxLabeled("Disable skill cap for androids - " + def.label + ":", ref test);
                skillStates[keys[num]] = test;
                if (!test)
                {
                    skillCapStates[keys[num]] = 20;
                }
                listingStandard.Label("Adjust max skill cap for androids - " + def.label + ": " + skillCapStates[keys[num]]);
                var value = listingStandard.Slider(skillCapStates[keys[num]], 0, 100);
                skillCapStates[keys[num]] = (int)value;
            }
            listingStandard.End();
            Widgets.EndScrollView();
            base.Write();
        }
        private static Vector2 scrollPosition = Vector2.zero;

    }
}

