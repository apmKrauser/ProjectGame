﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;

namespace SimpleGraphicsLib
{
    [DataContract]
    [KnownType("GetDerivedTypes")]
    public class GFXAnimation : IDisposable, IGFXAnimation
    {
        
        private static double PixelsPerGameMeter = 1;

        private TimingSource.Sources _timingSource = TimingSource.Sources.Manual;
        private GFXContainer GFXCont = null;

        public bool IsActive { get; set; }

        public String TypeName
        {
            get { return this.GetType().Name; }
            set { } // empty setter in order to show up in property inspector
        }

        protected GFXAnimation()
        {
            IsActive = true;
        }

        // tiggered by GFXContainer
        protected GFXAnimation(TimingSource.Sources _tsrc)
        {
            SetTimingSource(_tsrc);
            IsActive = true;
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
            if (IsActive)
                Update_Active(sender, e);
        }

        public virtual void Update_Active(object sender, FrameUpdateEventArgs e)
        {

        }

        public virtual void Start()
        {
            IsActive = true;
        }
        public virtual void Stop()
        {
            IsActive = false;
        }

        public static void CalibratePixelSpeed()
        {
            throw new System.NotImplementedException();
        }

        public static Vector NormToPixelSpeed(Vector _normSpeed)
        {
            return _normSpeed * PixelsPerGameMeter;
        }

        public static IEnumerable<Type> GetDerivedTypes()
        {
            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsSubclassOf(typeof(AnimationRigidBody))
                        select t;
            return types;
        }

    }
}
