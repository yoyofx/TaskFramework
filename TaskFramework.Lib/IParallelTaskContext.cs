using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFramework.Lib
{
    /// <summary>
    /// 上下文接口
    /// </summary>
    public interface IParallelTaskContext
    {
        /// <summary>
        /// UI线程上下文接口
        /// </summary>
        TaskScheduler UIThread { get; }

        /// <summary>
        /// 线程池上下文接口
        /// </summary>
        TaskScheduler ParallelScheduler { get; }

        /// <summary>
        /// 
        /// </summary>
        TaskFactory Factory { get; }

        /// <summary>
        /// 方法
        /// </summary>
        /// <param name="ac"></param>
        void Invoke(Action ac);

    }
}
