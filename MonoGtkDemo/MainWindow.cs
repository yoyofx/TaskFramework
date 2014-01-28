using System;
using Gtk;
using TaskFramework.Lib;
using System.Threading;
using System.ComponentModel.Composition;

namespace MonoGtkDemo
{
	public partial class MainWindow: Gtk.Window , IMainView
	{
		/// <summary>
		/// The main view controller.
		/// </summary>
		private readonly IMainViewContorller mainViewController = null;
		/// <summary>
		/// ListView的数据源，用于显示任务进度状态。
		/// </summary>
		private Gtk.ListStore ListStore = null;
		
		public MainWindow (IMainViewContorller controller) : base (Gtk.WindowType.Toplevel)
		{
			mainViewController = controller;
			LoadView (); //init views and events

			mainViewController.View = this;
			mainViewController.ParallelCount = Convert.ToInt32(spParallel.Value);
			mainViewController.Load ();    //controller loaded
		}

		/// <summary>
		/// Loads the view.
		/// </summary>
		public void LoadView()
		{
			// build main view , init controls and run application
			Build (); 
			//设置当前的同步上下文为GTK的同步上下文，用于构建TaskScheduler
			SynchronizationContext.SetSynchronizationContext (new GtkSynchronizationContext ());
			//添加列信息，用于显示任务状态列表
			addColumnsForTheTaskStateList (this.listView);
			this.btnStart.Clicked += btnStart_OnClick;
			this.btnStop.Clicked += btnStop_Click;
			this.spParallel.ValueChanged += nParallaCount_ValueChanged;
		}
		/// <summary>
		/// Raises the delete event event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="a">The alpha component.</param>
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			mainViewController.Exit ();
			Gtk.Application.Quit ();
			a.RetVal = true;
		}

		/// <summary>
		/// Buttons the start_ on click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void btnStart_OnClick (object sender, EventArgs e)
		{
			mainViewController.TaskQueueCount = Convert.ToInt32(spTotal.Value);
			mainViewController.ParallelCount = Convert.ToInt32(spParallel.Value);
			mainViewController.StartTaskTest ();

		}
		/// <summary>
		/// Buttons the stop_ click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void btnStop_Click(object sender, EventArgs e)
		{
			this.tvLog.Buffer.Text = "";
			this.ListStore.Clear ();
			mainViewController.StopTaskTest ();
		}
		/// <summary>
		///  the paralla count of value changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void nParallaCount_ValueChanged(object sender, EventArgs e)
		{
			mainViewController.ParallelCount = Convert.ToInt32(spParallel.Value);
		}

		/// <summary>
		/// 添加任务状态列表，列头信息
		/// </summary>
		/// <param name="tv">Tv.</param>
		private void addColumnsForTheTaskStateList(TreeView tv)
		{
			Gtk.TreeViewColumn idColumn = new Gtk.TreeViewColumn ();

			idColumn.Title = "Task ID";
			idColumn.Sizing = TreeViewColumnSizing.Fixed;
			idColumn.FixedWidth = 70;
			var idCell = new CellRendererText ();
			idColumn.PackStart (idCell, true);
			// Create a column for the song title
			Gtk.TreeViewColumn progressColumn = new Gtk.TreeViewColumn ();

			progressColumn.Title = "Progress Rate";
			progressColumn.Sizing = TreeViewColumnSizing.Fixed;
			progressColumn.FixedWidth = 600;
			var progressCell = new CellRendererProgress ();
			progressColumn.PackStart (progressCell, true);

			Gtk.TreeViewColumn msColumn = new Gtk.TreeViewColumn ();
			msColumn.Title = "Delay (ms)";
			var msCell = new CellRendererText ();
			msColumn.PackStart(msCell , true);
			// Add the columns to the TreeView
			tv.AppendColumn (idColumn);
			tv.AppendColumn (progressColumn);
			tv.AppendColumn (msColumn);

			idColumn.AddAttribute (idCell, "text", 0);
			progressColumn.AddAttribute (progressCell, "value", 1);
			msColumn.AddAttribute (msCell, "text", 2);


			// Create a model that will hold two strings - Artist Name and Song Title
			ListStore = new Gtk.ListStore (typeof (string), typeof (int),typeof(string));


			// Assign the model to the TreeView
			tv.Model = ListStore;


		}
		/// <summary>
		/// 设置界面中日志内容
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="log">Log.</param>
		public void SetViewLog(string log)
		{
			this.tvLog.Buffer.Text += log;
			//this.labRunning.Text = dc.WorkTaskCount.ToString();
		}
		/// <summary>
		/// 更新任务状态统计到界面中
		/// </summary>
		/// <param name="max">Max.</param>
		/// <param name="queue">Queue.</param>
		/// <param name="complated">Complated.</param>
		/// <param name="running">Running.</param>
		public void SetViewLabelState (string max ,string queue,string complated,string running)
		{

			if (!string.IsNullOrEmpty (max))
				this.labMax.Text = max;
			if (!string.IsNullOrEmpty (queue))
				this.labQueue.Text = queue;
			if (!string.IsNullOrEmpty (complated))
				this.labComplated.Text = complated;
			if (!string.IsNullOrEmpty (running))
				this.labRunning.Text = running;
		}

		/// <summary>
		/// 添加一行数据到任务状态列表中
		/// </summary>
		/// <param name="values">Values.</param>
		public void AddRowForTheListView (params object[] values)
		{
			ListStore.AppendValues(values);
		}

		/// <summary>
		/// 更新一条数据到任务状态列表中。
		/// </summary>
		/// <param name="rowidx">Rowidx.</param>
		/// <param name="columnidx">Columnidx.</param>
		/// <param name="value">Value.</param>
		public void UpdateRowForTheListView (int rowidx,int columnidx,object value)
		{
			TreeIter iter ;
			ListStore.GetIter(out iter,new TreePath(rowidx.ToString()));
			if (columnidx == 1) {
				ListStore.SetValue (iter, columnidx,Convert.ToInt32(value));
			}

		}

	}

}


