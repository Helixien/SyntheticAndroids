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
    public class Hediff_Overheating : Hediff
    {
        public override void Tick()
        {
            base.Tick();
            if (this.severityInt >= 0.99f)
            {
                GenExplosion.DoExplosion(radius: 2f, center: pawn.Position, map: pawn.Map, damType: DamageDefOf.Bomb, instigator: null);
                this.pawn.Kill(null, this);
            }
        }
    }
}
