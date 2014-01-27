using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{
	Gtk.ListStore ListStore = null;
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		addColumns (this.listView);
		this.btnStart.Clicked += btnStart_OnClick;
	}

	private void btnStart_OnClick (object sender, EventArgs e)
	{

		TreeIter iter ;
		ListStore.GetIter(out iter,new TreePath("1"));

		ListStore.SetValue (iter, 1, 60);


	}







	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	private void addColumns(TreeView tv)
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
		msColumn.Title = "ms";
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

		ListStore.AppendValues ("task 1",50,"30");
		ListStore.AppendValues ("task 2",50,"30");
		ListStore.AppendValues ("task 3",50,"30");
		// Assign the model to the TreeView
		tv.Model = ListStore;


	}

}



