using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SimpleGraphicsLib
{
    public class GFXParticle : IDisposable//, IParticleConfigurable
    {

        public class ParticleConfig : IParticleConfigurable, IPropertyInspectable
        {
            public double AverageLifetime { get; set; }
            public double LifetimeSpread { get; set; } // % of max life time
            public Rect EmmissionArea { get; set; }
            public Vector GroupVelocity { get; set; }
            public double GroupSpread { get; set; }

            public string Name { get; set;}

            public string TypeName
            {
                get { return this.GetType().Name; }
                set {}
            }
        }


        private ParticleConfig _config;
        public virtual ParticleConfig BaseConfig { get { return _config; } }

        public event Action<IGameObject, IGameObject, bool> OnCollision;  //  Me, Other, CalledByMe(me == caller of CheckCollision)

        public DrawingVisual DVisual = new DrawingVisual();

        public GFXContainer ParentContainer { get; set; }
        protected Vector DrawingOffset
        {
            get
            {
                try { return ParentContainer.DrawingOffset; }
                catch { return new Vector(0, 0); }
            }
        }


        private bool _alive = false;

        public bool IsAlive
        {
            get { return _alive; }
            protected set { _alive = value; }
        }

        public bool IsDead
        {
            get { return !_alive; }
        }

        public double TimeAlive { get; set; } // Milliseconds
        public double ThisLifetime { get; set; } // Milliseconds

        

        public GFXParticle()
        {
        }

        public virtual void init ()
        {
           _config = new ParticleConfig();
            Debug.WriteLine("=> Falsche init");
        }

        public virtual void init(ParticleConfig _cfg)
        {
           _config = _cfg;
        }

        public virtual void Spawn()
        {
            IsAlive = true;
            TimeAlive = 0;
        }

        //public void Spawn(ParticleSystem<GFXParticle> PS)
        //{
        //    // config aus PS und randomizen
        //    this.Spawn();
        //}
      

        // Todo: abstract method machen ?
        public virtual void Animation_Update(object sender, FrameUpdateEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// like a Draw() function. Just Draw. Animation is done by Animation_Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void Frame_Update(object sender, FrameUpdateEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void RaiseOnCollision(IGameObject me, IGameObject other, bool calledByMe)
        {
            OnCollision(me, other, calledByMe);
        }

    }
}
