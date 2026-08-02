using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.itemlogic
{
    public class ItemEntry(IItem item, int quantity)
    {
        public IItem Item { get; set; } = item;
        public int Quantity { get; set; } = quantity;
    }
}
