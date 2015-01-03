using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class LiveTileMenu : Page
    {
        public LiveTileMenu()
        {
            this.InitializeComponent(); 
        }

        private void CycleTile_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);

            var tileImage = tileXml.GetElementsByTagName("image")[0] as XmlElement;
            tileImage.SetAttribute("src", "ms-appx:///Assets/logo150x150.png");
            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

            tileImage.SetAttribute("src", "ms-appx:///Assets/Logo.scale-240.png");
            tileNotification = new TileNotification(tileXml);
            tileNotification.Tag = "myTag";
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        private void BadgeTile_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            var badgeXML = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
            var badge = badgeXML.SelectSingleNode("/badge") as XmlElement;
            badge.SetAttribute("src", "ms-appx:///Assets/logoBadge58x58.scale-240.png");
            var badgeNotification = new BadgeNotification(badgeXML);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeNotification);

        }


        //JUST ONE WORKED PART :((( OTHER JUST IGNOR ME :(
        async void SecondaryTile_Click(object sender, RoutedEventArgs e)
        {

            var secondaryTile = new SecondaryTile(
                   "secondaryTileId",
                   "Text shown on tile",
                   "secondTileArguments",
                   new Uri("ms-appx:///Assets/logo150x150.png", UriKind.Absolute),
                   TileSize.Square150x150);

            bool isPinned = await secondaryTile.RequestCreateAsync();
        }

        private void BackHome_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
