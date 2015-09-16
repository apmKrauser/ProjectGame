using SimpleGraphicsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BobbleCar
{
    public static class LevelScript
    {
        static string FirstLevelPath = Helper.DataLocalPath + @"\LevelOne.xml";

        public static SmoothGameScroller Scroller;
        public static LevelSet ThisLevel = new LevelSet();
        static GUIAnimator GUIAni;
        static CarObject Player;


        public static LevelSet StartLevel(GameWindow GWin, string levelFileName)
        {
            LevelSet Level = null;
            SpriteObject.AnimatedByDefault = false;

            string LevelPath = Helper.DataLocalPath + @"\" + levelFileName;

            ThisLevel.ClearLevel(GWin.MainGFX);
            ThisLevel = LevelSet.LoadLevel(LevelPath);
            ThisLevel.BuildLevel(GWin.MainGFX);

            GUIAni = new GUIAnimator(gfx: GWin.MainGFX,
                                     lvl: ThisLevel,
                                     timingSrc: TimingSource.Sources.CompositionTargetRendering,
                                     pgJumpRes: GWin.pgJumpResource);

            Player = GWin.MainGFX.FindObject("Player") as CarObject;
            if (Player == null)
            {
                ErrorMsg("No 'Player' object defined!");
                return Level;
            }

            ThisLevel.AnimatedAllSprites = true;  // Particle Systems do not start by AnimatedByDefault = true
            Scroller = new SmoothGameScroller(Player, GWin.MainGFX, GWin.GameWrapper);

            return Level;
        }

        public static void ErrorMsg ( string str )
        {
            MessageBox.Show(str);
        }

    }
}
