using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class CoreSkills(int _constitution, int _strength, int _dexterity, int _intelligence, int _psionics, int _charisma)
    {
        public int Constitution = _constitution;
        public int Strength = _strength;
        public int Dexterity = _dexterity;
        public int Intelligence = _intelligence;
        public int Psionics = _psionics;
        public int Charisma = _charisma;
    }
}
