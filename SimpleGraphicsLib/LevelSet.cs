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
        public SpriteObject Background = null;

        [DataMember]
        public SpriteObject LevelBkg = null;
        //public List<SpriteObject> Sprites = new List<SpriteObject>();
        [DataMember]
        public ObservableCollection<SpriteObject> Sprites = new ObservableCollection<SpriteObject>();


        public void selectImage(SpriteObject obj)
        {
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
                File.Copy(fullpath, Helper.AssemblyLocalPath +"\\"+ relpath, true);
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

        public void SaveLevel()
        {
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LevelSet));
            DataContractSerializer ser = new DataContractSerializer(typeof(LevelSet));

            using (FileStream fs = new FileStream(@"data\Test.xml", FileMode.Create, FileAccess.Write))
            {
                ser.WriteObject(fs, this);
            }
        }

        public static LevelSet LoadLevel()
        {
            LevelSet lvl = null;
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LevelSet));
            DataContractSerializer ser = new DataContractSerializer(typeof(LevelSet));

            using (FileStream fs = new FileStream(@"data\Test.xml", FileMode.Open, FileAccess.Read))
            {
                lvl = ser.ReadObject(fs) as LevelSet;
            }
            return lvl;
        }

    }
}
