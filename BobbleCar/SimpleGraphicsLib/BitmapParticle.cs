using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace SimpleGraphicsLib
{
    public class BitmapParticle : GFXParticle, IHasSeperateAnimationEvent, IRigidBody
    {

        new public class ParticleConfig : GFXParticle.ParticleConfig, IParticleConfigurable
        {
            public Color ColorFrom { get; set; }
            public Color ColorTo { get; set; }
            public Vector SizeFrom { get; set; }
            public Vector SizeTo { get; set; }
            public double BlurFrom { get; set; }
            public double BlurTo { get; set; }
            public double AirDrag { get; set; }
            public double AppearanceSpread { get; set; }
            public BitmapImage Bmp { get; set; }
        }

        private ParticleConfig _config;

        public override GFXParticle.ParticleConfig BaseConfig { get { return _config; }  }


        #region Properties

        // private object _AnimationSem = new object();  // semaphore

       // protected double GrowthSpeed;

        public double AirDrag
        {
            get { return _config.AirDrag; }
            set { _config.AirDrag = value; }
        }
        
        protected AnimationLinearTranslation AnimLinTrans;
        protected AnimationAirDrag AnimAirDrag;

        protected  double _blurEffectRadius;

        public Vector NormSpeed { get; set; }

        private object _positionSync = new object();
        private Vector _position;
        public Vector Position
        {
            get { lock (_positionSync) return _position; }
            set { lock (_positionSync) _position = value; }
        }

        public bool IsObstacle { get; set; }
        public bool IsMovable { get; set; }
        public bool CanCollide { get; set; }

        public double Weight { get; set; }

        public double Angle { get; set; }

        public bool IsGrounded { get; set; }


        public virtual Vector SizeV
        {
            get { return _size; }
            set
            {
                _size = value;
                _centerOfMassAbs = new Vector(SizeV.X * CenterOfMass.X, SizeV.Y * CenterOfMass.Y);
            }
        }

        public Rect Shape
        {
            get { return new Rect(Position.X, Position.Y, SizeV.X, SizeV.Y); }
        }

        public Vector CenterOfMass  // (0,0)..(1,1)
        {
            get { return _centerOfMass; }
            set
            {
                _centerOfMass = value;
                _centerOfMassAbs = new Vector(SizeV.X * _centerOfMass.X, SizeV.Y * _centerOfMass.Y);
            }
        }

        public virtual double BlurEffectRadius
        {
            get { return _blurEffectRadius; }
            set
            {
                    _blurEffectRadius = value < 0 ? 0 : value;
                    //((BlurEffect)DVisual.Effect).Radius = _blurEffectRadius;
            }
        }

        public Vector PixelSpeed
        {
            get
            {
                return GFXAnimation.NormToPixelSpeed(NormSpeed);
            }
        }

        public virtual Vector Acceleration { get; set; }
        public virtual Vector DRadius
        {
            set { SizeV = value; }
        }

        public virtual Color ParticleColor { get; set; }

        private double spreadedLifetime = 0;
        private Vector _centerOfMass;
        private Vector _centerOfMassAbs;
        private Vector _size;

        #endregion

        public BitmapParticle()
        {
            IsMovable = true;
            IsObstacle = false;
            CanCollide = false;
            BlurEffect myBlurEffect = new BlurEffect();
            myBlurEffect.Radius = BlurEffectRadius;
            DVisual.Effect = myBlurEffect;
            AnimLinTrans = new AnimationLinearTranslation(this);
            AnimAirDrag = new AnimationAirDrag(this);
        }

        public override void init()
        {
            CenterOfMass = new Vector(0.5, 0.5);
            _config = new ParticleConfig();
            //Debug.WriteLine("=> Particle Init = "+ BaseConfig.GetType().FullName);
            _config.ColorFrom = Color.FromArgb(255, 30, 30, 30);
            _config.ColorTo = Color.FromArgb(128, 128, 128, 128);
            _config.SizeFrom = new Vector(15,15);
            _config.SizeTo = new Vector(60, 60);
            _config.BlurFrom = 2;
            _config.BlurTo = 20;
            _config.AppearanceSpread = 0.1;
        }


        public override void init(GFXParticle.ParticleConfig _cfg)
        {
            _config = (_cfg as ParticleConfig);
        }
        
        public override void Spawn()
        {
            Spawn(_config);
        }
        public virtual void Spawn(ParticleConfig config)  // _config ist null, wenn von Subklasse aufgerufen
        {
            CenterOfMass = new Vector(0.5, 0.5);
            _config.Bmp = Properties.Resources.smoketest.ToMediaBitmap();
            if (config == null) config = _config;
            double fktSpread = (ThisLifetime - TimeAlive) * config.AppearanceSpread * 2;
            Random rnd = new Random();
            Vector spreadV = new Vector(rnd.NextDouble()-0.5, rnd.NextDouble()-0.5);
            if (spreadV.Length > 0)
                spreadV.Normalize();
            NormSpeed = config.GroupVelocity + (config.GroupSpread * spreadV);
            Position = new Vector(rnd.Next((int)config.EmmissionArea.Left, (int)config.EmmissionArea.Right),
                                  rnd.Next((int)config.EmmissionArea.Top, (int)config.EmmissionArea.Bottom));
            ThisLifetime = config.AverageLifetime + (config.AverageLifetime * ((rnd.NextDouble()-0.5) * config.LifetimeSpread * 2));
            fktSpread = (rnd.NextDouble() - 0.5) * fktSpread;
            spreadedLifetime = TimeAlive * fktSpread;
            // Todo: @debug
            //Debug.WriteLine("=> new Particle Pos:" + Position + " Speed:" + NormSpeed);
            base.Spawn();
        }

        // info: mit sealed override kann überschreiben verhindert werden
        public override void Animation_Update(object sender, FrameUpdateEventArgs e) 
        {
            //TimeAlive += e.ElapsedMilliseconds;  // here because Frame_Update can be dropped
           // try
          //  {
           //     mixupProperties();
            //}
            //catch (Exception ex)
            //{
                
            //    //throw;
            //}
            if (IsAlive)
            {
                AnimLinTrans.Update(this, e);
                AnimAirDrag.Update(this, e);
            }
        }


        public override void Frame_Update(object sender, FrameUpdateEventArgs e)
        {
            TimeAlive += e.ElapsedMilliseconds;
            mixupProperties();
            if (IsAlive)
            {
                if (TimeAlive > ThisLifetime)
                {
                    IsAlive = false;
                    TimeAlive = 0;
                    //Debug.WriteLine("=> Particle gone.");
                    using (DrawingContext dc = DVisual.RenderOpen()) { }
                } else
                    DrawParticle();                
            }
        }

        protected virtual void mixupProperties()
        {
            //double fktAvg = (TimeAlive / ThisLifetime);
            ParticleConfig config = _config;
            double fkt = 1;
            double a, r, g, b;
            a = r = g = b = 0;
            // randomize a little bit
            fkt = (TimeAlive + spreadedLifetime) / ThisLifetime;
            if (fkt < 0) fkt = 0;
            if (fkt > 0.999) fkt = 0.999;
            a = config.ColorFrom.A + ((config.ColorTo.A - config.ColorFrom.A) * fkt);
            r = config.ColorFrom.R + ((config.ColorTo.R - config.ColorFrom.R) * fkt);
            g = config.ColorFrom.G + ((config.ColorTo.G - config.ColorFrom.G) * fkt);
            b = config.ColorFrom.B + ((config.ColorTo.B - config.ColorFrom.B) * fkt);
            // nihct Threadsafe
                ParticleColor = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
                BlurEffectRadius = config.BlurFrom + ((config.BlurTo - config.BlurFrom) * fkt * 0.4);
                SizeV = config.SizeFrom + ((config.SizeTo - config.SizeFrom) * fkt * 1);
        }

        protected virtual void DrawParticle()
        {
            using (DrawingContext dc = DVisual.RenderOpen())
            {
                if (_config.Bmp != null)
                {
                    ((BlurEffect)DVisual.Effect).Radius = _blurEffectRadius;
                    dc.PushTransform(new TranslateTransform(Position.X + (DrawingOffset.X ), Position.Y + DrawingOffset.Y));
                    dc.PushTransform(new RotateTransform(Angle));
                    //if (FlipHorizontal)
                    //    dc.PushTransform(new ScaleTransform(-1, 1));
                    // dc.DrawImage(_config.Bmp, new Rect((Point)(_deformationPos - _centerOfMassAbs), SizeV + _deformationSize));
                    dc.DrawImage(_config.Bmp, new Rect((Point)( - _centerOfMassAbs), SizeV ));
                    //if (FlipHorizontal)
                    //    dc.Pop();
                    dc.Pop();
                    dc.Pop();
                }
            }
        }



    }
}
