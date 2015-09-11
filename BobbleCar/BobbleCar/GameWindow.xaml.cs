using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
using System.Windows.Threading;

namespace BobbleCar
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class GameWindow : MetroWindow
    {
        private bool _shutdown = false;


        public double DesignWindowStartWidth
        {
            get
            {
                return SystemParameters.WorkArea.Width * 0.9;
            }

            set { }
        }

        public double DesignWindowStartHeight
        {
            get
            {
                double height = SystemParameters.WorkArea.Width / 1550 * 720;
                height = Math.Min(height, SystemParameters.WorkArea.Height * 0.9);
                return height;
            }
        }
        
        
        public GameWindow()
        {
            InitializeComponent();
            // Animations start by default
            SpriteObject.AnimatedByDefault = true;
        }

        private void GameMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // After everything has loaded, rendered and so on
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {
                //System.Windows.Forms.MessageBox.Show("dispatched");
                MainGFX.Start();
                GameStarting();
            }));
        }

        private void GameStarting()
        {
            LevelScript.StartFirstLevel(this);
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
            // construct:
           // CarObject mySprite = new CarObject("Auto");
           // //mySprite.Bmp = Properties.Resources.BaseCar2.ToMediaBitmap();
           // //mySprite.Bmp = BitmapConversion.StringToBitmap("db");
           // //mySprite.CenterOfMass = new Vector(0, 0);
           // //mySprite.Position = new Vector(0, 150);
           // mySprite.Position = new Vector(300, 400);
           // mySprite.SizeV = new Vector(mySprite.Bmp.Width / 2, mySprite.Bmp.Height / 2);
           // mySprite.NormSpeed = new Vector(0, 0);
           // mySprite.SpringC = 25;
           // mySprite.DampingC = 2;
           // mySprite.IsDeformable = true;
           // mySprite.IsMovable = true;
           // MainGFX.AddObject(mySprite);
           // //theAnimation = new TestAnimation(mySprite, MainGFX);
           //// mySprite.AddAnimation(new TestAnimation(), "Test");
           // SpriteObjectElastic mySprite = (MainGFX.ObjectList["Auto"] as SpriteObjectElastic);
           // mySprite.Position = new Vector(0, 200);
           // mySprite.NormSpeed = new Vector(0, -200);
           // //mySprite.AddAnimation(new AnimationLinearTranslation(), "Mover");
           // mySprite.AddAnimation(new AnimationConstAcceleration(new Vector(20, 100)), "Gravity");
           // mySprite.AddAnimation(new AnimationWobble(1, 0.005), "Wobble");

           // //Scroller = new GameScroller(mySprite, MainGFX, GameWrapper);

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

        private void GameWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            MainGFX.RaiseWindowKeyDown(this, e);
        }

        private void GameWindow_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            MainGFX.RaiseWindowKeyUp(this, e);
        }

        private async void MetroWindow_Closing_Async(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_shutdown;
            if (_shutdown) return;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Quit",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("Quit",
                "Close Game?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            bool affirm = result == MessageDialogResult.Affirmative;
            if (affirm)
            {
                // Close Game 
                // Todo: save score
                // Todo: save score on dispose
            }
            _shutdown = affirm;

            if (_shutdown)
            {
                MainGFX.Dispose();
                this.Close();
                //System.Windows.Application.Current.Shutdown();
            }
        }


    }
}
