using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Health()
    {
        internal int HP;
        internal int MaxHP;

        public void Damage(int damage)
        {
            HP -= damage;
            HP = Math.Max(HP, 0);
        }

        public void Restore(int healing)
        {
            HP += healing;
            HP = Math.Min(HP, MaxHP);
        }
    }
}
