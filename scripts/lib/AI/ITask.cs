using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.AI
{
    public interface ITask
    {
        public abstract void DoTask();
        public abstract void DeleteTask();
        public abstract void CheckTaskFinished();
        public abstract string GetTaskName();
        public abstract string GetTaskDescription();
        public abstract float GetPriority();
    }
}
