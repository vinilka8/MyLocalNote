using My_Local_Note.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace My_Local_Note
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddMapNote : Page
    {
        private bool isViewingPozition = false;
        private MapNote mapNote;

        public AddMapNote()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Geopoint myPoint;
            CancellationTokenSource cts;
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            if (e.Parameter == null)
            {
                isViewingPozition = false;

                //add staff to notes
                var locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;
                var position = await locator.GetGeopositionAsync().AsTask(token);
                myPoint = position.Coordinate.Point;
                var pos = new Geopoint(new BasicGeoposition { Latitude = position.Coordinate.Point.Position.Latitude, Longitude = position.Coordinate.Point.Position.Longitude });
                DrawManIcon(pos);

            }
            else
            {
                isViewingPozition = true;
                //view or delete from notes
                mapNote = (MapNote)e.Parameter;
                titleTextBox.Text = mapNote.Title;
                noteTextBox.Text = mapNote.Note;
                addButton.Content = "Delete";

                var myPosition = new Windows.Devices.Geolocation.BasicGeoposition();
                myPosition.Latitude = mapNote.Latitude;
                myPosition.Longitude = mapNote.Longtitude;

                myPoint = new Geopoint(myPosition);

                DrawManIcon(myPoint);

            }
            await myMap.TrySetViewAsync(myPoint, 16D);
            //Geoposition g = await myPoint.GetGeopositionAsync().AsTask(token);
            

        }

        private void DrawManIcon(Geopoint pos)
        {
            const int manZIndewxz = 4;

            var manIcon = myMap.MapElements.OfType<MapIcon>().FirstOrDefault(p => p.ZIndex == manZIndewxz);
            if (manIcon == null)
            {
                manIcon = new MapIcon
                {
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    ZIndex = manZIndewxz
                };
                manIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/myPos.png"));
                myMap.MapElements.Add(manIcon);
            }
            manIcon.Location = pos;
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Delete")
            {
                //if user ckick on DELETE command
                //delete a data
                //from Json file
                App.DataModel.DeleteMapNote(mapNote);
                Frame.Navigate(typeof(NotePage));
            }
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (isViewingPozition)
            {
                //if user would like to delete
                //the message will pops up
                //on the screen
                var messageDialog = new Windows.UI.Popups.MessageDialog("Do you really want to delete?");
                //with 2 choses delete or cancel hahha :)
                messageDialog.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                //same as true and false
                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 1;
                //show a box :P
                await messageDialog.ShowAsync();
            }
            else
            {
                MapNote newMapNote = new MapNote();
                newMapNote.Title = titleTextBox.Text;
                newMapNote.Note = noteTextBox.Text;
                //newMapNote.Created = DataTime.now;
                newMapNote.Latitude = myMap.Center.Position.Latitude;
                newMapNote.Longtitude = myMap.Center.Position.Longitude;
                App.DataModel.AddMapNote(newMapNote);
                Frame.Navigate(typeof(NotePage));
            }
            
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NotePage));
        }
    }
}
