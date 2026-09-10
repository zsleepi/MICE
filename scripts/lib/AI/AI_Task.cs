using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.GodotThread;

namespace MICE.scripts.lib.AI
{
    public abstract class AI_Task(NPC _owner, string name, string desc, float priority): ITask
    {
        private readonly NPC _MYSELF = _owner;
        internal string _NAME = name;
        internal string _DESCRIPTION = desc;
        internal float _PRIORITY = priority;
        public abstract void DoTask();
        public abstract void DeleteTask();
        public abstract void CheckTaskFinished();
        public string GetTaskName() { return _NAME; }
        public string GetTaskDescription() { return _DESCRIPTION; }
        public float GetPriority() { return _PRIORITY; }

    }
}
