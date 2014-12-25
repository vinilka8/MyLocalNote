using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace My_Local_Note
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // PhoneProductId="d25ccc27-4f2c-4dba-b315-97764ae1fc49"
        Geolocator geolocator;

        public MainPage()
        {
            this.InitializeComponent();
            MapService.ServiceToken = "zzA11mz1oON29_nJ4B6OEQ";
            this.NavigationCacheMode = NavigationCacheMode.Required;
            geolocator = new Geolocator();
            
        }
  
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        async void myLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource cts;
                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                var locator = new Geolocator();
                geolocator.DesiredAccuracyInMeters = 30;
                geolocator.MovementThreshold = 100;
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                var position = await geolocator.GetGeopositionAsync();
                await myMap.TrySetViewAsync(position.Coordinate.Point, 18D);
                //geolocator.PositionChanged += Location_PositionChanged;


                Geoposition g = await geolocator.GetGeopositionAsync().AsTask(token);
                Geopoint geopoint = new Geopoint(new BasicGeoposition()
                {
                    Latitude = g.Coordinate.Point.Position.Latitude,
                    Longitude = g.Coordinate.Point.Position.Longitude



                });
                myMap.Center = geopoint;
                myMap.ZoomLevel = 18;
                DrawCarIcon(geopoint);

                mySlider.Value = myMap.ZoomLevel;
            }
            catch
            {

            }
        }

        private void DrawCarIcon(Geopoint pos)
        {
            const int carZIndewxz = 4;

            var carIcon = myMap.MapElements.OfType<MapIcon>().FirstOrDefault(p => p.ZIndex == carZIndewxz);
            if (carIcon == null)
            {
                carIcon = new MapIcon
                {
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    ZIndex = carZIndewxz
                };
                carIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/liveTile150x.png"));
                myMap.MapElements.Add(carIcon);
            }
            carIcon.Location = pos;
        }

        private void TakeNote_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddMapNote));
        }

        private void myNotes_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NotePage));
        }

        private void btnTileUpdates_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LiveTileMenu));
        }

        private void mySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (myMap != null) myMap.ZoomLevel = e.NewValue;
        }
    }
}
