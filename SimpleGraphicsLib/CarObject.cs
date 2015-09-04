using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGraphicsLib
{
    [DataContract]
    public class CarObject : SpriteObjectElastic, IHasSeperateAnimationEvent
    {

        public ParticleSystem<SmokeParticle, SmokeParticle.ParticleConfig> PSAuspuff;

        [DataMember]
        public int AuspuffMaxParticles { get; set; }

        [DataMember]
        private Vector _posAuspuff = new Vector(0,0);

        private Vector _posAuspuffPixel = new Vector(0,0);

        public Vector PosAuspuff
        {
            get { return _posAuspuff; }
            set { _posAuspuff = value; }
        }

        public override GFXContainer Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (value == null)
                {
                    UnregisterAllVisuals();
                    PSAuspuff.Dispose();
                } else 
                    init();
            }
        }
        public CarObject() : this("") { }

        public CarObject(string name) : base(name) 
        {
            AuspuffMaxParticles = 70;
        }

        [OnDeserializing]
        protected void OnDeserializing(StreamingContext context)
        {
            
        }

        protected override void init()   // bei setparent aufrufen?  artikel über virtual in ctor aufrufen lesen
        {
            //DrawingVisual vis = new DrawingVisual();
            //RenderOptions.SetBitmapScalingMode(vis, BitmapScalingMode.Fant);
            //Visuals.Add(vis);
            //RegisterDrawingVisual(vis);
            //AddAnimation(new AnimationLinearTranslation(), "LinMove");
            PSAuspuff = new ParticleSystem<SmokeParticle, SmokeParticle.ParticleConfig>(0, AuspuffMaxParticles, false);
            PosAuspuff = new Vector(-0.1, 0.5);
            PSAuspuff.Config.AverageLifetime = 3000; // ms
            PSAuspuff.GenerationRate = 15; // particles/sec
            PSAuspuff.Config.GroupVelocity = new Vector(-100, -35);
            PSAuspuff.Config.GroupSpread = 5; // isotropic speed
            PSAuspuff.Config.AirDrag = 0.6;
            PSAuspuff.Config.EmmissionArea = new Rect(0, 0, 1, 1);
            PSAuspuff.Config.AppearanceSpread = 0.05;
            PSAuspuff.Config.SubParticleSpread = 20;
            _parent.AddObject(PSAuspuff);
            //Ps2.Start();
            base.init();
        }

        public virtual void Animation_Update(object sender, FrameUpdateEventArgs e)
        {
            _posAuspuffPixel = new Vector(Shape.Width * _posAuspuff.X, Shape.Height * _posAuspuff.Y);
            _posAuspuffPixel += (Vector)Shape.Location;
            PSAuspuff.Config.EmmissionArea = new Rect(_posAuspuffPixel.X, _posAuspuffPixel.Y, 30, 30);
        }

    }
}
