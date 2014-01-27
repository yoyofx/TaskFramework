using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TaskFramework.Lib
{
    /// <summary>
    /// 任务执行队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ParallelTaskQueue<T> : IParallelTaskContext where T : IParallelTask
    {
        public  TaskScheduler UIThread { private set; get; }
        public TaskScheduler ParallelScheduler { private set; get; }

        private bool _isStoped;

        private int _taskMax = 0;

        /// <summary>
        /// 任务数量的峰值
        /// </summary>
        public int TaskMax { get { return _taskMax; } }

        /// <summary>
        /// T的最大数量，当到达T的最大数量时，开始任务调度，并执行T实现接口中的特定方法
        /// </summary>
        public int Max { get; set; }

        public bool IsStoped { get { return _isStoped; } }

        /// <summary>
        /// 任务是否全部完成
        /// </summary>
        public bool AllDone { get { return _taskMax == DoneTaskCount; } }

        /// <summary>
        /// 阻塞式的工作队列，没有数量上限
        /// </summary>
        public BlockingCollection<T> AllTasks;

        /// <summary>
        /// 工作队列字典，由单独的线程进行调度，监测任务十分完成，并及时进行调度
        /// </summary>
        public ConcurrentDictionary<int, T> WorkTasks;

        /// <summary>
        /// 存放已完成的任务队列，无论是用户取消还是运行结束的任务
        /// </summary>
        public ConcurrentDictionary<int, T> DoneTasks;

        /// <summary>
        /// 获取用户添加的所有任务的数量，这个数量可能随时有变化
        /// </summary>
        public int AllTaskCount { get { return AllTasks.Count; } }

        /// <summary>
        /// 获取任务队列中正在运行的任务的数量，这个数值可能随时有变化
        /// </summary>
        public int WorkTaskCount { get { return WorkTasks.Count; } }

        /// <summary>
        /// 获取已完成的任务队列中的任务数量，这个数值可能随时有变化
        /// </summary>
        public int DoneTaskCount { get { return DoneTasks.Count; } }

        public TaskFactory Factory { private set; get; }


        /// <summary>
        /// 用户添加任务时触发的事件
        /// </summary>
        public event EventHandler<TaskArgs> AllTasksAdded;

        /// <summary>
        /// 工作队列中添加任务触发的事件
        /// </summary>
        public event EventHandler<TaskArgs> WorkTasksAdded;

        /// <summary>
        /// 已完成的任务队列添加时触发的事件
        /// </summary>
        public event EventHandler<TaskArgs> DoneTasksAdded;

        /// <summary>
        /// 终止事件
        /// </summary>
        public event EventHandler OnStop;

        /// <summary>
        /// 正在终止事件
        /// </summary>
        public event EventHandler OnStoping;

        /// <summary>
        /// 暂停事件
        /// </summary>
        public event EventHandler OnPause;

        /// <summary>
        /// 恢复运行事件
        /// </summary>
        public event EventHandler OnResume;

        /// <summary>
        /// 工作任务调度后台线程
        /// </summary>
		//System.Timers.Timer DispatchTaskTimer = new System.Timers.Timer(100);

        /// <summary>
        /// 已完成任务调度后台线程
        /// </summary>
		//System.Timers.Timer DoneTaskTimer = new System.Timers.Timer(100);

		/// <summary>
		/// 队列监控线程
		/// </summary>
		System.Timers.Timer TaskTimer = new System.Timers.Timer(100);

        #region 线程安全的单例实现

        /// <summary>
        /// 单例模式用到的静态变量，初始值为null
        /// </summary>
        static volatile ParallelTaskQueue<T> _instance = null;
        static object obj = new object();

        /// <summary>
        /// 单例，全局保证只有一个实例
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static ParallelTaskQueue<T> GetGlobalCollection(int max = 10)
        {
            if (_instance == null)
            {
                lock (obj)
                {
                    if (_instance == null)
                    {
                        _instance = new ParallelTaskQueue<T>(max);
                    }
                }
            }
            return _instance;
        }

        #endregion

        /// <summary>
        /// 私有构造方法
        /// </summary>
        /// <param name="max"></param>
        private ParallelTaskQueue(int max)
        {
            this.Max = max;
            AllTasks = new BlockingCollection<T>();
            WorkTasks = new ConcurrentDictionary<int, T>(max, max);
            DoneTasks = new ConcurrentDictionary<int, T>();
            this.ParallelScheduler = new LimitedConcurrencyLevelTaskScheduler(max);
            
            this.Factory = new TaskFactory(this.ParallelScheduler);
            this.UIThread = TaskScheduler.FromCurrentSynchronizationContext();

			TaskTimer.Elapsed += TaskTimer_Elapsed;
			//DispatchTaskTimer.Elapsed += DispatchTaskTimer_Elapsed;
			//DoneTaskTimer.Elapsed += DoneTaskTimer_Elapsed;
			switchTimer();
        }

        /// <summary>
        ///  获取上下文
        /// </summary>
        /// <returns></returns>
        public IParallelTaskContext GetContext()
        {
            return this;
        }

        /// <summary>
        /// 设置任务最大数量
        /// </summary>
        /// <param name="count"></param>
        public void SetParallaCount(int count)
        {
            this.Max = count;
            (this.ParallelScheduler as LimitedConcurrencyLevelTaskScheduler).SetMaxDegreeOfParallelism(count);
        }

		/// <summary>
		/// 切换Timer状态
		/// </summary>
		private void switchTimer()
		{
//			DoneTaskTimer.Enabled = !DoneTaskTimer.Enabled;
//			DispatchTaskTimer.Enabled = !DispatchTaskTimer.Enabled;
			TaskTimer.Enabled = !TaskTimer.Enabled;
		}

		/// <summary>
		/// 任务队列监控线程
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void TaskTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			//已完成队列
			Parallel.ForEach(WorkTasks, (x, y) =>
				{
					if (x.Value.Status == TaskStatus.Canceled ||
						x.Value.Status == TaskStatus.Faulted ||
						x.Value.Status == TaskStatus.RanToCompletion)
					{
						DoneTasks.TryAdd(x.Key, x.Value);
						if (DoneTasksAdded != null) DoneTasksAdded.BeginInvoke(this, new TaskArgs(x.Value,this,x.Value.AsyncState), null, null);
						T t;
						WorkTasks.TryRemove(x.Key, out t);
					}
				});

			//工作队列
			if (WorkTasks.Count < Max)
			{
				T t;
				if (AllTasks.TryTake(out t))
				{
					if (WorkTasks.TryAdd(t.Id, t))
					{
						if (t.Status == TaskStatus.Created)
						{
							t.Start();
						}
						if (WorkTasksAdded != null)
							WorkTasksAdded.Invoke(this, new TaskArgs(t,this,t.AsyncState));
					}
				}
			}
		}

        /// <summary>
        /// 已完成任务计时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
//        void DoneTaskTimer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            Parallel.ForEach(WorkTasks, (x, y) =>
//            {
//                if (x.Value.Status == TaskStatus.Canceled ||
//                x.Value.Status == TaskStatus.Faulted ||
//                x.Value.Status == TaskStatus.RanToCompletion)
//                {
//                    DoneTasks.TryAdd(x.Key, x.Value);
//                    if (DoneTasksAdded != null) DoneTasksAdded.BeginInvoke(this, new TaskArgs(x.Value,this,x.Value.AsyncState), null, null);
//                    T t;
//                    WorkTasks.TryRemove(x.Key, out t);
//                }
//            });
//        }
        /// <summary>
        /// 执行任务计时器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
//        void DispatchTaskTimer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            if (WorkTasks.Count < Max)
//            {
//                T t;
//                if (AllTasks.TryTake(out t))
//                {
//                    if (WorkTasks.TryAdd(t.Id, t))
//                    {
//                        if (t.Status == TaskStatus.Created)
//                        {
//                            t.Start();
//                        }
//                        if (WorkTasksAdded != null)
//                            WorkTasksAdded.Invoke(this, new TaskArgs(t,this,t.AsyncState));
//                    }
//                }
//            }
//        }

        /// <summary>
        /// 终止所有线程，清空待执行任务
        /// </summary>
        public void Stop()
        {
			switchTimer();

            //这里应该直接压入到已完成队列，而不是清掉
            while (AllTaskCount > 0)
            {
                T t;
                if (AllTasks.TryTake(out t))
                {
                    if (DoneTasks.TryAdd(t.Id, t) && DoneTasksAdded != null)
                        DoneTasksAdded.Invoke(this, new TaskArgs(t,this,t.AsyncState));
                }
            }

            Task.Factory.StartNew(() =>
            {
                while (WorkTaskCount > 0)
                {
                    //给正在执行的线程传入CancelToken
                    Parallel.ForEach(WorkTasks, (x, y) =>
                    {
                        if (x.Value.Status == TaskStatus.Canceled ||
                        x.Value.Status == TaskStatus.Faulted ||
                        x.Value.Status == TaskStatus.RanToCompletion)
                        {
                            DoneTasks.TryAdd(x.Key, x.Value);
                            if (DoneTasksAdded != null) DoneTasksAdded.Invoke(this, new TaskArgs(x.Value,this,x.Value.AsyncState));
                            T t;
                            WorkTasks.TryRemove(x.Key, out t);
                        }
                        else
                        {
                            x.Value.Cancel();
                        }
                    });
                    
                    if (OnStoping != null)
                    {
                        OnStoping.BeginInvoke(this, null, null, null);
                    }
                    System.Threading.Thread.Sleep(10);
                }
            }).ContinueWith((x) =>
            {
                if (OnStop != null)
                {
                    OnStop.BeginInvoke(this, null, null, null);
                }
                _isStoped = true;
				switchTimer();
            });
        }

        /// <summary>
        /// 暂停所有线程
        /// </summary>
        public void Pause()
        {
			switchTimer();
            if (OnPause != null)
            {
                OnPause.BeginInvoke(this, null, null, null);
            }
        }

        /// <summary>
        /// 恢复执行
        /// </summary>
        public void Resume()
        {
			switchTimer();
            if (OnResume != null)
            {
                OnResume.BeginInvoke(this, null, null, null);
            }
        }

        /// <summary>
        /// 任务添加，面向用户
        /// </summary>
        /// <param name="t"></param>
        public void Push(T t)
        {
            AllTasks.Add(t);
            lock (obj)
            {
                t.SetContext(this);
                if (AllTasksAdded != null) AllTasksAdded(this, new TaskArgs(t,this,t.AsyncState));
                this._taskMax++;
            }
        }

        /// <summary>
        /// ITaskContext接口
        /// </summary>
        /// <param name="ac"></param>
        public void Invoke(Action ac)
        {
            Task.Factory.StartNew(ac, CancellationToken.None, TaskCreationOptions.None, UIThread);
        }


        
    }
}
