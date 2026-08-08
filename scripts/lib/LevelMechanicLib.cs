using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public static class LevelMechanicLib
    {
        public static int GetLevelExpRequirement(int currentLevel)
        {
            return (int)Math.Ceiling((100 + (20*currentLevel)) * Math.Pow(1.02, currentLevel));
        }
        public static int SkillPointsPerLevel = 2;
    }
}
