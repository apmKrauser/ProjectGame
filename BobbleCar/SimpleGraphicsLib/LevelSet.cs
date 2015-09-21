using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SimpleGraphicsLib
{
    [DataContract]
    public class LevelSet
    {
        [DataMember]
        public IGameObject Background = null;

        [DataMember]
        public IGameObject LevelBkg = null;
        //public List<SpriteObject> Sprites = new List<SpriteObject>();
        [DataMember]
        public ObservableCollection<SpriteObject> Sprites = new ObservableCollection<SpriteObject>();

        public bool AnimatedAllSprites 
        {
            set 
            {
                foreach (var sprite in Sprites)
                {
                    sprite.Animated = value;
                }
            }
        }

        public void selectImage(IGameObject sobj)
        {
            if (!(sobj is SpriteObject)) return;
            SpriteObject obj = sobj as SpriteObject;

            String fullpath = "";
            String relpath = "";
            try
            {
                OpenFileDialog dlgOpen = new OpenFileDialog();
                dlgOpen.Filter = "png|*.png|jpeg|*.jpg";
                if (dlgOpen.ShowDialog() == DialogResult.OK)
                {
                    fullpath = dlgOpen.FileName;
                    relpath = Helper.DataDir + "\\" + dlgOpen.SafeFileName;
                }
                if (!Directory.Exists(Helper.DataLocalPath))
                    Directory.CreateDirectory(Helper.DataLocalPath);
                try
                { File.Copy(fullpath, Helper.AssemblyLocalPath + "\\" + relpath, true); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
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

        public void ClearCollider(GFXContainer maingfx)
        {
            maingfx.Collider.Clear();
        }

        public void InitializeCollider(GFXContainer maingfx)
        {
            foreach (var sprite in Sprites)
            {
                if (sprite.CanCollide)
                    maingfx.Collider.AddObject(sprite); 
            }
        }

        public void BuildLevel (GFXContainer maingfx)
        {
            this.Background.loadFromImagePathPreserveObjectSize();
            this.LevelBkg.loadFromImagePathPreserveObjectSize();
            maingfx.AddObject(this.Background);
            maingfx.AddObject(this.LevelBkg);
            maingfx.Width = this.LevelBkg.SizeV.X;
            // Load Pictures
            foreach (var sprite in this.Sprites)
            {
                if (sprite is SpriteObject)
                   (sprite as SpriteObject).loadFromImagePathPreserveObjectSize();
            }
            this.AddSpritesTo(maingfx);
            this.InitializeCollider(maingfx);
        }


        public void ClearLevel (GFXContainer maingfx)
        {
            maingfx.Collider.Clear();
            RemoveSpritesFrom(maingfx);
            maingfx.RemoveObject(Background);
            maingfx.RemoveObject(LevelBkg);
        }

        public void SaveLevel(string filepath)
        {
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LevelSet));
            DataContractSerializer ser = new DataContractSerializer(typeof(LevelSet));

            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                ser.WriteObject(fs, this);
            }
        }

        public static LevelSet LoadLevel(string filepath)
        {
            LevelSet lvl = null;
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LevelSet));
            DataContractSerializer ser = new DataContractSerializer(typeof(LevelSet));

            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                lvl = ser.ReadObject(fs) as LevelSet;
            }
            return lvl;
        }

    }
}
