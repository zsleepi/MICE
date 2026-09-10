using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MICE.scripts.lib.stats.StatMod;

namespace MICE.scripts.lib.stats
{
    public class StatMod(decimal _mod, ModifierType _type, string _source )
    {
        public decimal Modifier = _mod;
        public enum ModifierType { additive, multiplicative };
        public ModifierType Type = _type;
        public string Source = _source;
    }
}
