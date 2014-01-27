using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		buildListView();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}


	protected void RunTestClicked(object sender, EventArgs e)
	{



	}
	#region private methods

	void buildListView()
	{
		Gtk.TreeViewColumn colId = new TreeViewColumn();
		colId.Title = "Task Id";

		Gtk.TreeViewColumn colProgress = new TreeViewColumn();
		colProgress.Title = "Progress";

		Gtk.TreeViewColumn colDelay = new TreeViewColumn();
		colDelay.Title = "Delay(ms)";

		listView.AppendColumn(colId);
		listView.AppendColumn(colProgress);
		listView.AppendColumn(colDelay);

		Gtk.ListStore mlist = new ListStore(typeof(string), typeof(string),typeof(string));
		listView.Model = mlist;
	}

	#endregion
}
