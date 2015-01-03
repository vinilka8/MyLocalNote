using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI.Core;
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
        private List<ManeuverDescription> maneuverList = new List<ManeuverDescription>();

        public MainPage()
        {
            this.InitializeComponent();
            MapService.ServiceToken = "zzA11mz1oON29_nJ4B6OEQ";
            this.NavigationCacheMode = NavigationCacheMode.Required;
            geolocator = new Geolocator();
            updateMyMap();           
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

        private async void updateMyMap()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 10;

            Geoposition position = await geolocator.GetGeopositionAsync(
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(30));

            var myGPSCenter = new Geopoint( new BasicGeoposition { 
                Latitude = position.Coordinate.Point.Position.Latitude, 
                Longitude = position.Coordinate.Point.Position.Longitude });

            await myMap.TrySetViewAsync(myGPSCenter, 17D);

        }


        private async void myLocation_Click(object sender, RoutedEventArgs e)
        {               
            try
            {
                CancellationTokenSource cts;
                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                var locator = new Geolocator();

                geolocator.DesiredAccuracyInMeters = 10;
                geolocator.DesiredAccuracy = PositionAccuracy.High;
                geolocator.MovementThreshold = 1;

                var position = await geolocator.GetGeopositionAsync().AsTask(token);
                await myMap.TrySetViewAsync(position.Coordinate.Point, 17D);
                //Geoposition g = await geolocator.GetGeopositionAsync().AsTask(token);
                var pos = new Geopoint(new BasicGeoposition { Latitude = position.Coordinate.Point.Position.Latitude, Longitude = position.Coordinate.Point.Position.Longitude });
                MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(pos);
                if (result.Status == MapLocationFinderStatus.Success)
                {
                    TName.Text = "Town: " + result.Locations[0].Address.Town;
                }


                DrawManIcon(pos);

                locator.PositionChanged += Location_PositionChanged;
                mySlider.Value = myMap.ZoomLevel;

               
            }
            catch
            {

            }
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


        async void Location_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    //gives us a position that we can use
                    Geoposition position = args.Position;
                    //LatitudeValue.Text = position.Coordinate.Latitude.ToString();
                    //LongitudeValue.Text = position.Coordinate.Longitude.ToString();
                    var pos = new Geopoint(new BasicGeoposition
                    {
                        Latitude = position.Coordinate.Point.Position.Latitude,
                        Longitude = position.Coordinate.Point.Position.Longitude
                    });
                    DrawManIcon(pos);
                }
                );
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

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //takes the app stuff out of memory when exited
            geolocator.PositionChanged -= Location_PositionChanged;
            base.OnNavigatingFrom(e);
        }
    }
}
