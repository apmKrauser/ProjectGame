using SimpleGraphicsLib;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media;
using WPF.JoshSmith.ServiceProviders.UI;
using System.Reflection;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
//using System.Collections.ObjectModel;

// todo: http://stackoverflow.com/questions/4616505/is-there-a-reason-why-a-base-class-decorated-with-xmlinclude-would-still-throw-a

namespace ZweiachsMofa
{
    /// <summary>
    /// Interaktionslogik für GameDesigner.xaml
    /// </summary>
    public partial class GameDesigner : MetroWindow
    {

        GameScroller Scroller;
        LevelSet ThisLevel = new LevelSet();
        Vector objResizeRefpoint = new Vector();
        //Point dragDropPoint = new Point(0,0);
        private bool _shutdown = false;

        ListViewDragDropManager<SpriteObject> dragMgr;
        //ListViewDragDropManager<SpriteObject> dragMgr2;

        public GameDesigner()
        {
            InitializeComponent();
            // Drag and Drop
            //Style itemContainerStyle = new Style(typeof(ListBoxItem));
            //itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
            //itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(s_PreviewMouseLeftButtonDown)));
            //itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new System.Windows.DragEventHandler(lstSprites)));
            //lstSprites.ItemContainerStyle = itemContainerStyle;

            // Drag&Drop Mngr
            this.dragMgr = new ListViewDragDropManager<SpriteObject>(lstSprites);
            //this.dragMgr2 = new ListViewDragDropManager<SpriteObject>(lstNewObj);
            // Turn the ListViewDragManager on . 
            //dragMgr.ListView = lstSprites;
            dragMgr.ShowDragAdorner = true;
            //dragMgr2.ShowDragAdorner = true;
            //dragMgr.DragAdornerOpacity = 5;
            // Apply or remove the item container style, which responds to changes
            // in the attached properties of ListViewItemDragState.
            //lstSprites.ItemContainerStyle = this.FindResource("ItemContStyle") as Style; 
            //lstSprites.ItemContainerStyle = null;

            this.dragMgr.ProcessDrop += dragMgr_ProcessDrop;
            //this.dragMgr2.ProcessDrop += dragMgr_ProcessDropNewObj;

            Assembly assi = Assembly.GetAssembly(typeof(GFXContainer));
            var GameObjects = from typ in assi.GetTypes()
                              where typeof(SpriteObject).IsAssignableFrom(typ) && !typ.IsInterface
                              select typ;
            dbNewObj.ItemsSource = GameObjects;
            dbNewObj.SelectedIndex = 0;
        }


        private void GameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           // Debug.WriteLine("=> Slider = {0}", GameSlider.Value);
        }

        private void GameSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("=> Slider = {0}", GameSlider.Value);
        }

        private void cmdSetParBkg_Click(object sender, RoutedEventArgs e)
        {
            //MainGFX.Height = 50;
            //GameWrapper.Width = 50;
            MainGFX.RemoveObject(ThisLevel.Background);
            ThisLevel.Background = new SpriteObject();
            ThisLevel.Background.Name = "Background";
            ThisLevel.Background.CenterOfMass = new Vector(0, 0);
            ThisLevel.Background.IsMovable = false;
            ThisLevel.Background.CanCollide = false;
            ThisLevel.selectImage(ThisLevel.Background);
            ThisLevel.Background.ZoomPreserveAspectRatio(height: GameWrapper.Height);
            MainGFX.AddObject(ThisLevel.Background);
            // todo: eigentlich in setlvlbackg
            //MainGFX.Width = thisLevel.Background.SizeV.X;
           // MainGFX.Height = 500;

        }
        private void cmdSetLevelBkg_Click(object sender, RoutedEventArgs e)
        {
            MainGFX.RemoveObject(ThisLevel.LevelBkg);
            ThisLevel.LevelBkg = new SpriteObject();
            ThisLevel.LevelBkg.Name = "LevelBkg";
            ThisLevel.LevelBkg.CenterOfMass = new Vector(0, 0);
            ThisLevel.LevelBkg.IsMovable = false;
            ThisLevel.LevelBkg.CanCollide = false;
            ThisLevel.selectImage(ThisLevel.LevelBkg);
            ThisLevel.LevelBkg.ZoomPreserveAspectRatio(height: GameWrapper.Height);
            ThisLevel.Background.ScrollScaling = (ThisLevel.Background.SizeV.X - (GameWrapper.Width/2))/ ThisLevel.LevelBkg.SizeV.X;
            MainGFX.AddObject(ThisLevel.LevelBkg);
            // todo: eigentlich in setlvlbackg
            MainGFX.Width = ThisLevel.LevelBkg.SizeV.X;
            // MainGFX.Height = 500; 
        }


        private void Window_Activated(object sender, EventArgs e)
        {
            //if (!_activated)
            //{
            //    _activated = true;
            //    Scroller = new GameScroller(GameSlider, MainGFX, GameWrapper);
            //    MainGFX.Start();
            //}
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // After everything has loaded, rendered and so on
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {
                //System.Windows.Forms.MessageBox.Show("dispatched");
                lstSprites.ItemsSource = ThisLevel.Sprites;
                Scroller = new GameScroller(GameSlider, MainGFX, GameWrapper);
                MainGFX.Start();
            }));
        }

        private void cmdSaveLevel_Click(object sender, RoutedEventArgs e)
        {         
            ThisLevel.SaveLevel();
        }

        private void cmdLoadLevel_Click(object sender, RoutedEventArgs e)
        {
            ThisLevel = LevelSet.LoadLevel();

            ThisLevel.Background.loadFromImagePathPreserveObjectSize();
            ThisLevel.LevelBkg.loadFromImagePathPreserveObjectSize();
            MainGFX.AddObject(ThisLevel.Background);
            MainGFX.AddObject(ThisLevel.LevelBkg);
            MainGFX.Width = ThisLevel.LevelBkg.SizeV.X;
            lstSprites.ItemsSource = ThisLevel.Sprites;
            foreach (var sprite in ThisLevel.Sprites)
            {
                sprite.loadFromImagePathPreserveObjectSize();
                //MainGFX.AddObject(sprite);
            }
            ThisLevel.AddSpritesTo(MainGFX);
            lstSprites.Items.Refresh();
        }



        private void cmdAddSprite_Click(object sender, RoutedEventArgs e)
        {
            //MainGFX.RemoveObject(ThisLevel.LevelBkg);

            // var name = await this.ShowInputAsync("Hello!", "What is your name?");
            // txtSpriteName.Text = name ?? "";
            //await this.ShowMessageAsync("Hello", "Hello " + result + "!");

            //var sobj= new SpriteObject(txtSpriteName.Text);

            var sobj = Activator.CreateInstance(dbNewObj.SelectedItem as Type) as SpriteObject;

            sobj.Name = txtSpriteName.Text;
            sobj.Position =  new Vector(GameSlider.Value, 100);
            //sobj.CenterOfMass = new Vector(0, 0);
            sobj.IsMovable = false;
            sobj.CanCollide = false;
            //ThisLevel.selectImage(sobj);
            ThisLevel.Sprites.Add(sobj);
            MainGFX.AddObject(sobj);
            lstSprites.Items.Refresh();
            txtSpriteName.Text = "";

            //lstSprites.Items.Add()
            //lstSprites.GroupStyle;
            //lstSprites.ItemsSource = ThisLevel.Sprites;
            //lstSprites.View = View.Details;
            //lstSprites.ItemTemplate.tex
        }


        private void Image_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("=> Clicked = " + sender.GetType().Name);
            ThisLevel.selectImage(lstSprites.SelectedItem as SpriteObject);
            lstSprites.Items.Refresh();

        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            GameSlider.Value =  (lstSprites.SelectedItem as SpriteObject).Position.X;
        }

        private void chkDrawShape_Checked(object sender, RoutedEventArgs e)
        {
            SpriteObject.DrawShape = true;
        }
        private void chkDrawShape_Unchecked(object sender, RoutedEventArgs e)
        {
            SpriteObject.DrawShape = false;
        }

        private void MainGFX_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SpriteObject obj = (lstSprites.SelectedItem as SpriteObject);
            if (obj != null)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                   // obj.Position = new Vector(e.GetPosition(MainGFX).X, e.GetPosition(MainGFX).Y);
                    obj.Position = (Vector)e.GetPosition(MainGFX) - MainGFX.DrawingOffset ;
                }
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    double newY = obj.SizeV.Y + (e.GetPosition(MainGFX) - objResizeRefpoint).Y;
                    if (newY < 1) newY = 1;
                    objResizeRefpoint = (Vector)e.GetPosition(MainGFX);
                    obj.ZoomPreserveAspectRatio(height: newY);
                }
            }
        }

        private void MainGFX_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            objResizeRefpoint = (Vector)e.GetPosition(MainGFX);
        }



        private void lstSprites_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            SpriteObject obj = (lstSprites.SelectedItem as SpriteObject);
            if (obj != null)
            {
                if (e.Key == Key.Delete)
                {
                    ThisLevel.Sprites.Remove(obj);
                    MainGFX.RemoveObject(obj);
                    lstSprites.Items.Refresh();
                }
            }
        }

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //SpriteObject obj = (lstSprites.SelectedItem as SpriteObject);
            //PropertyInspect pi = new PropertyInspect(obj);
            //pi.Show();
            //pi.Focus();
        }

        private void lstSprites_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SpriteObject obj = (lstSprites.SelectedItem as SpriteObject);
            PropertyInspect pi = new PropertyInspect(obj);
            pi.Show();
            Thread.Sleep(200);
            System.Windows.Forms.Application.DoEvents();
            pi.Focus();
        }


        #region Drag & Drop

        //private void lstSprites_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    // Store the mouse position
        //    dragDropPoint = e.GetPosition(null);
        //}

        //private void lstSprites_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    Point pos = e.GetPosition(null);
        //    Vector diff = dragDropPoint - pos;

        //    if (e.LeftButton == MouseButtonState.Pressed &&
        //       ( Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
        //        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
        //    {
        //        // Get dragged item
        //        System.Windows.Controls.ListView listView = sender as System.Windows.Controls.ListView;
        //        var listViewItem =
        //            FindAnchestor<System.Windows.Controls.ListViewItem>((DependencyObject)e.OriginalSource);

        //        // Find the data behind the ListViewItem
        //        SpriteObject sprite = listView.ItemContainerGenerator.ItemFromContainer(listViewItem) as SpriteObject;
 
        //        // Initialize the drag & drop operation
        //        System.Windows.DataObject dragData = new System.Windows.DataObject("SpriteObject", sprite );
        //        DragDrop.DoDragDrop(listViewItem, dragData, System.Windows.DragDropEffects.Move);
        //    }
        //}

        //private static T FindAnchestor<T>(DependencyObject current)
        //    where T : DependencyObject
        //{
        //    do
        //    {
        //        if (current is T)
        //        {
        //            return (T)current;
        //        }
        //        current = VisualTreeHelper.GetParent(current);
        //    }
        //    while (current != null);
        //    return null;
        //}

        void dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<SpriteObject> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.
            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            if (lowerIdx < 0)
            {
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
            ThisLevel.RemoveSpritesFrom(MainGFX);
            ThisLevel.AddSpritesTo(MainGFX);
        }

        void dragMgr_ProcessDropNewObj(object sender, ProcessDropEventArgs<SpriteObject> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.
            Debug.WriteLine("=> anderebox <");
            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            if (lowerIdx < 0)
            {
                Debug.WriteLine("=> anderebox = {0} {1}", lowerIdx, sender.GetType().Name);
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

        private void lstSprites_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Effects == System.Windows.DragDropEffects.None)
                return;

            //SpriteObject droppedData = e.Data.GetData(typeof(SpriteObject)) as SpriteObject;
            //SpriteObject target = ((System.Windows.Controls.ListViewItem)(sender)).DataContext as SpriteObject;

            //int removedIdx = lstSprites.Items.IndexOf(droppedData);
            //int targetIdx = lstSprites.Items.IndexOf(target);
            if (sender == this.lstSprites)
            {
                if (this.dragMgr.IsDragInProgress)
                    return;

               // so remove that item from the other ListView.
                //(this.listView2.ItemsSource as ObservableCollection<Task>).Remove(task);
            }
        }

        private void lstSprites_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Move;
        }

        #endregion

        private void cmdAddStaticCollider_Click(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine("=> dooooneeeeeeeeee= {0}" );
            return;
            Assembly assi = Assembly.GetAssembly(typeof(GFXContainer));
            var AnimatorTypes = from typ in assi.GetTypes()
                                where typeof(IAnimationRigidBody).IsAssignableFrom(typ) && !typ.IsInterface
                                select typ;

            var GameObjects = from typ in assi.GetTypes()
                              where typeof(SpriteObject).IsAssignableFrom(typ) && !typ.IsInterface
                              select typ;


           // lstNewObj.ItemsSource = typs;
            dbNewObj.ItemsSource = GameObjects;
            dbNewObj.SelectedIndex = 0;
            //lstNewObj.ItemsSource = GameObjects;

            //System.Windows.Forms.MessageBox.Show("Test: " + assi.FullName);
        }

        private async void MetroWindow_Closing_Async(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_shutdown;
            if (_shutdown) return;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Save & Quit",
                NegativeButtonText = "Quit",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("Quit Gamedesigner?",
                "Do you want to save changes?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            bool affirm = result == MessageDialogResult.Affirmative;
            if (affirm)
            {
                Debug.WriteLine("=> fwfghjrgeöerhgöerg = {0}");
                Thread.Sleep(2000);
            }

            _shutdown = true;
            Debug.WriteLine("=> result = {0}", result);

            if (_shutdown)
                this.Close();
                //System.Windows.Application.Current.Shutdown();
        }




    }
}
