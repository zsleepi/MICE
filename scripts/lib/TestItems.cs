using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class TestItems
    {
        public static List<Item> ItemList = [new("Stíck", "it's a stick", 1, true),
        new("Gold Nugget", "So shiny...", 100, true),
        new("Iron Sword", "swish swish swoosh", 50, false),
        new("Wooden Sword", "swish swish swoosh but worse", 5, false),
        ];
    }
}
