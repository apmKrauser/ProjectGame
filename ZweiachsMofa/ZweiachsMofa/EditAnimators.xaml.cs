using SimpleGraphicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace ZweiachsMofa
{
    /// <summary>
    /// Interaktionslogik für EditAnimators.xaml
    /// </summary>
    public partial class EditAnimators 
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

        private void dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<AnimationRigidBody> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.
            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            Debug.WriteLine("=> Drop ACTION <");
            if (lowerIdx < 0)
            {
                if (sender == this.lstNewAnimations)
                    Debug.WriteLine("=> Do Stuff" );
                Debug.WriteLine("=> item = {0} {1}", lowerIdx, sender.GetType().Name);
                // The item came from the lower ListView
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
            }

            // Set this to 'Move' so that the OnListViewDrop knows to 
            // remove the item from the other ListView.
            e.Effects = System.Windows.DragDropEffects.Move;
        }

       

        private void lstAnimations_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine("=> Draaaag <");
            e.Effects = System.Windows.DragDropEffects.Move;
        }

        private void lstAnimations_Drop(object sender, DragEventArgs e)
        {
            Debug.WriteLine("=> Drop <");
            // todo: alles löschen
            if (e.Effects == System.Windows.DragDropEffects.None)
                return;

            //SpriteObject droppedData = e.Data.GetData(typeof(SpriteObject)) as SpriteObject;
            //SpriteObject target = ((System.Windows.Controls.ListViewItem)(sender)).DataContext as SpriteObject;

            //int removedIdx = lstSprites.Items.IndexOf(droppedData);
            //int targetIdx = lstSprites.Items.IndexOf(target);
            if (sender == this.lstAnimations)
            {
                if (this.dragMgr.IsDragInProgress)
                    return;

                // so remove that item from the other ListView.
                //(this.listView2.ItemsSource as ObservableCollection<Task>).Remove(task);
            }
        }

        private void lstAnimations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IPropertyInspectable obj = (lstAnimations.SelectedItem as IPropertyInspectable);
            PropertyInspect pi = new PropertyInspect(obj);
            pi.Show();
        }

    }
}
