using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Reflection;

namespace SimpleGraphicsLib
{
    [DataContract]
    [KnownType("GetDerivedTypes")]
    public class SpriteObject : IDisposable, IGameObject, IAnimationOnDispose, IHasSeperateAnimationEvent, IPropertyInspectable
    {

        #region FieldsProperties

        public event Action<IGameObject, IGameObject, bool> OnCollision;  //  Me, Other, CalledByMe(me == caller of CheckCollision)
        //[XmlIgnore]
        public event Action<DrawingVisual> RegisterDrawingVisual;
        //[XmlIgnore]
        public event Action<DrawingVisual> UnregisterDrawingVisual;
        // geht auch:
        //event Action<DrawingVisual> IGFXObject.RegisterDrawingVisual
        //{
        //    add { throw new NotImplementedException(); }
        //    remove { throw new NotImplementedException(); }
        //}

        //[Conditional("Debug")] bleded ganze Funktion aus

        protected List<DrawingVisual> Visuals = new List<DrawingVisual>();
        //protected List<GFXAnimation> Animations = new List<GFXAnimation>();

        // public and moved from IAnimation... to class due to XMLSerializer
        //[DataMember]
        protected  ObservableCollection<IAnimationRigidBody> Animations = new ObservableCollection<IAnimationRigidBody>();

        // for serialization
        [DataMember]
        public ObservableCollection<AnimationRigidBody> SerializableAnimations
        {
            get
            {
                var r = from a in Animations
                        select a as AnimationRigidBody;
                return new ObservableCollection<AnimationRigidBody>(r);
            }
            set
            {
                RemoveAnimations();
                foreach (var ani in value)
                {
                    AddAnimation(ani);
                }
            }
        }

        

        protected BitmapImage _bmp = null;


        protected GFXContainer _parentContainer;
        private double _blurEffectRadius ;
        private Vector _centerOfMassAbs ;
        private Vector _size;
        private Vector _centerOfMass;
        private Rect _deformation ;
        private Vector _deformationPos ;  // position offset
        private Vector _deformationSize ;  // size offset
        private bool _isGrounded;
        private bool IsDisposed = false;

        static public bool DrawShape = false; // draw outline
        static public bool AnimatedByDefault = true; // start animations by default

        public bool Highlight { get; set; }  // Highlight if DrAWShape =true

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public virtual bool FlipHorizontal { get; set; }
        
        public String TypeName
        {
            get { return this.GetType().Name; }
            set { } // empty setter in order to show up in property inspector
        }

        public bool IsGrounded 
        { 
            get { return _isGrounded; }
            set { _isGrounded = value; }
        }

        private bool _animated;

        public virtual bool Animated
        {
            get { return _animated; }
            set 
            { 
                _animated = value;
                foreach (var animation in Animations)
                {
                    animation.IsActive = _animated;
                }
            }
        }
        

        //[XmlIgnore]
        public BitmapImage Bmp
        {
            get { return _bmp; }
            set 
            {
                _bmp = value;
                SizeV = value == null ? new Vector(1, 1) : new Vector(value.Width, value.Height);
            } 
        }

        //[XmlIgnore]
        public BitmapImage BmpNullfree
        {
            get 
            {
                if (Bmp == null)
                    return Properties.Resources.NoImage.ToBitmap().ToMediaBitmap();
                else
                    return Bmp; 
            }
        }

        [DataMember]
        public bool IsMovable { get; set; }

        [DataMember]
        public bool IsObstacle { get; set; }

        [DataMember]
        public bool CanCollide { get; set; }

        [DataMember]
        public Vector NormSpeed { get; set; }

        private object _positionSync; // Semaphore
        private Vector _position;

        [DataMember]
        public Vector Position
        {  // [MethonImpl] methode nur für 1 thread
            get { lock (_positionSync) return _position; }  // Interlocked. increment/.... für "Atomare" operationen
            set { lock (_positionSync) _position = value; }
        }

        [DataMember]
        public double ScrollScaling { get; set; }

        private object _shapeSync; // Semaphore

        [XmlIgnore]
        public Rect Shape
        {
            get { lock(_shapeSync) return new Rect(Position.X - _centerOfMassAbs.X, Position.Y - _centerOfMassAbs.Y, SizeV.X, SizeV.Y); }
        }

        private object _deformSync; // Semaphore

        [DataMember]
        public Rect Deformation
        {
            get { lock(_deformSync) return _deformation; }
            set 
            {
                lock (_deformSync)
                {
                    _deformation = value;
                    _deformationPos = new Vector(_deformation.TopLeft.X * SizeV.X,
                                                         _deformation.TopLeft.Y * SizeV.Y);
                    _deformationSize = new Vector((_deformation.Width * SizeV.X) - SizeV.X,
                                                           (_deformation.Height * SizeV.Y) - SizeV.Y);
                }
            }
        }

        [DataMember]
        public double Weight { get; set; }

        [DataMember]
        public double AirDrag { get; set; }

        [DataMember]
        public double Angle { get; set; }

        [DataMember]
        public virtual Vector SizeV 
        { 
            get { return _size; }
            set
            {
                _size = value;
                _centerOfMassAbs = new Vector(SizeV.X * CenterOfMass.X, SizeV.Y * CenterOfMass.Y);
            } 
        }

        [DataMember]
        public virtual String ImagePath { get; set; }

        [DataMember]
        public Vector CenterOfMass  // (0,0)..(1,1)
        {
            get { return _centerOfMass; } 
            set
            {
                _centerOfMass = value;
                _centerOfMassAbs = new Vector(SizeV.X * _centerOfMass.X, SizeV.Y * _centerOfMass.Y);
                //Debug.WriteLine("cog "+Name+" ## " + _centerOfMass + "  " + _centerOfMassAbs);
                // todo: Wird nur aufgerufen wenn Vector mit new zugewiesen wird!!
            }
        }

        [DataMember]
        public virtual double BlurEffectRadius {
            get { return _blurEffectRadius; }
            set 
            {
                if (Visuals.Count > 0)
                {
                    var vis = Visuals[0];
                    _blurEffectRadius = value < 0 ? 0 : value;
                    BlurEffect myBlurEffect = new BlurEffect();
                    myBlurEffect.Radius = BlurEffectRadius;
                    vis.Effect = myBlurEffect;
                }
            } 
        }

        [XmlIgnore]
        public Vector PixelSpeed
        {
            get
            {
                return GFXAnimation.NormToPixelSpeed(NormSpeed);
            }
        }

        [XmlIgnore]
        public virtual GFXContainer ParentContainer
        {
            get { return _parentContainer; }
            set
            {
                _parentContainer = value;
                if (value == null)
                    UnregisterAllVisuals();
                else
                    init();
            }
        }




#endregion

        public static IEnumerable<Type> GetDerivedTypes()
        {
            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsSubclassOf(typeof(SpriteObject))
                        select t;
            return types;
        }

        public SpriteObject() : this("") { }

        public SpriteObject(string name)
        {
            // keine overridable methoden im CTOR aufrufen !!
            // https://msdn.microsoft.com/en-us/library/ms182331.aspx
            this.Name = name;
            ImagePath = "";
            OnCreate();
            Position =  new Vector(0, 0);
            NormSpeed = new Vector(0, 0);
            SizeV = new Vector(1, 1);
            CenterOfMass = new Vector(0.5, 0.5);
            Weight = 1;
            Angle = 0;
            BlurEffectRadius = 0;
            ScrollScaling = 1;
            IsMovable = false;
            IsObstacle = true;
        }

        [OnDeserializing]  
        protected void OnDeserializing(StreamingContext context)
        {
            OnCreate();
        }

        private void OnCreate()
        {
            IsDisposed = false;
            Highlight = false;
            _animated = AnimatedByDefault;
            _positionSync = new object();
            _deformSync = new object();
            _shapeSync = new object();
            _blurEffectRadius = 0;
            _centerOfMassAbs = new Vector(0, 0);
            _deformation = new Rect(0,0,1,1);
            _deformationPos  = new Vector(0, 0);  // position offset
            _deformationSize = new Vector(0, 0);  // size offset
            // Drawing Visuals erstellen
            Animations = new ObservableCollection<IAnimationRigidBody>();
            Visuals = new List<DrawingVisual>();
            DrawingVisual vis = new DrawingVisual();
            RenderOptions.SetBitmapScalingMode(vis, BitmapScalingMode.Fant);
            Visuals.Add(vis);
            AddAnimation(new AnimationLinearTranslation(), "LinMove");
        }

        protected virtual void init ()   // bei setparent aufrufen?  artikel über virtual in ctor aufrufen lesen
        {
            RegisterDrawingVisual(Visuals[0]);
            foreach (var animation in Animations)
            {
                // if animations are added before GFXContainer
                animation.SetTimingSource(this.ParentContainer);
                if (animation is IAnimKeyInput) 
                {
                    var kani = animation as IAnimKeyInput;
                    _parentContainer.WindowKeyDown += kani.OnKeyDown;
                    _parentContainer.WindowKeyUp += kani.OnKeyUp;
                }
            }
        }


        protected void UnregisterAllVisuals ()
        {
            try
            {
                foreach (var vis in Visuals)
                {
                    UnregisterDrawingVisual(vis);
                }

            }
            catch (Exception e)
            {
                //1global::System.Windows.Forms.MessageBox.Show("UnregisterDrawingVisual w/o GFXContainer ["+ e.GetType().Name + "]" );
                Debug.WriteLine("=> UnregisterDrawingVisual w/o GFXContainer [{0}]", e.GetType().Name);
                //throw;
            }
        }

        private void RemoveAnimations()
        {
            for (int i = Animations.Count - 1; i >= 0; i--)
            {
                if ((Animations[i] is IAnimKeyInput)  && (_parentContainer != null))
                {
                    var kani = Animations[i] as IAnimKeyInput;
                    _parentContainer.WindowKeyDown -= kani.OnKeyDown;
                    _parentContainer.WindowKeyUp -= kani.OnKeyUp;
                }
                Animations[i].Dispose();
            }
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                UnregisterAllVisuals();
                // todo: rückwärts zählen wg remove
                //foreach (var animation in Animations.Values)
                //{
                //    Debug.WriteLine("## Ani: " + animation.Name);
                //}
                // dispose is designed to remove ani from list, hence, foreach impossible 
                RemoveAnimations();
                //            foreach (var animation in Animations.Values)
                //            {
                //            }
                Animations.Clear();
                Visuals.Clear();
            }
        }


        ~SpriteObject()
        {
            this.Dispose();
        }

        public IAnimationRigidBody GetAnimation(string name)
        {
            var r = (from n in Animations
                    where n.Name.Equals(name)
                    select n).FirstOrDefault();
            return r;
        }

        public void AddAnimation(IAnimationRigidBody animation, string name = null)
        {
            if (name != null) animation.Name = name;
            if ((animation.Name ?? "").Equals("")) animation.Name = "Ani" + "::" + animation.GetHashCode().ToString();
            //if (String.IsNullOrEmpty(animation.Name))
            int i = Animations.IndexOf(animation);
            if (i < 0)
                Animations.Add(animation);
            else
                Animations[i] = animation;

            animation.Sprite = this;
            animation.SetTimingSource(TimingSource.Sources.Manual);
            animation.IsActive = Animated;
            if ((animation is IAnimKeyInput) && (_parentContainer != null))
            {
                var kani = animation as IAnimKeyInput;
                _parentContainer.WindowKeyDown += kani.OnKeyDown;
                _parentContainer.WindowKeyUp += kani.OnKeyUp;
            }
            //if (this.Parent != null)
            //    animation.SetTimingSource(this.Parent);
        }

        public void RemoveAnimation(string animation)
        {
            try
            {
                GetAnimation(animation).Dispose();
                //Animations.Remove(animation);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Animation {0} not present in {1} :: ", animation, Name, e.Message);
            }
        }

        public void RemoveAnimation(IAnimationRigidBody animation)
        {
            //RemoveAnimation(animation.Name);
            try
            {
                if ((animation is IAnimKeyInput) && (_parentContainer != null))
                {
                    var kani = animation as IAnimKeyInput;
                    _parentContainer.WindowKeyDown -= kani.OnKeyDown;
                    _parentContainer.WindowKeyUp -= kani.OnKeyUp;
                }
                Animations.Remove(animation);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Animation {0} not present in {1}.", animation.Name, Name);
                //throw;
            }
        }

        public bool ContainsVisual(Visual vis)
        {
            bool res = false;
            foreach (var v in Visuals)
            {
                if (v.Equals(vis))
                    res = true;
            }
            return res;
        }

        public void Animation_OnDispose(IAnimationRigidBody animation)
        {
                // if animations are added before GFXContainer
            RemoveAnimation(animation);
            //Debug.WriteLine("=> Animation_OnDispose = {0}");
        }

        public virtual void Frame_Update(object sender, FrameUpdateEventArgs e)
        {
            DrawingVisual vis = Visuals[0] as DrawingVisual;

            using (DrawingContext dc = vis.RenderOpen())
            {
                if (Bmp != null)
                {
                    dc.PushTransform(new TranslateTransform(Position.X + (_parentContainer.DrawingOffset.X * ScrollScaling), Position.Y + _parentContainer.DrawingOffset.Y));
                    dc.PushTransform(new RotateTransform(Angle));
                    if (FlipHorizontal)
                        dc.PushTransform(new ScaleTransform(-1, 1));
                    dc.DrawImage(Bmp, new Rect((Point)(_deformationPos - _centerOfMassAbs), SizeV + _deformationSize));

                    // dc.DrawImage(Bmp, new Rect(-_centerOfMassAbs.X, -_centerOfMassAbs.Y, SizeV.X, SizeV.Y));
                    if (FlipHorizontal)
                        dc.Pop();
                    dc.Pop();
                    dc.Pop();
                }
                if (DrawShape)
                {
                    DrawShapeAndMarkers(dc);
                }
            }
        }

        //[Conditional("Debug")]
        protected virtual void DrawShapeAndMarkers (DrawingContext dc)
        {
            System.Windows.Media.Brush br;
            if (Highlight)
               br = new System.Windows.Media.SolidColorBrush(Color.FromArgb(120, 200, 200, 255));
            else
               br = null;
            dc.DrawRectangle(br, new Pen(Brushes.Black, 2), rectangle: new Rect(Shape.Location + (_parentContainer.DrawingOffset * ScrollScaling), Shape.Size));
            dc.DrawEllipse(null, new Pen(Brushes.Red, 2), (Point)(Position + (_parentContainer.DrawingOffset * ScrollScaling)), 5, 5);

            if (Highlight)
            {
                var text = new FormattedText(Name,
                      CultureInfo.GetCultureInfo("en-us"),
                      FlowDirection.LeftToRight,
                      new Typeface("Verdana"),
                      10, System.Windows.Media.Brushes.DarkRed);
                dc.DrawText(text, (Point)(Position + (_parentContainer.DrawingOffset * ScrollScaling)));
            }
        }

        public virtual void Animation_Update(object sender, FrameUpdateEventArgs e)
        {
            foreach (var animation in Animations)
            {
                animation.Update(this, e);
            }
        }


        public void ZoomPreserveAspectRatio(double width = 0, double height = 0)
        {
            if (width > 0)
            {
                SizeV = new Vector(width, SizeV.Y * (width / SizeV.X));
            }
            if (height > 0)
            {
                SizeV = new Vector(SizeV.X * (height / SizeV.Y), height);
            }
        }

        public void  loadFromImagePath()
        {
            if (ImagePath.Equals("")) return;
            try
            {
                //Bmp = new BitmapImage(new Uri(@"file:///" + ImagePath));
                String path = Helper.AssemblyLocalPath + "\\" + ImagePath;
                //Debug.WriteLine("=> Set Image: " + path);
                //Bmp = new BitmapImage(new Uri(path));
                Bmp = BitmapConversion.LoadBitmapAndDispose(path);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error loading Image from Disk for SpriteObject " + Name + ":\n" + ex.Message);
            }
        }

        public void loadFromImagePathPreserveObjectSize ()
        {
            Vector tmp = new Vector(SizeV.X, SizeV.Y);
            loadFromImagePath();
            SizeV = tmp;
        }




        public ObservableCollection<IGFXObject> GetChildren()
        {
            // A "Leaf" of a composition doesn't have children
            return new ObservableCollection<IGFXObject>();
        }


        public void RaiseOnCollision(IGameObject me, IGameObject other, bool calledByMe)
        {
            if (OnCollision != null)
                OnCollision(me, other, calledByMe);
        }
    }
}
