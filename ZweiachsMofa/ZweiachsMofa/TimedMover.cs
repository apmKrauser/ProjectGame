using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace ZweiachsMofa
{
    class TimedMover : IDisposable
    {
        private const uint FPS = 50;
        //internal Boolean wobble = false;
        //private GFXContainer Gfx;
        private double sinex = 0;
        private Point CurrPos;
        private Size iniSize;
        DispatcherTimer myTimer = new DispatcherTimer();  // Threading.Timer wg. WPF
        public delegate void myEventHandler(object sender, EventArgs e);
        internal event myEventHandler Crash;
        internal event Action UpdateEvent;
        private bool crashed;



        public TimedMover(/*GFXContainer _Gfx*/)
        {
            //Gfx = _Gfx;

        }

        public void Start()
        {
            myTimer.Tick += new EventHandler(Update);
            // myTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000.0 / FPS));
            myTimer.Interval = TimeSpan.FromMilliseconds((1000.0 / FPS));
            //myTimer.Start();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            UpdateEvent();
        }

        public void Stop()
        {
            try
            {
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
                myTimer.Stop();
            }
            catch (Exception e)
            {
                Debug.WriteLine("=> TimedMover :: {0}", e.ToString());
            }
            
        }

        private void Update(Object myObject, EventArgs myEventArgs)
        {
            //Application.DoEvents();
            //Thread.Yield(); // warten, andere  threads kurz machen lassen
            if (UpdateEvent != null)
            {
                UpdateEvent();
            }
        }

        private void OnCrashEvent()
        {
            //EventHandler handler = Crash;
            if (Crash != null)
            {
                Crash(this, EventArgs.Empty);
            }
        }


        public void Dispose()
        {
            this.Stop();
        }

        ~TimedMover()
        {
            this.Dispose();
        }
    }
}
