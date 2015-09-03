using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SimpleGraphicsLib
{
    public class GFXAnimation : IDisposable, IGFXAnimation
    {
        
        private static double PixelsPerGameMeter = 1;

        TimingSource.Sources _timingSource = TimingSource.Sources.Manual;
        GFXContainer GFXCont = null;

        protected GFXAnimation()
        {
        }

        // tiggered by GFXContainer
        protected GFXAnimation(TimingSource.Sources _tsrc)
        {
            SetTimingSource(_tsrc);
        }

        protected GFXAnimation(GFXContainer _GFXContainer)
        {
            SetTimingSource(_GFXContainer);
        }


        ~GFXAnimation()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
           // unhook Update or stop timer
            // consider updated by gfx container
            switch (_timingSource)
            {
                case TimingSource.Sources.CompositionTargetRendering:
                    TimingSource.TimingEvents.UpdateCTargetRendering -= Update;
                    break;
                case TimingSource.Sources.DispatchTimer:
                    TimingSource.TimingEvents.UpdateDispatchTimer -= Update;
                    break;
                case TimingSource.Sources.SeparateThread:
                    TimingSource.TimingEvents.UpdateSeparateThread -= Update;
                    break;
                default:
                    break;
            }
            if (GFXCont != null)
            {
                GFXCont.UpdateAnimation -= Update;
            }
        }

        public void SetTimingSource(GFXContainer _GFXContainer)
        {
            GFXCont = _GFXContainer;
            GFXCont.UpdateAnimation += Update;
        }

        public void SetTimingSource(TimingSource.Sources _tsrc)
        {
            _timingSource = _tsrc;
            switch (_timingSource)
            {
                case TimingSource.Sources.CompositionTargetRendering:
                    TimingSource.TimingEvents.UpdateCTargetRendering += Update;
                    break;
                case TimingSource.Sources.DispatchTimer:
                    TimingSource.TimingEvents.UpdateDispatchTimer += Update;
                    break;
                case TimingSource.Sources.SeparateThread:
                    TimingSource.TimingEvents.UpdateSeparateThread += Update;
                    break;
                default:
                    break;
            }

        }


        public virtual void Update(object sender, FrameUpdateEventArgs e)
        {

        }

        public static void CalibratePixelSpeed()
        {
            throw new System.NotImplementedException();
        }

        public static Vector NormToPixelSpeed(Vector _normSpeed)
        {
            return _normSpeed * PixelsPerGameMeter;
        }

    }
}
