using System;

namespace MonoGtkDemo
{
	public interface IMainViewContorller
	{
		/// <summary>
		/// 总的并发数量
		/// </summary>
		/// <value>The parallel count.</value>
		int ParallelCount{ set; get; }

		/// <summary>
		/// 任务队列数量
		/// </summary>
		/// <value>The task count.</value>
		int TaskQueueCount{set;get;}
		/// <summary>
		/// 视图接口
		/// </summary>
		/// <value>The view.</value>
		IMainView View{ set;get;}
		/// <summary>
		/// 加载
		/// </summary>
		void Load();
		/// <summary>
		/// 退出
		/// </summary>
		void Exit();
		/// <summary>
		/// Starts the task test.
		/// </summary>
		void StartTaskTest();
		/// <summary>
		/// 停止任务测试
		/// </summary>
		void StopTaskTest();

	}
}

