using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskFramework.Lib
{
    /// <summary>
    /// 任务事件参数
    /// </summary>
    public class TaskArgs : EventArgs
    {
        /// <summary>
        /// 任务
        /// </summary>
        public IParallelTask Task{ private set;get;}
        /// <summary>
        /// 任务上下文
        /// </summary>
        public IParallelTaskContext Context{private set;get;}

        /// <summary>
        /// 
        /// </summary>
        public object State { set; get; }
        
        /// <summary>
        /// 任务事件参数构造方法
        /// </summary>
        /// <param name="t"></param>
        public TaskArgs(IParallelTask t,IParallelTaskContext context,object state)
        {
            this.Task = t;
            this.Context = context;
            this.State = state;
        }
    }
}
