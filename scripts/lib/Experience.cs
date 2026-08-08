using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Experience
    {
        internal int exp;
        internal int level;
        internal int skillPoints;


        public void GainEXP(int gain) {
            exp += gain;
            while (exp >= LevelMechanicLib.GetLevelExpRequirement(level))
            {
                exp -= LevelMechanicLib.GetLevelExpRequirement(level);
                RaiseLevel(1);
            }
        }

        public void DrainExp(int drain, string drainStat)
        {
            exp -= drain;
            while (exp < 0)
            {
                LowerLevel(1, drainStat);
                exp += LevelMechanicLib.GetLevelExpRequirement(level);
            }
        }

        public void RaiseLevel(int raiseBy) {
            level += raiseBy;
            skillPoints += raiseBy * LevelMechanicLib.SkillPointsPerLevel;
        }

        public void LowerLevel(int lowerBy, string drainStat) {
            level -= lowerBy;
            skillPoints -= lowerBy * LevelMechanicLib.SkillPointsPerLevel;
            ResolveSkillDebt(drainStat);
        }

        public void ResolveSkillDebt(string drainStat)
        {
            while (skillPoints < 0)
            {
                // TODO: get reference to actor's coreskills and do method in it?
                skillPoints++;
            }
        }
    }
}
