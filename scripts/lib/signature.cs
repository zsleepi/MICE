using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Signature(Color _primary, Color _secondary)
    {
        public Color primary = _primary;

        public Color secondary = _secondary;

        public void Set(Color _primary, Color _secondary)
        {
            primary = _primary;
            secondary = _secondary;
        }

        public void Randomize()
        {
            Random rnd = new Random();
            primary = new Color(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
            secondary = new Color(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
        }
    }
}
