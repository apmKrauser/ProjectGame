using SimpleGraphicsLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BobbleCar
{

    /// <summary>
    /// Game Logic
    /// Collision callbacks, Level initialization
    /// </summary>
    public static class LevelScript
    {
        static string FirstLevelPath = Helper.DataLocalPath + @"\LevelOne.xml";

        public static SmoothGameScroller Scroller;
        public static LevelSet ThisLevel = new LevelSet();
        static GUIAnimator GUIAni;
        static CarObject Player;
        static GameWindow GWin;
        static Stopwatch SWatch = new Stopwatch();

        /// <summary>
        /// Load Level and start animators
        /// </summary>
        /// <param name="gWin">Main game window object</param>
        /// <param name="levelFileName">Name of XML-File with level layout</param>
        /// <returns></returns>
        public static LevelSet StartLevel(GameWindow gWin, string levelFileName)
        {
            LevelSet Level = null;
            SpriteObject.AnimatedByDefault = false;

            string LevelPath = Helper.DataLocalPath + @"\" + levelFileName;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            ThisLevel.ClearLevel(gWin.MainGFX);
            ThisLevel = LevelSet.LoadLevel(LevelPath);
            ThisLevel.BuildLevel(gWin.MainGFX);
            
            GWin = gWin;

            GUIAni = new GUIAnimator(gfx: gWin.MainGFX,
                                     lvl: ThisLevel,
                                     timingSrc: TimingSource.Sources.CompositionTargetRendering,
                                     pgJumpRes: gWin.pgJumpResource);

            Player = gWin.MainGFX.FindObject("Player") as CarObject;
            if (Player == null)
            {
                ErrorMsg("No 'Player' object defined!");
                return Level;
            }

            // Particle Systems do not start by AnimatedByDefault = true
            // therefore call property setter
            ThisLevel.AnimatedAllSprites = true;  
            Scroller = new SmoothGameScroller(Player, gWin.MainGFX, gWin.GameWrapper);

            return Level;
        }


        /// <summary>
        /// Level initalization (called after StartLevel)
        /// </summary>
        /// <param name="GWin">Main game window object</param>
        /// <param name="LevelNumber">Number of Level</param>
        public static void LevelInit(GameWindow GWin, int LevelNumber)
        {
            switch (LevelNumber)
            {
                case 1:
                    var MotorObj = GWin.MainGFX.FindObject("Generator") as IGameObject;
                    if (MotorObj != null) MotorObj.OnCollision += Level1_Motor_OnCollision_BroughtHome;
                    break;
                case 10:
                    var MotorObj2 = GWin.MainGFX.FindObject("Generator") as IGameObject;
                    if (MotorObj2 != null) MotorObj2.OnCollision += Level1_Motor_OnCollision_BroughtHome;
                    break;
                default:
                    break;
            }           
        }


        /// <summary>
        /// Show intro (instructive Messagebox, video ...)
        /// </summary>
        /// <param name="GWin">Main game window object</param>
        /// <param name="LevelNumber">Number of Level</param>
        public static void Intro_Level(GameWindow GWin, int LevelNumber)
        {
            switch (LevelNumber)
            {
                case 1:
                    GWin.MessageBox_Dispatched("Bobblecar needs a new Engine", "Get the engine on top of the mountains by pushing it backwards towards Bobblecar's cave.\n\nControl keys:\n\tLeft\t= drive left\n\tRight\t= drive right\n\tDown\t= brake\n\tUp\t= load damper spring (release to jump)\n\n");
                    break;
                case 10:
                    GWin.MessageBox_Dispatched("Bobblecar needs a new Engine", "Player 1 control keys:\n\tLeft\t= drive left\n\tRight\t= drive right\n\tDown\t= brake\n\tUp\t= load damper spring (release to jump)\n\nPlayer 2 control keys:\n\ta\t= drive left\n\td\t= drive right\n\ts\t= brake\n\tw\t= load damper spring (release to jump)");
                    break;
                default:
                    break;
            }
            // StopWatch tracking time player needs to proceed level
            SWatch.Restart();
        }

        // Collision Callbacks
#region Collision_Event_Listener
        public static void Level1_Motor_OnCollision_BroughtHome(IGameObject me, IGameObject other, bool calledByMe)
        {
            if (calledByMe)
            {
                if (other.Name.Contains("Höhle"))
                {
                    SWatch.Stop();
                    GWin.MessageBox_Dispatched("Great Job", String.Format("You achieved this in\n>> {0}:{1:00} <<\nHow fast can your buddies handle this?", 
                        SWatch.Elapsed.Hours*60 + SWatch.Elapsed.Minutes, 
                        SWatch.Elapsed.Seconds));
                    me.OnCollision -= Level1_Motor_OnCollision_BroughtHome;
                }
            }
        }

#endregion

        public static void ErrorMsg ( string str )
        {
            MessageBox.Show(str);
        }


    }
}
