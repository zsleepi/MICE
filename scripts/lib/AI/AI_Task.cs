using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.GodotThread;

namespace MICE.scripts.lib.AI
{
    public abstract class AI_Task: ITask
    {
        internal string _name;
        internal string _description;
        internal float _priority;
        public abstract void DoTask();
        public abstract void DeleteTask();
        public abstract void CheckTaskFinished();
        public float GetPriority() { return _priority; }
        public string GetTaskName() { return _name; }
        public string GetTaskDescription() { return _description; }

    }
}
