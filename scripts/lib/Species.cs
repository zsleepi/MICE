using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Species(string Name, string NamePlural,string SpritePath, CoreSkills SpeciesSkillBonus, Signature BaseSig )
    {
        public string Name = Name, NamePlural = NamePlural;
        public string SpritePath = SpritePath;
        public CoreSkills SpeciesSkillBonus = SpeciesSkillBonus;
        public Signature BaseSig = BaseSig;
    }
}
