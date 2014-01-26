using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFramework.Lib
{
    public interface IParallelTaskContext
    {
        TaskScheduler UIThread { get; }
        TaskScheduler ParallelScheduler { get; }

        TaskFactory Factory { get; }
        void Invoke(Action ac);

    }
}
