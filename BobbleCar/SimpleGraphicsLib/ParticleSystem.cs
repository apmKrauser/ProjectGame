using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SimpleGraphicsLib
{


    public class ParticleSystem<T, TConf> : IGFXObject where T: GFXParticle, new()
                                                       where TConf: class, IParticleConfigurable, new()
    {

        public const double DropFramesMinFPS = 40;
        public const double DropFramesMaxFPS = 55;
        public event Action<DrawingVisual> RegisterDrawingVisual;
        public event Action<DrawingVisual> UnregisterDrawingVisual;

        public int MaxParticles { get; protected set; }
        public int InitialParticles { get; protected set; }

        public double GenerationRate { get; set; }
        public string Name { get; set; }

        public bool Animated { get; set; }
        

        public TConf Config
        {
            get {  return (_configParticle.BaseConfig as TConf); }
        }

        private int frameDrop = 3;  // 2 = draw every 2nd frame, 3 = draw every 3rd frame
        private int frameShift = 0;


        private GFXContainer _parentContainer;
        public GFXContainer ParentContainer
        {
            get { return _parentContainer; }
            set
            {
                _parentContainer = value;
                if (value == null)
                    Dispose();
                else
                    init();
            }
        }

        protected List<DrawingVisual> Visuals = new List<DrawingVisual>();
        protected List<T> Particles = new List<T>();
        protected bool GrowDynamically = false;
        protected int CurrentParticleLimit = 0;
        protected T _configParticle = new T();
        protected double MillisSinceLastEmmission = 0;


        public ParticleSystem(int initialParticleCount, int maxParticleCount, bool growDynamically)
            : this(initialParticleCount, maxParticleCount, growDynamically, null)
        { }

        public ParticleSystem(int initialParticleCount, int maxParticleCount, bool growDynamically, TConf particleConf)
        {
            if (particleConf != null)
                _configParticle.init(particleConf as GFXParticle.ParticleConfig);
            else
                _configParticle.init();
            //Debug.WriteLine("Mist");
            //var grrr = (_configParticle as T).BaseConfig;
            //var grrr3 = (_configParticle.BaseConfig as TConf);
            //Debug.WriteLine("=>  Mist = " + grrr.GetType().FullName);
            this.MaxParticles = maxParticleCount;
            this.InitialParticles = initialParticleCount;
            this.GrowDynamically = growDynamically;
        }

        protected void init()
        {
            Animated = false;
            if (GrowDynamically)
                GenerateNewParticles((int)(MaxParticles / 32) + 1);
            else
                GenerateNewParticles(MaxParticles);
        }

        protected void GenerateNewParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T particle = new T();
                particle.ParentContainer = _parentContainer;
                particle.init(_configParticle.BaseConfig);
                Particles.Add(particle);
                RegisterDrawingVisual(particle.DVisual);
            }
            CurrentParticleLimit += count;
        }

        public void Start()
        {
            Animated = true;
        }

        public void Stop()
        {
            Animated = false;
        }

        public void SpawnParticle()
        {
            bool spawned = false;
            foreach (var p in Particles)
            {
                if (p.IsDead)
                {
                    p.Spawn();
                    spawned = true;
                    break;
                }
            }
            if (!spawned)
            {
                int newpart = Particles.Count;
                if ((MaxParticles - Particles.Count)< newpart) 
                    newpart = (MaxParticles - Particles.Count);
                GenerateNewParticles(newpart);
            }

        }

        public virtual void Animation_Update(object sender, FrameUpdateEventArgs e)
        {
            if (Animated)
            {
                MillisSinceLastEmmission += e.ElapsedMilliseconds;
                if (MillisSinceLastEmmission > (1000 / GenerationRate))
                {
                    int newparticles = (int)(MillisSinceLastEmmission / (1000 / GenerationRate));
                    //Debug.WriteLine("=> New Particles" + newparticles);
                    for (int i = 0; i < newparticles; i++)
                    {
                        SpawnParticle();
                    }
                    MillisSinceLastEmmission = 0;
                }
                foreach (var particle in Particles)
                {
                    particle.Animation_Update(this, e);
                }
            }
        }

        public void Frame_Update(object sender, FrameUpdateEventArgs e)
        {// wenn e > min fps nur jeden 2./3. 
            // hilft nix !!!!!!!
            //if (e.ElapsedMilliseconds > (1000 / DropFramesMinFPS))
            //{
            //    //frameDrop = Math.Min(frameDrop + 1, Particles.Count / 4);
            ////    frameDrop = Math.Min(frameDrop + 1, 2);
            //}
            //if (e.ElapsedMilliseconds < (1000 / DropFramesMaxFPS))
            //{
            ////    frameDrop = Math.Max(frameDrop - 1, 1);
            ////    frameShift = 0;
            //}
            //frameShift++;
            //if (frameShift > frameDrop - 1) frameShift = 0;
            //for (int i = frameShift; i < Particles.Count; i += frameDrop)
            //{
            //    Particles[i].Frame_Update(this, e);
            //}

            //// Todo: drawShape für PS !
            //if (Config.DrawShape)
            //using (DrawingContext dc = vis.RenderOpen())
            //{
            //    if (DrawShape)
            //        dc.DrawRectangle(null, new Pen(Brushes.Black, 2), rectangle: new Rect(Shape.Location + (_parent.DrawingOffset * ScrollScaling), Shape.Size));
            //}

            foreach (var particle in Particles)
            {
                particle.Frame_Update(this, e);
            }     
        }


        ~ParticleSystem()
        {
            Dispose();
        }

        public void Dispose()
        {
            UnregisterAllVisuals();
            foreach (var p in Particles)
            {
                p.Dispose();
            }
            Particles.Clear();
        }

        private void UnregisterAllVisuals()
        {
            try
            {
                foreach (var p in Particles)
                {
                    UnregisterDrawingVisual(p.DVisual);
                }

            }
            catch (Exception e)
            {
            }
        }



        public void AddObject(IGFXObject obj)
        {
            throw new NotImplementedException("A Leaf of a Composion cannot hold children.\nUse GFXComposition instead.");
        }

        public void RemoveObject(IGFXObject obj)
        {
            throw new NotImplementedException("A Leaf of a Composion cannot hold children.\nUse GFXComposition instead.");
        }

        public ObservableCollection<IGFXObject> GetChildren()
        {
            return new ObservableCollection<IGFXObject>();
        }


        public bool Highlight
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public bool ContainsVisual(Visual v)
        {
            throw new NotImplementedException();
        }
    }
}
