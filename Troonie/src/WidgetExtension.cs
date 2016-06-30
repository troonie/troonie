using System;
using Gtk;

namespace Troonie
{
	public static class WidgetExtension
	{
		public static void DestroyAll(this Container container)
		{
			foreach (Widget child in container.Children) {
				if (child is Container) {
					DestroyAll (child as Container);
				} else {
					child.Destroy ();
				}
			}

			container.Destroy ();
		}
	}
}

