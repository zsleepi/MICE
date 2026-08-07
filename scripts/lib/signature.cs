using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Signature(string _primaryHex, string _secondaryHex)
    {
        public Color primary = Color.FromHtml(_primaryHex);

        public Color secondary = Color.FromHtml(_secondaryHex);

        public void Set(string _primaryHex, string _secondaryHex)
        {
            primary = Color.FromHtml(_primaryHex);
            secondary = Color.FromHtml(_secondaryHex);
        }

        public void Randomize()
        {
            Random rnd = new Random();
            primary = new Color(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
            secondary = new Color(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle());
        }
    }
}
