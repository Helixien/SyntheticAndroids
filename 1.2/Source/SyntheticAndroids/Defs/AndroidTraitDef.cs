using HarmonyLib;
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
	public class AndroidTraitDef : TraitDef
	{
        public List<PawnKindDef> allowedPawnKindDefs = new List<PawnKindDef>();
    }
}