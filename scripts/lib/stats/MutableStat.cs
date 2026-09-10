using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.stats
{
    public class MutableStat(decimal baseStat)
    {
        internal decimal _baseStat = baseStat;
        public List<StatMod> Modifiers = [];

        public void SetBase(decimal number) { _baseStat = number; }
        public void ModifyBase(decimal number) { _baseStat += number; }
        public decimal GetBase() { return _baseStat; }

        public decimal GetMod() {
            decimal _stat = _baseStat;
            foreach (StatMod additiveMod in Modifiers.Where( i => i.Type == StatMod.ModifierType.additive  ))
            {
                _stat += additiveMod.Modifier;
                _stat = Math.Max( _stat, 0 ); // prevent negative
            }
            foreach (StatMod multMod in Modifiers.Where(i => i.Type == StatMod.ModifierType.multiplicative))
            {
                _stat *= multMod.Modifier;
            }
            return _stat;
        }
    }
}
 