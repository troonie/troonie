using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Troonie_Lib
{
	public static class MemoryReducer
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetProcessHeap();

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool HeapLock(IntPtr heap);

		[DllImport("kernel32.dll")]
		internal static extern uint HeapCompact(IntPtr heap, uint flags);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool HeapUnlock(IntPtr heap);

		[DllImport("kernel32.dll")]
		private static extern bool SetProcessWorkingSetSize(IntPtr hProcess,
			int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

		[DllImport("kernel32.dll")]
		private static extern bool SetProcessWorkingSetSize(IntPtr hProcess,
			UIntPtr dwMinimumWorkingSetSize, UIntPtr dwMaximumWorkingSetSize);
		
		/// <summary>
		/// Reduces the memory usage.
		/// <remarks>http://stackoverflow.com/questions/263234/net-minimize-to-tray-and-minimize-required-resources</remarks>
		/// </summary>
		public static void ReduceMemoryUsage(bool useNewVersion)
		{
			if (useNewVersion)
			{
				// collect objects
				GC.Collect();

				// wait for finalizer
				GC.WaitForPendingFinalizers();

				// collect finalized objects
				GC.Collect();

				if (!Constants.I.WINDOWS) {
					return;
				}

				// TODO: the following code is bad and normally should never be called

				//// reduce the process' working size
				//EmptyWorkingSet(Process.GetCurrentProcess().Handle);

				// trim the process' working size
				SetProcessWorkingSetSize(
					Process.GetCurrentProcess().Handle,
					(UIntPtr)0xFFFFFFFF,
					(UIntPtr)0xFFFFFFFF);

				IntPtr heap = GetProcessHeap();

				// compact the process' heap
				if (HeapLock(heap))
				{
					try
					{
						if (HeapCompact(heap, 0) == 0)
						{
							// ignore error condition 
						}
					}
					finally
					{
						HeapUnlock(heap);
					}
				}                
			}
			else
			{
				try
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
					if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					{
						SetProcessWorkingSetSize(
							Process.GetCurrentProcess().Handle, -1, -1);
					}
				}
				catch { }
			}
		}
	}
}

