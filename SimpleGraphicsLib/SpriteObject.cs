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

namespace SimpleGraphicsLib
{
    public class SpriteObject : IGFXObject, IRigidBody, IAnimationOnDispose
    {

        #region FieldsProperties
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

        protected List<DrawingVisual> Visuals = new List<DrawingVisual>();
        //protected List<GFXAnimation> Animations = new List<GFXAnimation>();
        protected SortedList<string, IAnimatonRigidBody> Animations = new SortedList<string, IAnimatonRigidBody>();

        protected BitmapImage _bmp = null;


        protected GFXContainer _parent;
        private double _blurEffectRadius = 0;
        private Vector _centerOfMassAbs = new Vector(0, 0);
        private Vector _size;
        private Vector _centerOfMass;
        private Rect _deformation = new Rect(0,0,1,1);
        private Vector _deformationPos  = new Vector(0, 0);  // position offset
        private Vector _deformationSize = new Vector(0, 0);  // size offset

        //[XmlIgnore]
        static public bool DrawShape = false; // draw outline; debug
        //private AnimationLinearTranslation _aniMove = new AnimationLinearTranslation();

        public string Name { get; set; }
        public String TypeName
        {
            get { return this.GetType().Name; }
            set {  } // empty setter in order to show up in property inspector
        }
        

        private String myVar;

        

        [XmlIgnore]
        public BitmapImage Bmp
        {
            get { return _bmp; }
            set 
            {
                _bmp = value;
                SizeV = value == null ? new Vector(1, 1) : new Vector(value.Width, value.Height);
            } 
        }

        [XmlIgnore]
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

        public bool IsMovable { get; set; }
        public bool IsObstacle { get; set; }
        public Vector NormSpeed { get; set; }

        private object _positionSync = new object();
        private Vector _position;
        public Vector Position
        {
            get { lock (_positionSync) return _position; }
            set { lock (_positionSync) _position = value; }
        }

        public double ScrollScaling { get; set; }

        [XmlIgnore]
        public Rect Shape
        {
            get { return new Rect(Position.X - _centerOfMassAbs.X, Position.Y - _centerOfMassAbs.Y, SizeV.X, SizeV.Y); }
        }

        public Rect Deformation
        {
            get { return _deformation; }
            set 
            {
                _deformation = value;
                _deformationPos = new Vector(  _deformation.TopLeft.X * SizeV.X,
                                                     _deformation.TopLeft.Y * SizeV.Y );
                _deformationSize =  new Vector( (_deformation.Width  * SizeV.X) - SizeV.X, 
                                                       (_deformation.Height * SizeV.Y) - SizeV.Y );
            }
        }
        

        public double Weight { get; set; }

        public double AirDrag { get; set; }

        public double Angle { get; set; }

        public virtual Vector SizeV 
        { 
            get { return _size; }
            set
            {
                _size = value;
                _centerOfMassAbs = new Vector(SizeV.X * CenterOfMass.X, SizeV.Y * CenterOfMass.Y);
            } 
        }

        public virtual String ImagePath { get; set; }

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
        public virtual GFXContainer Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (value == null)
                    UnregisterAllVisuals();
                else
                    init();
            }
        }




#endregion


        public SpriteObject() : this("") { }

        public SpriteObject(string name)
        {
            // keine overridable methoden im CTOR aufrufen !!
            // https://msdn.microsoft.com/en-us/library/ms182331.aspx
            this.Name = name;
            ImagePath = "";
            Position = NormSpeed = new Vector(0, 0);
            SizeV = new Vector(1, 1);
            CenterOfMass = new Vector(0.5, 0.5);
            Weight = 1;
            Angle = 0;
            BlurEffectRadius = 0;
            ScrollScaling = 1;
            IsMovable = false;
            IsObstacle = true;
            // Drawing Visuals erstellen
            DrawingVisual vis = new DrawingVisual();
            RenderOptions.SetBitmapScalingMode(vis, BitmapScalingMode.Fant);
            Visuals.Add(vis);

            //Debug.WriteLine("=> Constructor von SpriteObject called !!!");
        }

        protected virtual void init ()   // bei setparent aufrufen?  artikel über virtual in ctor aufrufen lesen
        {
            RegisterDrawingVisual(Visuals[0]);
            AddAnimation(new AnimationLinearTranslation(), "LinMove");
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

        public virtual void Dispose()
        {
            UnregisterAllVisuals();
            // todo: rückwärts zählen wg remove
            //foreach (var animation in Animations.Values)
            //{
            //    Debug.WriteLine("## Ani: " + animation.Name);
            //}
            for (int i = 0; i < Animations.Count; i++)
            {
                Animations.Values[0].Dispose();
            }
//            foreach (var animation in Animations.Values)
//            {
//            }
            Animations.Clear();
            Visuals.Clear();
        }


        ~SpriteObject()
        {
            this.Dispose();
        }

        public void AddAnimation(IAnimatonRigidBody animation, string name = null)
        {
            if (name == null) name = animation.GetType().Name + "::" + animation.GetHashCode().ToString();
            animation.Name = name;
            if (Animations.ContainsKey(name))
                Animations[name] = animation;
            else
                Animations.Add(name, animation);
            animation.Sprite = this;
            animation.SetTimingSource(this.Parent);
        }

        public void RemoveAnimation(string animation)
        {
            try
            {
                Animations[animation].Dispose();
                //Animations.Remove(animation);
            }
            catch (Exception e)
            {
            }
        }

        public void RemoveAnimation(IAnimatonRigidBody animation)
        {
            //RemoveAnimation(animation.Name);
            try
            {
                 Animations.Remove(animation.Name);
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        public void Animation_OnDispose(IAnimatonRigidBody animation)
        {
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
                    dc.PushTransform(new TranslateTransform(Position.X + (_parent.DrawingOffset.X * ScrollScaling), Position.Y + _parent.DrawingOffset.Y));
                    dc.PushTransform(new RotateTransform(Angle));
                    dc.DrawImage(Bmp, new Rect((Point)(_deformationPos - _centerOfMassAbs), SizeV + _deformationSize));

                    // dc.DrawImage(Bmp, new Rect(-_centerOfMassAbs.X, -_centerOfMassAbs.Y, SizeV.X, SizeV.Y));
                    dc.Pop();
                    dc.Pop();
                }
                if (DrawShape)
                    dc.DrawRectangle(null, new Pen(Brushes.Black, 2), rectangle: new Rect(Shape.Location + (_parent.DrawingOffset * ScrollScaling), Shape.Size));
            }
            //BlurBitmapEffect myBlurEffectObsolete = new BlurBitmapEffect();  // obsolete check new!!
                //visual.BitmapEffect = myBlurEffectObsolete;
                //if (BlurEffectRadius > 0)
                //{
                //    BlurEffect myBlurEffect = new BlurEffect();
                //    myBlurEffect.Radius = BlurEffectRadius;
                //    vis.Effect = myBlurEffect;
                //}
                //else
                //{
                //    vis.Effect = null;
                //}
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
            try
            {
                //Bmp = new BitmapImage(new Uri(@"file:///" + ImagePath));
                String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + "\\" + ImagePath;
                Debug.WriteLine("=> Set Image: " + path);
                //Bmp = new BitmapImage(new Uri(path));
                Bmp = BitmapConversion.LoadBitmapAndDispose((new Uri(path)).LocalPath);
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

    }
}
