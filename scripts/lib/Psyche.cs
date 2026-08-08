using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Psyche
    {
        internal int PP;
        internal int MaxPP;

        public void Damage(int damage)
        {
            PP -= damage;
            PP = Math.Max(PP, 0);
        }

        public void Restore(int healing)
        {
            PP += healing;
            PP = Math.Min(PP, MaxPP);
        }
    }
}
