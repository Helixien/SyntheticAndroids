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
	public class MentalState_Wander_DazedAndroid : MentalState
	{
		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}

        public override bool CanEndBeforeMaxDurationNow
        {
			get
            {
				if (this.pawn.IsAndroid())
                {
                    if (this.pawn.GetComp<CompAndroid>().dazedState)
                    {
                        return false;
                    }
                    return true;
                }
                return base.CanEndBeforeMaxDurationNow;
            }
        }
    }
}