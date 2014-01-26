using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskFramework.Lib
{
    public class SimpleParallelTask : IParallelTask
    {
        #region 字段和属性

        private object _asyncState;

        /// <summary>
        /// 任务是否完成
        /// </summary>
        public bool IsCompleted
        {
            get { return this._task.IsCompleted; }
        }

        /// <summary>
        /// 任务是否取消
        /// </summary>
        public bool IsCanceld
        {
            get { return this._task.IsCanceled; }
        }

        /// <summary>
        /// 是否由于未经处理异常的原因而完成
        /// </summary>
        public bool IsFaulted
        {
            get { return this._task.IsFaulted; }
        }

        /// <summary>
        /// 用户定义的异步对象信息
        /// </summary>
        public object AsyncState
        {
            get { return _asyncState; }
            private set
            {
                if (_asyncState != value)
                {
                    _asyncState = value;
                }
            }
        }

        /// <summary>
        /// 任务ID
        /// </summary>
        public int Id
        {
            get { return this._task.Id; }
        }

        /// <summary>
        /// 获取此任务的状态(System.Threading.Tasks.TaskStatus)
        /// </summary>
        public TaskStatus Status
        {
            get { return this._task.Status; }
        }

        #endregion

        #region 私有字段

        /// <summary>
        /// 
        /// </summary>
        private Action<object> _actionWithObject = null;

        /// <summary>
        /// 
        /// </summary>
        private volatile Task _task = null;

        //cancel token define here
        private CancellationTokenSource _cancellationTokenSource = null;
        private CancellationToken _cancellationToken;

        public CancellationToken CancellationToken { get { return _cancellationToken; } }

        #endregion

        #region 构造

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        public SimpleParallelTask(Action<object> action, object state)
            : this()
        {
            this._actionWithObject = action;
            this.AsyncState = state;
            this._task = new Task(this._actionWithObject, this.AsyncState, this._cancellationToken);
        }

        /// <summary>
        /// 无参数私有构造方法
        /// </summary>
        private SimpleParallelTask()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        #endregion

        /// <summary>
        /// 开始任务，根据用户设定值决定是否异步
        /// </summary>
        public void Start()
        {
            this._task = this.Context.Factory.StartNew(this._actionWithObject,new TaskArgs(this,this.Context,this.AsyncState),_cancellationToken);
        }

        /// <summary>
        /// 取消任务
        /// </summary>
        public void Cancel()
        {
            this._cancellationTokenSource.Cancel();
        }

        public IParallelTaskContext Context { get;private set; }
        public void SetContext(IParallelTaskContext context)
        {
            if (this.Context == null)
                this.Context = context;
            else
                throw new InvalidOperationException("不能重复设置任务上下文！");
        }
      
    }
}
