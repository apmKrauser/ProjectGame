using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BobbleCar
{
    public static class GUIHelper
    {

        //public static Task<MessageDialogResult> ShowMessageAsync(this MetroWindow window, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        //{
        //    window.Dispatcher.VerifyAccess();
        //    return HandleOverlayOnShow(settings, window).ContinueWith(z =>
        //    {
        //        return (Task<MessageDialogResult>)window.Dispatcher.Invoke(new Func<Task<MessageDialogResult>>(() =>
        //        {
        //            if (settings == null)
        //            {
        //                settings = window.MetroDialogOptions;
        //            }

        //            //create the dialog control
        //            var dialog = new MessageDialog(window, settings)
        //            {
        //                Message = message,
        //                Title = title,
        //                ButtonStyle = style
        //            };

        //            SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
        //            dialog.SizeChangedHandler = sizeHandler;

        //            return dialog.WaitForLoadAsync().ContinueWith(x =>
        //            {
        //                if (DialogOpened != null)
        //                {
        //                    window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs())));
        //                }

        //                return dialog.WaitForButtonPressAsync().ContinueWith(y =>
        //                {
        //                    //once a button as been clicked, begin removing the dialog.

        //                    dialog.OnClose();

        //                    if (DialogClosed != null)
        //                    {
        //                        window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs())));
        //                    }

        //                    Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
        //                    return closingTask.ContinueWith(a =>
        //                    {
        //                        return ((Task)window.Dispatcher.Invoke(new Func<Task>(() =>
        //                        {
        //                            window.SizeChanged -= sizeHandler;

        //                            window.RemoveDialog(dialog);

        //                            return HandleOverlayOnHide(settings, window);
        //                        }))).ContinueWith(y3 => y).Unwrap();
        //                    });
        //                }).Unwrap();
        //            }).Unwrap().Unwrap();
        //        }));
        //    }).Unwrap();
        //}

    }
}
