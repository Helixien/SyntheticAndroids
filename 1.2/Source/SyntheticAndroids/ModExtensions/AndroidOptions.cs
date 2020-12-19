using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SyntheticAndroids
{
    public class AndroidOptions : DefModExtension
    {
        public List<TraitDef> disallowedTraits;
        public List<ThoughtDef> disallowedThoughts;
    }
}
