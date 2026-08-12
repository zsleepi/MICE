using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Experience(IActor owner)
    {
        internal IActor owner = owner;
        internal int exp;
        internal int level;
        internal int skillPoints;

        public int GetExp()
        {
            return exp;
        }

        public int GetLevel()
        {
            return level;
        }

        public void GainEXP(int gain) {
            exp += gain;
            while (exp >= LevelMechanicLib.GetLevelUpExpRequirement(level))
            {
                exp -= LevelMechanicLib.GetLevelUpExpRequirement(level);
                RaiseLevel(1);
            }
        }

        public void DrainExp(int drain, string drainStat)
        {
            exp -= drain;
            while (exp < 0)
            {
                LowerLevel(1, drainStat);
                exp += LevelMechanicLib.GetLevelUpExpRequirement(level);
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
