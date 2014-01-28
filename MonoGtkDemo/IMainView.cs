using System;

namespace MonoGtkDemo
{
	public interface IMainView
	{
		/// <summary>
		/// Loads the view.
		/// </summary>
		void LoadView();
		/// <summary>
		/// 添加一行数据到任务状态列表中
		/// </summary>
		/// <param name="values">Values.</param>
		void AddRowForTheListView (params object[] values);
		/// <summary>
		/// 更新一条数据到任务状态列表中。
		/// </summary>
		/// <param name="rowidx">Rowidx.</param>
		/// <param name="columnidx">Columnidx.</param>
		/// <param name="value">Value.</param>
		void UpdateRowForTheListView (int rowidx,int columnidx,object value);
		/// <summary>
		/// 设置界面中日志内容
		/// </summary>
		/// <param name="msg">Message.</param>
		void SetViewLog (string msg);
		/// <summary>
		/// 更新任务状态统计到界面中
		/// </summary>
		/// <param name="max">Max.</param>
		/// <param name="queue">Queue.</param>
		/// <param name="complated">Complated.</param>
		/// <param name="running">Running.</param>
		void SetViewLabelState (string max ,string queue,string complated,string running);
	}
}

