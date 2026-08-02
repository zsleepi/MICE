using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Signature
    {
        public Color primary;

        public Color secondary;

        public void Set(Color _primary, Color _secondary)
        {
            primary = _primary;
            secondary = _secondary;
        }

        public void Randomize()
        {
            Random rnd = new Random();
            primary = new Color(rnd.Next(), rnd.Next(), rnd.Next());
            secondary = new Color(rnd.Next(), rnd.Next(), rnd.Next());
        }
    }
}
