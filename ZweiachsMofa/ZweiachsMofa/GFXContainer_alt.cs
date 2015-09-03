using System;
using System.Collections.Generic;
//using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
//using System.Drawing;
//using System.Threading;

namespace SimpleGraphicsLib
{
    public class GFXContainer_alt : FrameworkElement, IDisposable
    {

        const double MaxFPS = 100;
        double currentFPS = 0;

        private Stopwatch frameSW = new Stopwatch();

        public class FrameUpdateEventArgs : EventArgs
        {
            public double ElapsedMilliseconds { get; private set; }
            public FrameUpdateEventArgs(long _elapsedMilliseconds) : this((double)_elapsedMilliseconds) { }
            public FrameUpdateEventArgs (double _elapsedMilliseconds) : base()
            {
                ElapsedMilliseconds = (_elapsedMilliseconds < 1) ? 1f : _elapsedMilliseconds;
            }
        }

        VisualCollection Visuals;
        //VisualCollection OverlayVisuals;
        VisualCollection SystemOverlayVisuals;

        List<DrawingVisual> vc = new List<DrawingVisual>(); // @debug

        public event Action<Object, FrameUpdateEventArgs> UpdateFrame;
        BitmapImage bmp;  // debug
        double sinex = 0;
        double SpriteBlur = 0;
        Point SpritePos;
        Point SpriteSize;
        //TimedMover myMover; // debug
        private double SpriteAngle = 0; // debug


        GFXContainer_alt()
        {
            Visuals = new VisualCollection(this);

            this.Loaded += new RoutedEventHandler(GFXContainer_Loaded);
        }

        void GFXContainer_Loaded(object sender, RoutedEventArgs e)
        {
            int x = 0;

            DrawingVisual visual_pack = new DrawingVisual();

            VisualCollection extraVisuals = new VisualCollection(visual_pack);
            
            for (int i = 0; i < 2; i++)
            {
                
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext dc = visual.RenderOpen())
                {
                    if (i < 1)
                    {
                    dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 2),
                        new Rect(new Point(0 + x, 0), new Size(40, 40)));
                    } else {
                //    bmp = Properties.Resources.BaseCar2.ToMediaBitmap();
                    SpritePos = new Point(-50, -20.2);
                    SpriteSize = new Point(bmp.Width/2, bmp.Height/2);
                    dc.DrawImage(bmp, new Rect(0, 0, bmp.Width, bmp.Height));
                    }
                }
                RenderOptions.SetBitmapScalingMode(visual, BitmapScalingMode.Fant);
                Debug.WriteLine("=> BitmapScaling = {0}", RenderOptions.GetBitmapScalingMode(visual));
                Debug.WriteLine("=> EdgeMode = {0}", RenderOptions.GetEdgeMode(visual));
                //visual.Transform.BeginAnimation
                Visuals.Add(visual);

                // Test mit mehr Visuals
                //extraVisuals.Add(visual);                
                // --- end Test
            }
            //Visuals.Add(visual_pack);

            // Test mit mehr Visuals

            DrawingVisual visual2 = new DrawingVisual();

            using (DrawingContext dc = visual2.RenderOpen())
            {
                dc.DrawText(
                      new FormattedText(String.Format("FPS: {0:#0.0}", currentFPS),
                      CultureInfo.GetCultureInfo("en-us"),
                      FlowDirection.LeftToRight,
                      new Typeface("Verdana"),
                      26, System.Windows.Media.Brushes.Black),
                    new Point(300, 0));
            }

            Debug.WriteLine("=> Parent Before = {0}", visual2.Parent);
            Visuals.Add(visual2);
            // --- end Test      

            //List<DrawingVisual> vc = new List<DrawingVisual>();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 8000; i++)
            {
                DrawingVisual vis = new DrawingVisual();
                vc.Add(vis);
                Visuals.Add(vis);
            }
            Debug.WriteLine("=> generate = {0}", sw.ElapsedMilliseconds);
            sw.Restart();
            //foreach (var item in vc)
            //{
            //    Visuals.Remove(item);
            //}
            for (int i = 0; i < vc.Count; i++)
            {
               // vc[i] = new DrawingVisual();
                //vc[i] = null;
            }
            Debug.WriteLine("=> remove = {0}", sw.ElapsedMilliseconds);
            DrawingVisual vos = (DrawingVisual)Visuals[20];
            Debug.WriteLine("=> Parent = {0}", vos.Parent);
            Debug.WriteLine("=> Parent after Remove = {0}", vos.Parent);


        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= Visuals.Count)
                throw new ArgumentOutOfRangeException("index");
            return Visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return Visuals.Count;
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

        public void UpdateFrame_delme(Object sender, FrameUpdateEventArgs elapsedMillis)
        {
            //if (sinex > 2*Math.PI) sinex = 0;
            sinex %= 2 * Math.PI;
            sinex += 2.5;
            SpriteAngle += 1.5;
            SpriteAngle %= 360;
            SpriteBlur += 0.5;
            SpriteBlur %= 20;
            // wrappanel benutzen für crop?
            Vector dpos = new Vector(Math.Round(Math.Sin(sinex) * 1),
                      Math.Round(Math.Sin(sinex) * 5));
            Vector dsize = new Vector(Math.Round(dpos.X / 2.0), -dpos.Y);

            Point newPos = Point.Add(SpritePos, dpos);
            Size newSize = (Size)Point.Add((Point)SpriteSize, dsize);
            //Point newPos = SpritePoint;
            //Size newSize = new Size(50+sinex, 30+sinex);

            //newPos += dpos;
            //bmp = Properties.Resources.BaseCar.ToMediaBitmap();

            // Draw FPS infotext
            DrawingVisual visualText = (DrawingVisual)Visuals[3];
            //DrawingVisual visualText = vc[3];
            using (DrawingContext dc = visualText.RenderOpen())
            {
                dc.DrawText(
                      new FormattedText(String.Format("FPS: {0:#0.0}", currentFPS),
                      CultureInfo.GetCultureInfo("en-us"),
                      FlowDirection.LeftToRight,
                      new Typeface("Verdana"),
                      26, System.Windows.Media.Brushes.Black),
                    new Point(0, 0));
                // size of text
                // http://stackoverflow.com/questions/6717199/how-to-calculate-size-of-formattedtext-or-glyphrun-in-wpf
            }


           // using (DrawingContext dc = vis.RenderOpen()) { }
            DrawingVisual visual = (DrawingVisual)Visuals[2];
            
            //DrawingVisual visual = vc[1];
            using (DrawingContext dc = visual.RenderOpen())
            {
                //dc.PushTransform(new TranslateTransform(newPos.X, newPos.Y));
                dc.PushTransform(new RotateTransform(SpriteAngle));          
                dc.DrawImage(bmp, new Rect(newPos, newSize));
                
                dc.Pop();
                //dc.Pop();
            }
            //BlurBitmapEffect myBlurEffectObsolete = new BlurBitmapEffect();  // obsolete check new!!
            //visual.BitmapEffect = myBlurEffectObsolete;
            BlurEffect myBlurEffect = new BlurEffect();
            myBlurEffect.Radius = SpriteBlur;
            visual.Effect = myBlurEffect;
    

  /*
             Bitmap newImage = new Bitmap(newWidth, newHeight);
using (Graphics gr = Graphics.FromImage(newImage))
{
    gr.SmoothingMode = SmoothingMode.HighQuality;
    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
    gr.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
}
             */
        }


        public void Dispose()
        {
            try
            {
                Stop();
            }
            catch (Exception e)
            {
                Debug.WriteLine("=> GFXContainer :: {0}", e.ToString());
            }
        }

        ~GFXContainer_alt()
        {
            this.Dispose();
        }

        public void Start()
        {
            frameSW.Restart();
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            UpdateFrame += UpdateFrame_delme; // @debug
        }

        public void Stop()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // do not invoke faster than MaxFPS
            long elapsed = frameSW.ElapsedMilliseconds;
            if (elapsed >= (1000.0 / MaxFPS))
            {
                currentFPS = 1000.0 / elapsed;
                frameSW.Restart();
                UpdateFrame(this, new FrameUpdateEventArgs(elapsed));
            }
        }

    }
}
