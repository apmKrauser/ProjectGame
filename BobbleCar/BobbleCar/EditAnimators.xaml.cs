using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SimpleGraphicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.JoshSmith.ServiceProviders.UI;


namespace BobbleCar
{
    /// <summary>
    /// Adding/Removing an changing animation properties
    /// </summary>
    public partial class EditAnimators : MetroWindow
    {

        IPropertyInspectable GObj;
        ObservableCollection<AnimationRigidBody> AList = new ObservableCollection<AnimationRigidBody>();
        //ObservableCollection<AnimationRigidBody> AListNew = new ObservableCollection<AnimationRigidBody>();
        ListViewDragDropManager<AnimationRigidBody> dragMgr;
        ListViewDragDropManager<AnimationRigidBody> dragMgr2;


        public EditAnimators()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="obj">Game object wich implement IAnimationRigidBody</param>
        public EditAnimators(IPropertyInspectable obj)
        {
            GObj = obj as SpriteObject;
            if (GObj == null)
            {
                this.Close();
                return;
            }
            InitializeComponent();
            dragMgr = new ListViewDragDropManager<AnimationRigidBody>(lstAnimations);
            dragMgr2 = new ListViewDragDropManager<AnimationRigidBody>(lstNewAnimations);
            dragMgr.ShowDragAdorner = true;
            dragMgr.ProcessDrop += dragMgr_ProcessDrop;
            dragMgr2.ShowDragAdorner = true;
            dragMgr2.ProcessDrop += dragMgr_ProcessDrop;
            Assembly assi = Assembly.GetAssembly(typeof(GFXContainer));
            var AnimatorObjects = from typ in assi.GetTypes()
                              where typeof(AnimationRigidBody).IsAssignableFrom(typ) && !typ.IsInterface
                              select (Activator.CreateInstance(typ) as AnimationRigidBody);

            lstNewAnimations.ItemsSource = AnimatorObjects;
            lstNewAnimations.SelectedIndex = 0;

            //AList = new ObservableCollection<AnimationRigidBody>(((GObj as SpriteObject).SerializableAnimations));
            AList = (GObj as SpriteObject).SerializableAnimations;
            lstAnimations.ItemsSource = AList;
        }

        
        private async void dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<AnimationRigidBody> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.
            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            if ((lowerIdx < 0) && (e.DataItem.Name == null))
            {
                var name = await this.ShowInputAsync("Name", "Please specify unique animator name or hit cancel!");

                //await this.ShowMessageAsync("Hello", "Hello " + result + "!");

                //var sobj= new SpriteObject(txtSpriteName.Text);
                Debug.WriteLine("=> item = {0} {1}", e.DataItem.GetType().Name, e.DataItem.Name);
                AnimationRigidBody ani = Activator.CreateInstance(e.DataItem.GetType()) as AnimationRigidBody;
                ani.Name = name;
                (GObj as SpriteObject).AddAnimation(ani);  // generates unique  name if name==null
                AList.Add(ani) ;
                //lstAnimations.Items.Refresh();

                // Resize Columns
                var gridView = lstAnimations.View as GridView;
                foreach (var column in gridView.Columns.Where(column => Double.IsNaN(column.Width)))
                {
                    Contract.Assume(column != null);
                    column.Width = column.ActualWidth;
                    column.Width = Double.NaN;
                }

                // so just insert it.
                //e.ItemsSource.Insert(higherIdx, e.DataItem);
            }
            else
            {
                // null values will cause an error when calling Move.
                // It looks like a bug in ObservableCollection to me.
                if (e.ItemsSource[lowerIdx] == null ||
                    e.ItemsSource[higherIdx] == null)
                    return;

                // The item came from the ListView into which
                // it was dropped, so swap it with the item
                // at the target index.
                e.ItemsSource.Move(lowerIdx, higherIdx);
                e.ItemsSource.Move(higherIdx - 1, lowerIdx);
                // update Sprite animation list
                (GObj as SpriteObject).SerializableAnimations = AList;
            }

            // Set this to 'Move' so that the OnListViewDrop knows to 
            // remove the item from the other ListView.
            e.Effects = System.Windows.DragDropEffects.Move;
        }

       

        private void lstAnimations_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Move;
        }

        private void lstAnimations_Drop(object sender, DragEventArgs e)
        {
            if (e.Effects == System.Windows.DragDropEffects.None)
                return;

            if (sender == this.lstAnimations)
            {
                if (this.dragMgr.IsDragInProgress)
                    return;
            }
        }

        private void lstAnimations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IPropertyInspectable obj = (lstAnimations.SelectedItem as IPropertyInspectable);
            PropertyInspect pi = new PropertyInspect(obj);
            pi.Show();
        }

        private void lstAnimations_KeyDown(object sender, KeyEventArgs e)
        {
            AnimationRigidBody obj = (lstAnimations.SelectedItem as AnimationRigidBody);
            if (obj != null)
            {
                if (e.Key == Key.Delete)
                {
                    AList.Remove(obj);
                    (GObj as SpriteObject).RemoveAnimation(obj);
                }
            }
        }

    }
}
