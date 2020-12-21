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
    class SyntheticAndroidsMod : Mod
    {
        public static SyntheticAndroidsSettings settings;
        public SyntheticAndroidsMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<SyntheticAndroidsSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            var skills = DefDatabase<SkillDef>.AllDefsListForReading;
            foreach (var skill in skills)
            {
                if (settings.skillStates == null)
                    settings.skillStates = new Dictionary<string, bool>();
                if (settings.skillCapStates == null)
                    settings.skillCapStates = new Dictionary<string, int>();

                if (!settings.skillStates.ContainsKey(skill.defName))
                {
                    settings.skillStates[skill.defName] = true;
                }
                if (!settings.skillCapStates.ContainsKey(skill.defName))
                {
                    settings.skillCapStates[skill.defName] = 10;
                }
            }
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Rimworld: Synthetic Androids";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }
}