using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SimpleGraphicsLib
{
    [DataContract]
    public class CarObject : SpriteObjectElastic, IHasSeperateAnimationEvent
    {

        public ParticleSystem<SmokeParticle, SmokeParticle.ParticleConfig> PSAuspuff;

        [DataMember]
        public int AuspuffMaxParticles { get; set; }

        [DataMember]
        public SmokeParticle.ParticleConfig AuspuffPSConfig { get; set; }


        public override bool Animated
        {
            get
            {
                return base.Animated;
            }
            set
            {
                base.Animated = value;
                if (PSAuspuff != null)
                    PSAuspuff.Animated = Animated;
            }
        }


        private Vector _posAuspuff = new Vector(0,0);

        private Vector _posAuspuffPixel = new Vector(0,0);

        [DataMember]
        public Vector PosAuspuff
        {
            get { return _posAuspuff; }
            set { _posAuspuff = value; }
        }

        private double _auspuffGenRate;

        [DataMember]
        public double AuspuffGenerationRate
        {
            get { return _auspuffGenRate; }
            set 
            { 
                _auspuffGenRate = value;
                if (PSAuspuff != null)
                    PSAuspuff.GenerationRate = _auspuffGenRate;
            }
        }

        [DataMember]
        public override bool FlipHorizontal
        {
            get
            { return base.FlipHorizontal; }
            set
            {
                if (value != FlipHorizontal)
                {
                    AuspuffPSConfig.GroupVelocity = new Vector(-AuspuffPSConfig.GroupVelocity.X, AuspuffPSConfig.GroupVelocity.Y);
                    PosAuspuff = new Vector(1 - PosAuspuff.X, PosAuspuff.Y);
                }
                base.FlipHorizontal = value;
            }
        }
        

        public override GFXContainer ParentContainer
        {
            get { return _parentContainer; }
            set
            {
                _parentContainer = value;
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
               OnCreate();
        }

        [OnDeserializing]
        protected void OnDeserializing(StreamingContext context)
        {
            OnCreate();
        }

        private void OnCreate()
        {
            //Animated = AnimatedByDefault;
            IsDeformable = true;
            IsObstacle = true;
            CanCollide = true;
            AuspuffMaxParticles = 70;
            AuspuffGenerationRate = 15; // particles/sec
            _posAuspuffPixel = new Vector(0,0);
            PosAuspuff = new Vector(-0.1, 0.5);
            // Provide an existing config object at deserialization
            AuspuffPSConfig = new SmokeParticle.ParticleConfig();
            AuspuffPSConfig.AverageLifetime = 3000; // ms
            AuspuffPSConfig.GroupVelocity = new Vector(-100, -35);
            AuspuffPSConfig.GroupSpread = 5; // isotropic speed
            AuspuffPSConfig.AirDrag = 0.6;
            AuspuffPSConfig.EmmissionArea = new Rect(0, 0, 30, 30);
            AuspuffPSConfig.AppearanceSpread = 0.05;
            AuspuffPSConfig.SubParticleSpread = 20;

            AuspuffPSConfig.ColorFrom = Color.FromArgb(230, 30, 30, 30);
            AuspuffPSConfig.ColorTo = Color.FromArgb(0, 108, 108, 108);
            AuspuffPSConfig.RadiusFrom = 4;
            AuspuffPSConfig.RadiusTo = 50;
            AuspuffPSConfig.BlurFrom = 7;
            AuspuffPSConfig.BlurTo = 20;
            AddAnimation(new AnimationWobble(0.8, 0.005), "Wobble");
        }

        protected override void init()   // bei setparent aufrufen?  artikel über virtual in ctor aufrufen lesen
        {
            //DrawingVisual vis = new DrawingVisual();
            //RenderOptions.SetBitmapScalingMode(vis, BitmapScalingMode.Fant);
            //Visuals.Add(vis);
            //RegisterDrawingVisual(vis);
            //AddAnimation(new AnimationLinearTranslation(), "LinMove");
            PSAuspuff = new ParticleSystem<SmokeParticle, SmokeParticle.ParticleConfig>(0, AuspuffMaxParticles, false, AuspuffPSConfig);
            PSAuspuff.Animated = Animated;
            PSAuspuff.GenerationRate = AuspuffGenerationRate; // particles/sec

            _parentContainer.AddObject(PSAuspuff);
            //Ps2.Start();
            base.init();
        }

        protected override void DrawShapeAndMarkers(DrawingContext dc)
        {
            base.DrawShapeAndMarkers(dc);
           // dc.DrawRectangle(null, new Pen(Brushes.Black, 2), rectangle: new Rect(Shape.Location + (_parent.DrawingOffset * ScrollScaling), Shape.Size));
           // dc.DrawEllipse(null, new Pen(Brushes.Red, 2), (Point)(Position + (_parent.DrawingOffset * ScrollScaling)), 5, 5);
            _posAuspuffPixel = new Vector(Shape.Width * _posAuspuff.X, Shape.Height * _posAuspuff.Y);
            _posAuspuffPixel += (Vector)Shape.Location;
            dc.DrawRectangle(null, new Pen(Brushes.Green, 2), new Rect((Point)(_posAuspuffPixel + (_parentContainer.DrawingOffset * ScrollScaling)), AuspuffPSConfig.EmmissionArea.Size));
        }

        public override void Animation_Update(object sender, FrameUpdateEventArgs e)
        {
            base.Animation_Update(sender, e);
            _posAuspuffPixel = new Vector(Shape.Width * _posAuspuff.X, Shape.Height * _posAuspuff.Y);
            _posAuspuffPixel += (Vector)Shape.Location;
            PSAuspuff.Config.EmmissionArea = new Rect(_posAuspuffPixel.X, _posAuspuffPixel.Y, PSAuspuff.Config.EmmissionArea.Width, PSAuspuff.Config.EmmissionArea.Height);
        }

    }
}
