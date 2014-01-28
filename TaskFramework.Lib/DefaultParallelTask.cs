using System;

namespace TaskFramework.Lib
{
	/// <summary>
	/// 用户自定义并行任务抽象类
	/// </summary>
	public abstract class DefaultParallelTask : SimpleParallelTask , IParallelTask
	{
		public DefaultParallelTask ()
			:base(null,null)
		 {
			this._actionWithObject = ProressProxyMethod;
		 }

		private void ProressProxyMethod(object state)
		{
			Proress ((TaskArgs)state);
		}

		/// <summary>
		/// 并行任务处理方法
		/// </summary>
		/// <param name="args">任务上下文</param>
		public abstract void Proress(TaskArgs args);


	}
}

