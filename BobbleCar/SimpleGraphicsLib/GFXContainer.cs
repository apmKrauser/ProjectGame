using System;
using System.Collections.Generic;
//using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
//using System.Drawing;
//using System.Threading;

namespace SimpleGraphicsLib
{
    public class GFXContainer : FrameworkElement, IDisposable
    {


        #region FieldsProperties
        private const double MaxFPS = 100;
        private double currentFrameFPS = 0;
        private double currentAnimationFPS = 0;
        private Stopwatch frameSW = new Stopwatch();


        VisualCollection Visuals;
        VisualCollection SystemOverlayVisuals;
        SortedList<string,IGFXObject> GFXObjects = new SortedList<string,IGFXObject>();

        public event Action<Object, FrameUpdateEventArgs> UpdateFrame;  // timing source for drawing
        public event Action<Object, FrameUpdateEventArgs> UpdateAnimation; // Preferred Animation timing source
        public event KeyEventHandler WindowKeyDown;  // key events routet from window
        public event KeyEventHandler WindowKeyUp;  // key events routet from window
        public SimpleCollider Collider = new SimpleCollider();

        public bool IsRunning { get; set; }
        public Vector DrawingOffset { get; set; }


        public SortedList<string,IGFXObject> ObjectList
        {
            get { return GFXObjects; }
        }
        

        #endregion

        public GFXContainer()
        {
            DrawingOffset = new Vector(0, 0);
            Visuals = new VisualCollection(this);
            SystemOverlayVisuals = new VisualCollection(this);
            //Focusable = true;
            this.Loaded += new RoutedEventHandler(GFXContainer_Loaded);
        }

        void GFXContainer_Loaded(object sender, RoutedEventArgs e)
        {
            // Bild für WPF designer:
            IsRunning = false;
            DrawingVisual visual = new DrawingVisual();
            Visuals.Add(visual);
            SystemOverlayVisuals.Add(new DrawingVisual());
            GFXContainer_DrawSplash();
            this.SizeChanged += GFXContainer_SizeChanged;
        }

        void GFXContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsRunning)
               GFXContainer_DrawSplash();
        }

        void GFXContainer_DrawSplash ()
        {
            using (DrawingContext dc = (Visuals[0] as DrawingVisual).RenderOpen())
            {
                //dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 2),
                //    new Rect(new Point(0 + x, 0), new Size(40, 40)));
                BitmapImage bmp = Properties.Resources.Splash1.ToMediaBitmap();
                dc.DrawImage(bmp, new Rect(0, 0, this.Width, this.Height));
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= (Visuals.Count + SystemOverlayVisuals.Count))
                throw new ArgumentOutOfRangeException("index");
            // SystemOverlayVisual are appended after normal visuals
            // check if index lies in SystemOverlayVisual range
            if (index >= Visuals.Count)
                return SystemOverlayVisuals[index - Visuals.Count]; 
            else
                return Visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return Visuals.Count + SystemOverlayVisuals.Count;
            }
        }

        // @debug
        public String TestHitTest(double x, double y)
        {
            //Point HitPoint = e.GetPosition(canvas1);
            //hitArea = new EllipseGeometry(HitPoint, 1.0, 1.0);
            ////This line will call a call back method HitTestCallBack
            //VisualTreeHelper.HitTest(canvas1, null, HitTestCallBack,
            //new GeometryHitTestParameters(hitArea));
            // http://www.c-sharpcorner.com/UploadFile/yougerthen/wpf-and-user-interactivity-part-i-dealing-with-geometries-and-shapes/

            //HitTestResult ht = (Visuals[1] as DrawingVisual).HitTest(new Point(x, y));
            HitTestResult ht = VisualTreeHelper.HitTest(this,new Point(x, y));
            string str = "null";
            if (ht != null)
            {
                var dv = (ht.VisualHit as DrawingVisual);
                str = dv == null ? "null" : "IDX[" + Visuals.IndexOf(dv) +"] : " + dv.ToString() + " > ";
                //String str = ht == null ? "null" : (ht as DrawingVisual).ToString();
                //str += (ht as GeometryHitTestResult).IntersectionDetail.ToString();
            }
            return str;
        }


        private void DrawOverlays()
        {
            // Draw FPS infotext
            DrawingVisual visualText = (DrawingVisual)SystemOverlayVisuals[0];
            using (DrawingContext dc = visualText.RenderOpen())
            {
                var fpstext = new FormattedText(String.Format("Frame FPS:\t{0:#0.0}\nAnim FPS:\t{1:#0.0}", currentFrameFPS, currentAnimationFPS),
                      CultureInfo.GetCultureInfo("en-us"),
                      FlowDirection.LeftToRight,
                      new Typeface("Verdana"),
                      10, System.Windows.Media.Brushes.Black);
                //dc.DrawText(fpstext, new Point(this.Width - fpstext.Width - 10, 0));
                dc.DrawText(fpstext, new Point(5, 5));
                // size of text
                // http://stackoverflow.com/questions/6717199/how-to-calculate-size-of-formattedtext-or-glyphrun-in-wpf
            }
        }

        protected void RegisterVisual_Callback (DrawingVisual vis)
        {
            Visuals.Add(vis);
        }

        protected void UnregisterVisual_Callback(DrawingVisual vis)
        {
            try
            {
                Visuals.Remove(vis);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("=> [GFXContainer.UnregisterVisual_Callback]: " + ex.Message);
            }
        }

        public void AddObject(IGFXObject obj, string name = null)
        {
            if (name == null) name = obj.GetType().Name + "::" + obj.GetHashCode().ToString();
            if ((obj.Name ?? "").Equals(""))
                obj.Name = name;
            if (GFXObjects.IndexOfKey(obj.Name) >= 0)
                obj.Name += "#" + obj.GetHashCode().ToString();
            GFXObjects.Add(obj.Name, obj);
            obj.RegisterDrawingVisual   += this.RegisterVisual_Callback;
            obj.UnregisterDrawingVisual += this.UnregisterVisual_Callback;
            obj.ParentContainer = this;
            this.UpdateFrame += obj.Frame_Update;
            //if (obj is IHasSeperateAnimationEvent)
            //this.UpdateAnimation += (obj as IHasSeperateAnimationEvent).Animation_Update;
            this.UpdateAnimation += obj.Animation_Update;
        }

        public IGFXObject RemoveObject(IGFXObject obj)
        {
            if (obj != null)
            {
                String name = "";
                try
                {
                    name = obj.Name;
                    GFXObjects.Remove(obj.Name);
                    this.UpdateFrame -= obj.Frame_Update;
                    //if (obj is IHasSeperateAnimationEvent)
                    this.UpdateAnimation -= obj.Animation_Update;
                    obj.ParentContainer = null; // invoke UnregisterAllVisuals. important to set before event is removed
                    obj.RegisterDrawingVisual -= this.RegisterVisual_Callback;
                    obj.UnregisterDrawingVisual -= this.UnregisterVisual_Callback;
                }
                catch (Exception ex) 
                {
                    Debug.WriteLine("=> [GFXContainer]: failed to remove " + name);
                }
            }
            return obj;
        }

        public IGFXObject FindObject(string name)
        {
            IGFXObject gobj;
            if (!GFXObjects.TryGetValue(name, out gobj))
                gobj = null;
            return gobj;
        }


        public void Dispose()
        {
            try
            {
                Stop();
                foreach (var item in GFXObjects)
                {
                    item.Value.Dispose();
                }
                GFXObjects.Clear();
            }
            catch (Exception e)
            {
                Debug.WriteLine("=> GFXContainer :: {0}", e.ToString());
            }
        }

        ~GFXContainer()
        {
            this.Dispose();
        }

        public void Start()
        {
            IsRunning = true;
            // Remove Splash Image
            using (DrawingContext dc = (Visuals[0] as DrawingVisual).RenderOpen()) { };

            // Start Stopwatch and hook Frameupdates methods 
            TimingSource.TimingEvents.UpdateCTargetRendering -= UpdateFrame_Invoke;
            TimingSource.TimingEvents.UpdateCTargetRendering += UpdateFrame_Invoke;
            //TimingSource.TimingEvents.UpdateDispatchTimer -= UpdateAnimation_Invoke;
            //TimingSource.TimingEvents.UpdateDispatchTimer += UpdateAnimation_Invoke;
            TimingSource.TimingEvents.UpdateSeparateThread += UpdateAnimation_Invoke;
        }

        public void Stop()
        {
            TimingSource.TimingEvents.UpdateCTargetRendering -= UpdateFrame_Invoke;
            //TimingSource.TimingEvents.UpdateDispatchTimer -= UpdateAnimation_Invoke;
            TimingSource.TimingEvents.UpdateSeparateThread -= UpdateAnimation_Invoke;
            IsRunning = false;
        }

        void UpdateFrame_Invoke(object sender, FrameUpdateEventArgs e)
        {
            currentFrameFPS = e.CurrentFPS;
            // update the sender
            if (UpdateFrame != null) UpdateFrame(this, e);
            DrawOverlays();
        }

        void UpdateAnimation_Invoke(object sender, FrameUpdateEventArgs e)
        {
            currentAnimationFPS = e.CurrentFPS;
            // update the sender
            if (UpdateAnimation != null) UpdateAnimation(this, e);
        }



        public object GetObjectXY(Point _point)
        {
            HitTestResult ht = VisualTreeHelper.HitTest(this, _point);
            string str = "null";
            if (ht != null)
            {
                var dv = (ht.VisualHit as DrawingVisual);

                str = dv == null ? "null" : "IDX[" + Visuals.IndexOf(dv) + "] : " + dv.ToString() + " > ";
                //String str = ht == null ? "null" : (ht as DrawingVisual).ToString();
                //str += (ht as GeometryHitTestResult).IntersectionDetail.ToString();
                Debug.WriteLine("############## " + str);
                return dv;
            }
            return null;
        }

        public void RaiseWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (WindowKeyDown != null)
                WindowKeyDown(this, e);
        }

        public void RaiseWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (WindowKeyUp != null)
                WindowKeyUp(this, e);
        }

        public List<string> GetAllNames(IGFXObject rootObject, int lvl)
        {
            List<string> lst = new List<string>();
            foreach (var item in rootObject.GetChildren())
            {
                lst.Add((new String('-', lvl)) + " " + item.Name);
                lst.AddRange(GetAllNames(item, lvl + 1));
            }
            return lst;
        }

        public List<string> ToList()
        {
            List<string> lst = new List<string>();
            foreach (var item in GFXObjects.Values)
            {
                lst.Add("+ " + item.Name);
                lst.AddRange(GetAllNames(item, 1));
            }
            return lst;
        }
        public override string ToString()
        {
            return String.Join("\r\n", this.ToList());
        }

    }
}
