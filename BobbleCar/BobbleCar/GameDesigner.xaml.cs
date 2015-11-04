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

namespace BobbleCar
{
    /// <summary>
    /// Interaktionslogik für GameDesigner.xaml
    /// </summary>
    public partial class GameDesigner : MetroWindow
    {

        GameScroller Scroller;
        LevelSet ThisLevel = new LevelSet();
        Vector objResizeRefpoint = new Vector();
        Vector objMoveRefpoint = new Vector();
        GUIAnimator GUIAni;
        private bool _shutdown = false;

        ListViewDragDropManager<IGameObject> dragMgr;

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
            this.dragMgr = new ListViewDragDropManager<IGameObject>(lstSprites);
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

            SpriteObject.AnimatedByDefault = false;
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
            ThisLevel.Background.IsObstacle = false;
            ThisLevel.selectImage(ThisLevel.Background);
            (ThisLevel.Background as SpriteObject).ZoomPreserveAspectRatio(height: GameWrapper.Height);
            MainGFX.AddObject(ThisLevel.Background);
            MainGFX.Width = ThisLevel.Background.SizeV.X;
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
            ThisLevel.LevelBkg.IsObstacle = false;
            ThisLevel.selectImage(ThisLevel.LevelBkg);
            (ThisLevel.LevelBkg as SpriteObject).ZoomPreserveAspectRatio(height: GameWrapper.Height);
            (ThisLevel.Background as SpriteObject).ScrollScaling = (ThisLevel.Background.SizeV.X - (GameWrapper.Width/2))/ ThisLevel.LevelBkg.SizeV.X;
            MainGFX.AddObject(ThisLevel.LevelBkg);
            MainGFX.Width = ThisLevel.LevelBkg.SizeV.X;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // After everything has loaded, rendered and so on
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {
                lstSprites.ItemsSource = ThisLevel.Sprites;
                Scroller = new GameScroller(GameSlider, MainGFX, GameWrapper);
                MainGFX.Start();
            }));
        }

        private void cmdSaveLevel_Click(object sender, RoutedEventArgs e)
        {
            string filepath = Helper.SaveFile();
            if (filepath == null) return;
            ThisLevel.SaveLevel(filepath);
        }

        private void cmdLoadLevel_Click(object sender, RoutedEventArgs e)
        {
            string filepath = Helper.OpenFile();
            if (filepath == null) return;
            ThisLevel.ClearLevel(MainGFX);
            try
            {
                ThisLevel = LevelSet.LoadLevel(filepath);
                ThisLevel.BuildLevel(MainGFX);
                lstSprites.ItemsSource = ThisLevel.Sprites;
                lstSprites.Items.Refresh();
                GUIAni = new GUIAnimator(gfx: MainGFX,
                                         lvl: ThisLevel,
                                         timingSrc: TimingSource.Sources.CompositionTargetRendering,
                                         pgJumpRes: pgJumpResource);
            }
            catch (Exception ex)
            {
                MessageBox_Dispatched("Failed loading level description", ex.Message);
            }
        }



        private void cmdAddSprite_Click(object sender, RoutedEventArgs e)
        {

            var sobj = Activator.CreateInstance(dbNewObj.SelectedItem as Type) as SpriteObject;

            sobj.Name = txtSpriteName.Text;
            sobj.Position =  new Vector(GameSlider.Value, 100);
            //sobj.CenterOfMass = new Vector(0, 0);
            sobj.IsMovable = false;
            sobj.CanCollide = true;
            sobj.IsObstacle = false;
            //ThisLevel.selectImage(sobj);
            int idx = ThisLevel.Sprites.IndexOf(lstSprites.SelectedItem as SpriteObject);
            if ((idx >= 0) && (idx < ThisLevel.Sprites.Count - 1))
            {
                idx++;
                ThisLevel.Sprites.Insert(idx, sobj);
            } else
                ThisLevel.Sprites.Add(sobj);
            MainGFX.AddObject(sobj);
            lstSprites.Items.Refresh();
            txtSpriteName.Text = "";

        }


        private void Image_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Debug.WriteLine("=> Clicked = " + sender.GetType().Name);
            ThisLevel.selectImage(lstSprites.SelectedItem as SpriteObject);
            lstSprites.Items.Refresh();

        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SpriteObject obj = (lstSprites.SelectedItem as SpriteObject);
            if (obj != null)
            {
                GameSlider.Value = obj.Position.X;
                foreach (var sprite in ThisLevel.Sprites)
                {
                    if (sprite != null)
                        sprite.Highlight = false;
                }
                obj.Highlight = true;
            }
            
            // Results in Errors due to selection changes !!!
            //switch (Keyboard.Modifiers)
            //{
            //    case ModifierKeys.Alt:
            //        break;
            //    case ModifierKeys.Control:
            //        obj.Animated = !obj.Animated;
            //        break;
            //    case ModifierKeys.None:
            //        GameSlider.Value = obj.Position.X;
            //        break;
            //    case ModifierKeys.Shift:
            //        break;
            //    case ModifierKeys.Windows:
            //        break;
            //    default:
            //        break;
            //}
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
                switch (Keyboard.Modifiers)
                {
                    case ModifierKeys.Alt:
                        break;
                    case ModifierKeys.Control:
                        obj.Position += 0.2 * ((Vector)e.GetPosition(MainGFX) - objMoveRefpoint);
                        objMoveRefpoint = (Vector)e.GetPosition(MainGFX);
                        break;
                    case ModifierKeys.None:
                        objMoveRefpoint = (Vector)e.GetPosition(MainGFX);
                        break;
                    case ModifierKeys.Shift:
                        if ((e.RightButton == MouseButtonState.Released) && (e.LeftButton == MouseButtonState.Pressed))
                            obj.Position = (Vector)e.GetPosition(MainGFX) - MainGFX.DrawingOffset;
                        break;
                    case ModifierKeys.Windows:
                        break;
                    default:
                        break;
                }
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    double newY = obj.SizeV.Y + (e.GetPosition(MainGFX) - objResizeRefpoint).Y;
                    double newX = obj.SizeV.X + (e.GetPosition(MainGFX) - objResizeRefpoint).X;
                    if (newY < 1) newY = 1;
                    if (newX < 1) newX = 1;
                    objResizeRefpoint = (Vector)e.GetPosition(MainGFX);
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                        obj.SizeV = new Vector(newX, newY);
                    else
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
                // Space not possible
                switch (e.Key)
                {
                    case Key.Delete:
                        ThisLevel.Sprites.Remove(obj);
                        MainGFX.RemoveObject(obj);
                        lstSprites.Items.Refresh();
                        break;
                    case Key.Enter:
                        obj.Animated = !obj.Animated;
                        lstSprites.Items.Refresh();
                        break;
                    default:
                        break;
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

        void dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<IGameObject> e)
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

        private void cmdRefreshCollider_Click(object sender, RoutedEventArgs e)
        {
            ThisLevel.ClearCollider(MainGFX);
            ThisLevel.InitializeCollider(MainGFX);
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
                cmdSaveLevel_Click(this, new RoutedEventArgs());
            }

            _shutdown = true;

            if (_shutdown)
            {
                MainGFX.Dispose();
                System.Windows.Application.Current.Shutdown();
                //this.Close();
            }
        }

        private void MainGFX_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                Visual v = MainGFX.GetObjectXY((Point)e.GetPosition(MainGFX)) as Visual;
                if (v != null)
                {
                    var res = from s in ThisLevel.Sprites
                              where s.ContainsVisual(v)
                              select s;
                    try
                    {
                        var spriteSel = res.First();
                        lstSprites.SelectedItem = spriteSel;
                        foreach (var sprite in ThisLevel.Sprites)
                        {
                            if (sprite != null)
                                sprite.Highlight = false;
                        }
                        spriteSel.Highlight = true;
                    }
                    catch (Exception) { }
                }
            }
        }

        private void DesignerWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            MainGFX.RaiseWindowKeyDown(this, e);
        }

        private void DesignerWindow_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            MainGFX.RaiseWindowKeyUp(this, e);
        }

        public void MetroWindow_MessageBox(string title, string text)
        {
            Action<string, string> del = async (_title, _text) => { await this.ShowMessageAsync(_title, _text); };
            del(title, text);
        }

        public void MessageBox_Dispatched(string title, string text)
        {
            //this.Dispatcher.Invoke(MetroWindow_MessageBox,(object)title, (object)text));
            this.Dispatcher.Invoke(new Action(() => MetroWindow_MessageBox(title, text)));
        }




    }
}
