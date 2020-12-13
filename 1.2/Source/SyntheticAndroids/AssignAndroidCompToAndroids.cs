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
	[StaticConstructorOnStartup]
	public static class AssignAndroidCompToAndroids
	{
		static AssignAndroidCompToAndroids()
		{
			SADefOf.SA_Android.comps.Add(new CompProperties_Android());
		}
	}
}