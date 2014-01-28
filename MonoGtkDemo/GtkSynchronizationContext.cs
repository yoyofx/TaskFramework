using System;
using System.Threading;

namespace MonoGtkDemo
{
	public class GtkSynchronizationContext : SynchronizationContext
	{
		public override void Send (SendOrPostCallback callback, object state)
		{
			var signal = new ManualResetEvent (false);

			Post (stateObject => {

				callback(stateObject);
				signal.Set();

			}, state);

			signal.WaitOne ();
		}

		public override void Post (SendOrPostCallback callback, object state)
		{
			Gtk.Application.Invoke (delegate { callback(state);});
		}

	}
}

