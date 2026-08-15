using MICE.scripts.lib.knowledge_logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.knowledge
{
    public class ActorKnowledge(IActor _owner)
    {
        internal IActor Owner = _owner; 
        public int SlotAmount = GetKnowledgeSlotCount(_owner.coreSkills.Intelligence);
        public KnowledgeSlot[] Slots = [];
        public static int GetKnowledgeSlotCount(int _intelligence)
        {
            return (int)Math.Ceiling((decimal)_intelligence / 2);
        }
    }
}
