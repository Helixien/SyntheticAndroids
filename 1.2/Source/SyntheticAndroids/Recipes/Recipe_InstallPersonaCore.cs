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
    public class Recipe_InstallPersonaCore : Recipe_Surgery
    {
        public override bool AvailableOnNow(Thing thing)
        {
			if (thing is Pawn pawn && !SA_Utils.HasTrait(pawn, SADefOf.SA_Sentient))
            {
				return true;
            }
			return false;
        }
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, delegate (BodyPartRecord record)
            {
                if (!pawn.health.hediffSet.GetNotMissingParts().Contains(record))
                {
                    return false;
                }
                if (pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record))
                {
                    return false;
                }
                return (!pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part == record && (x.def == recipe.addsHediff || !recipe.CompatibleWithHediff(x.def)))) ? true : false;
            });
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
            }
			pawn.story.traits.GainTrait(new Trait(SADefOf.SA_Sentient));
			GeneratePassions(pawn);
			pawn.health.AddHediff(recipe.addsHediff, part);
			Find.LetterStack.ReceiveLetter("SA.GainedSentience".Translate(pawn.Named("PAWN")), "SA.GainedSentienceDesc".Translate(pawn.Named("PAWN")), LetterDefOf.NeutralEvent, pawn);
		}

		private void GeneratePassions(Pawn pawn)
		{
			List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				SkillDef skillDef = allDefsListForReading[i];
				SkillRecord skill = pawn.skills.GetSkill(skillDef);
				if (skill.TotallyDisabled)
				{
					continue;
				}
				int num = skill.Level;
				bool flag = false;
				bool flag2 = false;
				foreach (Trait allTrait in pawn.story.traits.allTraits)
				{
					if (allTrait.def.ConflictsWithPassion(skillDef))
					{
						flag = true;
						flag2 = false;
						break;
					}
					if (allTrait.def.RequiresPassion(skillDef))
					{
						flag2 = true;
					}
				}
				if (!flag)
				{
					float num2 = (float)num * 0.11f;
					float value = Rand.Value;
					if (flag2 || value < num2)
					{
						if (value < num2 * 0.2f)
						{
							skill.passion = Passion.Major;
						}
						else
						{
							skill.passion = Passion.Minor;
						}
					}
				}
			}
		}
	}
}