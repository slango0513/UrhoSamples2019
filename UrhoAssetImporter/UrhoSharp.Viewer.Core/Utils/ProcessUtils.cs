using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Threading;

namespace UrhoSharp.Viewer.Core.Utils
{
    public static class ProcessUtils
    {
        public static bool StartCancellableProcess(string file, string args, CancellationToken token)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = file,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args,
            };
            var process = new Process { StartInfo = processInfo };
            process.Start();

            using (var waitHandle = new SafeWaitHandle(process.Handle, false))
            {
                using (var processFinishedEvent = new ManualResetEvent(false))
                {
                    processFinishedEvent.SafeWaitHandle = waitHandle;
                    int index = WaitHandle.WaitAny(new[] { processFinishedEvent, token.WaitHandle });
                    if (index == 1)
                    {
                        process.Kill();
                        Debug.WriteLine($"Process {file} {args} is terminated");
                        return false;
                    }
                }
            }
            return !token.IsCancellationRequested;
        }
    }
}
