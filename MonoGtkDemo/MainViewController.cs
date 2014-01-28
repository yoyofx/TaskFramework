using System;
using System.ComponentModel.Composition;
using TaskFramework.Lib;

namespace MonoGtkDemo
{
	public class MainViewController : IMainViewContorller
	{
		/// <summary>
		/// 任务执行队列
		/// </summary>
		private ParallelTaskQueue<SimpleParallelTask> dc = null;
		private int totalTask = 0; //总的执行次数
		/// <summary>
		/// 加载
		/// </summary>
		public void Load ()
		{
			dc = ParallelTaskQueue<SimpleParallelTask>.GetGlobalCollection(ParallelCount);
			dc.WorkTasksAdded += OnWorkTasksAdded;
			dc.AllTasksAdded += lookAtTaskEventState;
			dc.DoneTasksAdded += lookAtTaskEventState;
			dc.OnPause += lookAtTaskEventState;
			dc.OnStop += lookAtTaskEventState;
			dc.OnResume += lookAtTaskEventState;
			dc.OnStoping += lookAtTaskEventState;
		}

		/// <summary>
		/// 退出
		/// </summary>
		public void Exit()
		{
			dc.Stop ();
			dc.WorkTasksAdded -= OnWorkTasksAdded;
			dc.AllTasksAdded -= lookAtTaskEventState;
			dc.DoneTasksAdded -= lookAtTaskEventState;
			dc.OnPause -= lookAtTaskEventState;
			dc.OnStop -= lookAtTaskEventState;
			dc.OnResume -= lookAtTaskEventState;
			dc.OnStoping -= lookAtTaskEventState;
			dc = null;
		}

		/// <summary>
		/// Starts the task test.
		/// </summary>
		public void StartTaskTest ()
		{
			dc.SetParallaCount(ParallelCount);

			for (int i = 0; i < TaskQueueCount; i++)
			{
				SimpleParallelTask dt = new SimpleParallelTask(computeProgress, totalTask);
				dc.Push(dt);
				totalTask++;
			}
		}

		/// <summary>
		/// 停止任务测试
		/// </summary>
		public void StopTaskTest()
		{
			dc.Stop();
		
			totalTask = 0;
		}

		/// <summary>
		///The task for Computes the progress.
		/// </summary>
		/// <param name="param">Parameter.</param>
		void computeProgress(object param)
		{
			var args = param as TaskArgs;

			int delay = new Random().Next(200);
			args.Context.Invoke(() => {
				this.View.AddRowForTheListView(args.Task.Id.ToString(),"0",delay.ToString());
			});

			for (int i = 1; i < 100; i++)
			{
				if (args.Task.CancellationToken.IsCancellationRequested)
					break;

				System.Threading.Thread.Sleep(delay);
				args.Context.Invoke(() => {

					this.View.UpdateRowForTheListView(Convert.ToInt32(args.Task.AsyncState),1,i);

				});
			}
		}


		#region Task Event
		/// <summary>
		/// Looks the state of the at task event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void lookAtTaskEventState(object sender, EventArgs e)
		{
			dc.GetContext().Invoke(() => {
				this.View.SetViewLabelState( dc.AllTaskCount.ToString(),dc.TaskMax.ToString(),dc.DoneTaskCount.ToString(),dc.WorkTaskCount.ToString());
			});
		}
		/// <summary>
		/// 任务添加事件
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void OnWorkTasksAdded(object sender, TaskArgs e)
		{
			string msg = string.Format( "Task:{0} added.\r\n", e.Task.Id.ToString());
	
			dc.Invoke(() => {
				this.View.SetViewLog(msg);
			});
		}


		#endregion




		#region Property
		private int _parallelCount = 0;
		/// <summary>
		/// 总的并发数量
		/// </summary>
		/// <value>The parallel count.</value>
		public int ParallelCount{ 
			set{
				if (dc != null)
					dc.SetParallaCount (value);
				else
					_parallelCount = value;
			 } 
			get{ return _parallelCount; } 
		}
		/// <summary>
		/// 任务队列数量
		/// </summary>
		/// <value>The task count.</value>
		public int TaskQueueCount {set;get;}
		/// <summary>
		/// 视图接口
		/// </summary>
		/// <value>The view.</value>
		public IMainView View {set; get; }



		#endregion


	}


}

