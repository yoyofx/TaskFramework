using System;
using Gtk;

namespace MonoGtkDemo
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow (new MainViewController());

			win.Show ();
			Application.Run ();
		}
	}
}
