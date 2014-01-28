using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskFramework.Lib
{
    public interface IParallelTask
    {
        /// <summary>
        /// 任务上下文
        /// </summary>
        IParallelTaskContext Context { get; }

        void SetContext(IParallelTaskContext context);

        /// <summary>
        /// 任务ID
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 是否由于未经处理异常的原因而完成
        /// </summary>
        bool IsFaulted { get; }

        /// <summary>
        /// 是否取消
        /// </summary>
        bool IsCanceld { get; }

        /// <summary>
        /// 用户定义的对象，它限定或包含关于异步操作的信息。
        /// </summary>
        object AsyncState { get; }

        /// <summary>
        /// 获取此任务的 System.Threading.Tasks.TaskStatus
        /// </summary>
        TaskStatus Status { get; }

        CancellationToken CancellationToken { get; }
        /// <summary>
        /// 开始任务
        /// </summary>
        void Start();

        /// <summary>
        /// 取消任务
        /// </summary>
        void Cancel();


    }
}
