using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Experience(int _startingLevel)
    {
        internal int lvl = _startingLevel;
        internal int exp = 0;
        internal static int EXP_TO_LEVEL = 100; // make statics non-instance data ?

        internal int skillPoints = _startingLevel * SKILL_POINTS_PER_LEVEL;
        internal static int SKILL_POINTS_PER_LEVEL = 2;

        public int GetLevel() { return lvl; }

        public int GetExp() { return exp; }

        public void GainExp(int Exp)
        {
            exp += Exp;
            while (exp >= EXP_TO_LEVEL) {
                LevelUp();
                exp -= EXP_TO_LEVEL;
            }
        }

        public void DrainExp(int DrainedExp)
        {
            exp -= DrainedExp;
            while (exp < 0)
            {
                LevelDown();
                exp += EXP_TO_LEVEL;
            }

            while (skillPoints < 0)
            {
                // TODO remove invested skill points until debt resolved
                skillPoints++;
            }
        }

        public void LevelUp()
        {
            lvl++;
            skillPoints += 2;
        }

        public void LevelDown()
        {
            lvl--;
            skillPoints -= 2;
        }

    }
}
