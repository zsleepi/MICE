using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MICE.scripts.lib
{
    public class Psyche(int Intelligence)
    {
        internal int maxPsychePoints = Intelligence * 5 + 1;
        internal int psychePoints = Intelligence * 5 + 1;

        public void Damage( int damage )
        {
            psychePoints -= damage;
            psychePoints = Math.Max(psychePoints, 0 );
            if (psychePoints == 0 ) { GD.Print("defeated by mental magicks!"); }
        }

        public void Restore(int healthPoints)
        {
            healthPoints += healthPoints;
            healthPoints = Math.Min(healthPoints, maxPsychePoints);
        }
    }
}
