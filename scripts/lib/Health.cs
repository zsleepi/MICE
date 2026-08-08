using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MICE.scripts.lib
{
    public class Health(int Constitution)
    {
        internal int maxHealthPoints = Constitution * 5 + 1;
        internal int healthPoints = Constitution * 5 + 1;

        public void Damage( int damage )
        {
            healthPoints -= damage;
            healthPoints = Math.Max( healthPoints, 0 );
            if ( healthPoints == 0 ) { GD.Print("defeated!"); }
        }

        public void Restore(int healthPoints)
        {
            healthPoints += healthPoints;
            healthPoints = Math.Min(healthPoints, maxHealthPoints);
        }
    }
}
