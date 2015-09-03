using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace SimpleGraphicsLib
{
    // Thread safe Singleton
    public class TimingSource : IDisposable
    {
        private const double CTargetRenderMaxFPS = 100.0;
        private const double DispatcherFPS = 60.0;
        private const double SeperateThreadFPS = 35.0;

        private Mutex MutexDispTimer = new Mutex();
        private Mutex MutexSepTimer = new Mutex();

        public enum Sources
        {
            Manual,
            CompositionTargetRendering, 
            DispatchTimer,
            SeparateThread
        }

        public event Action<Object, FrameUpdateEventArgs> UpdateCTargetRendering;
        public event Action<Object, FrameUpdateEventArgs> UpdateDispatchTimer;
        public event Action<Object, FrameUpdateEventArgs> UpdateSeparateThread; // ensure to use thread safe operations
        
        private static volatile TimingSource instance;
        private static object syncRoot = new Object();
        private DispatcherTimer DispTimer = new DispatcherTimer();
        private System.Threading.Timer ThreadTimer;
        private Stopwatch swRendering = new Stopwatch();
        private Stopwatch swDispatch = new Stopwatch();
        private Stopwatch swThread = new Stopwatch();


        public static TimingSource TimingEvents
        {
            get 
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new TimingSource();
                    }
                }
                return TimingSource.instance; 
            }
        }  

        private TimingSource()
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            DispTimer.Tick += DispTimer_Tick;
            DispTimer.Interval = TimeSpan.FromMilliseconds((1000.0 / DispatcherFPS));
            DispTimer.Start();
            ThreadTimer = new System.Threading.Timer(ThreadTimer_Callback, this, 0, (int)Math.Round(1000.0 / SeperateThreadFPS));
            swRendering.Start();
            swDispatch.Start();
            swThread.Start();
        }

        public void Dispose()
        {
            DispTimer.Stop();
            ThreadTimer.Dispose();
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        ~TimingSource()
        {
            this.Dispose();
        }


        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // do not invoke faster than MaxFPS
            long elapsed = swRendering.ElapsedMilliseconds;
            if (elapsed >= (1000.0 / CTargetRenderMaxFPS))
            {
                swRendering.Restart();
                if (UpdateCTargetRendering != null) UpdateCTargetRendering(this, new FrameUpdateEventArgs(elapsed));
            }
        }

        void DispTimer_Tick(object sender, EventArgs e)
        {
            MutexDispTimer.WaitOne();
            long elapsed = swDispatch.ElapsedMilliseconds;
            swDispatch.Restart();
            if (UpdateDispatchTimer != null) UpdateDispatchTimer(this, new FrameUpdateEventArgs(elapsed));
            MutexDispTimer.ReleaseMutex();
        }

        private void ThreadTimer_Callback(object StateObj)
        {
            MutexSepTimer.WaitOne();
            long elapsed = swThread.ElapsedMilliseconds;
            swThread.Restart();
            if (UpdateSeparateThread != null) UpdateSeparateThread(this, new FrameUpdateEventArgs(elapsed));
            MutexSepTimer.ReleaseMutex();
        }


    }
}
