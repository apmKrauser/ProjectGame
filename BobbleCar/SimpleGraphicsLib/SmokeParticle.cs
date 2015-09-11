using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SimpleGraphicsLib
{
    public class SmokeParticle : FlyingSphericParticle
    {

        new public class ParticleConfig : FlyingSphericParticle.ParticleConfig, IParticleConfigurable
        {
            public double SubParticleSpread { get; set; }
        }

        private ParticleConfig _config;
        private double _spreadPos = 1;

        public override GFXParticle.ParticleConfig BaseConfig { get { return _config; } }

        protected List<Vector> PositionOffsets = new List<Vector>();
        
        public SmokeParticle() : base()
        {
            
        }

        public override void init()
        {
            _config = new ParticleConfig();
            //Debug.WriteLine("=> Particle Init = " + BaseConfig.GetType().FullName);
            _config.ColorFrom = Color.FromArgb(230, 30, 30, 30);
            _config.ColorTo = Color.FromArgb(0, 108, 108, 108);
            _config.RadiusFrom = 4;
            _config.RadiusTo = 50;
            _config.BlurFrom = 7;
            _config.BlurTo = 20;
            _config.AppearanceSpread = 0.05;
            _config.SubParticleSpread = 20;
        }

        public override void init(GFXParticle.ParticleConfig _cfg)
        {
            _config = (_cfg as ParticleConfig);
            base.init(_cfg);
        }

        public override void Spawn()
        {
            Random rnd = new Random();
            base.Spawn(_config);
            _spreadPos = 1;
            PositionOffsets.Clear();
            //PositionOffsets = new List<Vector>();
            for (int i = 0; i < 5; i++)
            {
                Vector pos = new Vector(rnd.Next((int)_config.EmmissionArea.Left, (int)_config.EmmissionArea.Right),
                                      rnd.Next((int)_config.EmmissionArea.Top, (int)_config.EmmissionArea.Bottom)) ;
                PositionOffsets.Add(pos - Position);
            }
        }

        public override void Animation_Update(object sender, FrameUpdateEventArgs e)
        {
            if (IsAlive)
            {
                _spreadPos += _config.SubParticleSpread * (e.ElapsedMilliseconds/1000);
            }
            base.Animation_Update(sender, e);
        }


        //public override void Frame_Update(object sender, FrameUpdateEventArgs e)
        //{
        //    base.Frame_Update(sender, e);
        //    if (IsAlive)
        //    {
        //        DrawParticle();
        //    }
        //}

        protected override void DrawParticle()
        {
            using (DrawingContext dc = DVisual.RenderOpen())
            {
                Vector _offs;
                foreach (var offset in PositionOffsets)  // diese zeile ist auch nicht threadsafe
                {
                    _offs = offset;
                    if (_offs.Length > 0) _offs.Normalize();
                    _offs *= _spreadPos;
                    // nicht Threadsafe
                    ((BlurEffect)DVisual.Effect).Radius = _blurEffectRadius;
                    //dc.PushTransform(new TranslateTransform(_position.X, _position.Y));
                    SolidColorBrush brush = new SolidColorBrush(ParticleColor);
                    dc.DrawEllipse(brush, null, (Point)(Position + (_offs) + DrawingOffset), SizeV.X, SizeV.Y);
                    //dc.DrawEllipse(Brushes.Beige, null, new Point(0, 0), 20, 20);
                    //dc.Pop();
                } 
            }
        }

    }
}
