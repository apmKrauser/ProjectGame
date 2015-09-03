using SimpleGraphicsLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZweiachsMofa
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class GameWindow 
    {
        GameScroller Scroller;

        public GameWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainGFX.Dispose();
        }

        private void CmdStart_Click(object sender, RoutedEventArgs e)
        {
            MainGFX.Start();

            SpriteObjectElastic Backgr = new SpriteObjectElastic("Background");
            Backgr.Bmp = Properties.Resources.Cartoon_jungle_background.ToMediaBitmap();
            Backgr.SizeV = new Vector(MainGFX.Width, MainGFX.Height);
            Backgr.CenterOfMass = new Vector(0, 0);
            Backgr.IsLiquid = false;
            Backgr.SpringC = 10;
            Backgr.DampingC = 5;
            Backgr.Position = new Vector(0, 0);
            MainGFX.AddObject(Backgr);


            SpriteObjectElastic WaterBk = new SpriteObjectElastic("WaterBk");
            WaterBk.Bmp = Properties.Resources.water_texture1.ToMediaBitmap();
            WaterBk.SizeV = new Vector(600, 150);
            WaterBk.CenterOfMass = new Vector(0, 0);
            WaterBk.IsLiquid = true;
            WaterBk.SpringC = 10;
            WaterBk.DampingC = 5;
            WaterBk.Position = new Vector(550, 400);
            MainGFX.AddObject(WaterBk);
            //MainGFX.Collider.AddObject(WaterBk);

            SpriteObjectElastic Floor = new SpriteObjectElastic("Floor");
            Floor.Bmp = Properties.Resources.strip_of_wood.ToMediaBitmap();
            Floor.SizeV *= 0.5;
            Floor.CenterOfMass = new Vector(0, 0);
            Floor.IsLiquid = false;
            Floor.SpringC = 30;
            Floor.DampingC = 5;
            Floor.Position = new Vector(50, 150);
            MainGFX.AddObject(Floor);
            MainGFX.Collider.AddObject(Floor);

            CarObject mySprite = new CarObject("Auto");
            mySprite.Bmp = Properties.Resources.BaseCar2.ToMediaBitmap();
            //mySprite.Bmp = BitmapConversion.StringToBitmap("db");
            //mySprite.CenterOfMass = new Vector(0, 0);
            //mySprite.Position = new Vector(0, 150);
            mySprite.Position = new Vector(300, 400);
            mySprite.SizeV = new Vector(mySprite.Bmp.Width / 2, mySprite.Bmp.Height / 2);
            mySprite.NormSpeed = new Vector(0,0);
            mySprite.SpringC = 25;
            mySprite.DampingC = 2;
            mySprite.IsDeformable = true;
            mySprite.IsMovable = true;
            MainGFX.AddObject(mySprite);



            SpriteObjectElastic Water = new SpriteObjectElastic("Water");
            Water.Bmp = Properties.Resources.water_texture1.ToMediaBitmap();
            Water.SizeV = new Vector(600, 200);
            Water.CenterOfMass = new Vector(0, 0);
            Water.IsLiquid = true;
            Water.SpringC = 15;
            Water.DampingC = 3;
            Water.Position = new Vector(550, 450);
            MainGFX.AddObject(Water);
            MainGFX.Collider.AddObject(Water);

            foreach (var item in MainGFX.ObjectList)
            {
                Debug.WriteLine(item.Key);
            }

        }

        private void MainGFX_MouseMove(object sender, MouseEventArgs e)
        {
            CmdTest.Content = MainGFX.TestHitTest(e.GetPosition(MainGFX).X, e.GetPosition(MainGFX).Y); ;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CmdStopAni_Click(object sender, RoutedEventArgs e)
        {
            //theAnimation.Dispose();
            SpriteObjectElastic mySprite = (MainGFX.ObjectList["Auto"] as SpriteObjectElastic);
            mySprite.RemoveAnimation("Mover");
            mySprite.RemoveAnimation("Gravity");
        }

        private void CmdStartAni_Click(object sender, RoutedEventArgs e)
        {
            //theAnimation = new TestAnimation(mySprite, MainGFX);
           // mySprite.AddAnimation(new TestAnimation(), "Test");
            SpriteObjectElastic mySprite = (MainGFX.ObjectList["Auto"] as SpriteObjectElastic);
            mySprite.Position = new Vector(0, 200);
            mySprite.NormSpeed = new Vector(0, -200);
            //mySprite.AddAnimation(new AnimationLinearTranslation(), "Mover");
            mySprite.AddAnimation(new AnimationConstAcceleration(new Vector(20, 100)), "Gravity");
            mySprite.AddAnimation(new AnimationWobble(1, 0.005), "Wobble");

            Scroller = new GameScroller(mySprite, MainGFX, GameWrapper);

        }

        private void CmdTest_Click(object sender, RoutedEventArgs e)
        {
            //GFXParticle g = new GFXParticle();
            //g.init();
            //FlyingSphericParticle p = new FlyingSphericParticle();
            //p.init();
            //Debug.WriteLine("=> Particle init = {0}", p.Common.Config.b);
            //Debug.WriteLine("==================> type particle = " + (typeof(FlyingSphericParticle.ParticleConfig)).ToString());
            //ParticleSystem<GFXParticle, GFXParticle.ParticleConfig> Ps = new ParticleSystem<GFXParticle,GFXParticle.ParticleConfig>(0, 0, false);
            //Ps.Config.AverageLifetime = 5;

            //ParticleSystem<FlyingSphericParticle, FlyingSphericParticle.ParticleConfig> Ps2 = new ParticleSystem<FlyingSphericParticle, FlyingSphericParticle.ParticleConfig>(0, 100, false);
            //ParticleSystem<SmokeParticle, SmokeParticle.ParticleConfig> Ps2 = new ParticleSystem<SmokeParticle,SmokeParticle.ParticleConfig>(0, 50, false);
            //Debug.WriteLine("");
            //if (Ps2.Config == null) Debug.WriteLine("=> null");
            //Ps2.Config.AverageLifetime = 4000;
            //Ps2.Config.AirDrag = 0;
            //Ps2.GenerationRate = 10;
            //Ps2.Config.GroupVelocity = new Vector(-40, -35);
            //Ps2.Config.GroupSpread = 5;
            //Ps2.Config.AirDrag = 0.05;
            //Ps2.Config.EmmissionArea = new Rect(200, 600, 30, 20);
            //MainGFX.AddObject(Ps2);
            //Ps2.Start();
            //TimingSource.TimingEvents.UpdateCTargetRendering += Ps2.Animation_Update;
            //TimingSource.TimingEvents.UpdateSeparateThread += Ps2.Animation_Update;
            //Thread.Sleep(4000);
            //TimingSource.TimingEvents.UpdateSeparateThread -= Ps2.Animation_Update;
            //Ps2.Config.BlurFrom = 80;
            ////Debug.WriteLine("=> fs BlurFrom = {0}", (fs.BaseConfig as FlyingSphericParticle.ParticleConfig).BlurFrom);
            //Rect r1 = new Rect(new Point(0, 0), new Point(-0.5, 0.5));
            //Console.WriteLine(r1);
            //MainGFX.Margin = new Thickness(0,0, 0,0);
           // MainGFX.Height = 100;

        }
    }
}
