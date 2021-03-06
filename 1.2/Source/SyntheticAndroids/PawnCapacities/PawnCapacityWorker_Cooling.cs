﻿using RimWorld;
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
	public class PawnCapacityWorker_Cooling : PawnCapacityWorker
	{
		public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
		{
			return PawnCapacityUtility.CalculateTagEfficiency(diffSet, SADefOf.SA_CoolingSource, float.MaxValue, default(FloatRange), impactors);
		}
		public override bool CanHaveCapacity(BodyDef body)
		{
			return body.HasPartWithTag(SADefOf.SA_CoolingSource);
		}
	}
}