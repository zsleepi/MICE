using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Species(string name, string namePlural, Signature baseSig, string spritePath, CoreSkills skillBonus)
    {
        public string name = name;
        public string namePlural = namePlural;

        public Signature baseSig = baseSig;
        public string spritePath = spritePath;

        public CoreSkills skillBonus = skillBonus;
    }
}
