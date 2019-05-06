using System;
using System.Threading;

namespace Common
{
    public class BackWorker
    {
        private readonly int queryIntervalMilliSec;
        private readonly int interrupIntervalMilliSec;
        private readonly AutoResetEvent shutdownRequest;

        public void SetShutdown()
        {
            shutdownRequest.Set();
        }

        public void ResetShutdown()
        {
            shutdownRequest.Reset();
        }

        public BackWorker(string procName, Action process, int queryIntervalMilliSec, int interrupIntervalMilliSec = 1000)
        {
            this.queryIntervalMilliSec = queryIntervalMilliSec;
            this.interrupIntervalMilliSec = interrupIntervalMilliSec;

            shutdownRequest = new AutoResetEvent(false);

            var workingThread = new Thread(new ParameterizedThreadStart(Processing))
            {
                IsBackground = false,
                Name = procName
            };
            workingThread.Start(process);
        }

        private void Processing(object process)
        {
            Action work = (Action)process;
            while (true)
            {
                for (var i = 0; i < queryIntervalMilliSec / interrupIntervalMilliSec; ++i)
                    if (shutdownRequest.WaitOne(interrupIntervalMilliSec))
                        return; 

                    work.Invoke();
            }
        }
    }
}