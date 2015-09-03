using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SimpleGraphicsLib
{
    public class LevelSet
    {
        public SpriteObject Background = null;
        public SpriteObject LevelBkg = null;
        //public List<SpriteObject> Sprites = new List<SpriteObject>();
        public ObservableCollection<SpriteObject> Sprites = new ObservableCollection<SpriteObject>();

        public void selectImage(SpriteObject obj)
        {
            String fullpath = "";
            String relpath = "";
            BitmapImage img = null;
            try
            {
                OpenFileDialog dlgOpen = new OpenFileDialog();
                dlgOpen.Filter = "png|*.png|jpeg|*.jpg";
                if (dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    fullpath = dlgOpen.FileName;
                    relpath = @"data\" + dlgOpen.SafeFileName;
                }
                if (!Directory.Exists("data"))
                    Directory.CreateDirectory("data");
                File.Copy(fullpath, relpath, true);
                obj.ImagePath = relpath;
                obj.loadFromImagePath();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error in File Selection SpriteObject " + obj.Name + ":\n" + ex.Message);
                Trace.WriteLine("LevelSet Load Fehler");
            }
        }

        public void RemoveSpritesFrom (GFXContainer gfxc)
        {
            foreach (var sprite in Sprites)
            {
                gfxc.RemoveObject(sprite);
            }
        }

        public void AddSpritesTo (GFXContainer gfxc)
        {
            foreach (var sprite in Sprites)
            {
                gfxc.AddObject(sprite);
            }
        }


    }
}
