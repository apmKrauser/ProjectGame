using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    public class FrameUpdateEventArgs : EventArgs
    {
        public double ElapsedMilliseconds { get; private set; }
        public double CurrentFPS { get; private set; }
        public FrameUpdateEventArgs(long _elapsedMilliseconds) : this((double)_elapsedMilliseconds) { }
        public FrameUpdateEventArgs(double _elapsedMilliseconds)
            : base()
        {
            ElapsedMilliseconds = (_elapsedMilliseconds < 1) ? 1f : _elapsedMilliseconds;
            CurrentFPS = 1000.0 / ElapsedMilliseconds;
        }
    }
}

