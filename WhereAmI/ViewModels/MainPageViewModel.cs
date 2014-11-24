using System;
using System.Windows.Media;
using Caliburn.Micro;
using WhereAmI.Resources;

using System.Windows;
using System.Collections.Generic;
using System.Device.Location;
using Windows.Devices.Geolocation;
using WhereAmI.Common;

using Telerik.Windows.Controls;
using Microsoft.Phone.Maps.Services;
using System.Text;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using WhereAmI.ViewModels;
using System.Threading.Tasks;
using Windows.Foundation;

namespace WhereAmI
{
    public class MainPageViewModel : Screen
    {
        private readonly BackgroundImageBrush backgroundImageBrush;
        private readonly INavigationService navigationService;
        private readonly ILog logger;
        private readonly Share share;
        private readonly ReverseGeocodeQuery reverseGeocode = new ReverseGeocodeQuery();
        private MapAddress locationAddress;
        private readonly FlipTileCreator flipTileCreator;
        private readonly ScreenshotExtractor screenshotExtractor;
        private readonly ShareSessionsStorage shareSessionsStorage;
        private readonly WhereAmISettingsManager settingsManager;
        private bool locationMessageShown;
        private readonly Geolocator geolocator = new Geolocator();


        public MainPageViewModel(BackgroundImageBrush backgroundImageBrush, 
            INavigationService navigationService, 
            Share share, 
            ILog logger, 
            FlipTileCreator flipTileCreator,
            ScreenshotExtractor screenshotExtractor,
            ShareSessionsStorage shareSessionsStorage,
            WhereAmISettingsManager settingsManager)
        {
            this.backgroundImageBrush = backgroundImageBrush;
            this.navigationService = navigationService;
            this.logger = logger;
            this.share = share;
            this.flipTileCreator = flipTileCreator;
            this.screenshotExtractor = screenshotExtractor;
            this.shareSessionsStorage = shareSessionsStorage;
            this.settingsManager = settingsManager;

            PinButtonText = "pin tile";
            ShareButtonText = "share";
            LocationButtonText = "get location";
            AboutButtonText = "about";
            SettingsButtonText = "settings";
            HelpButtonText = "help";
            ClearSharesText = "reset shares";

            LocationButtonIcon = new Uri("Assets/GetLocationIcon.png", UriKind.Relative);
            ShareButtonIcon = new Uri("Assets/ShareIcon.png", UriKind.Relative);
            PinButtonIcon = new Uri("Assets/PinIcon.png", UriKind.Relative);
            PrivacyButtonText = "privacy";

            CenterOfMap = new GeoCoordinate(1.44, 5.33);

            geolocator.DesiredAccuracyInMeters = 250;
            geolocator.MovementThreshold = 20;
            geolocator.StatusChanged += geolocator_StatusChanged;

            reverseGeocode.QueryCompleted += reverseGeocode_QueryCompleted;
        }

        private bool locationServicesEnabled;

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            if (args.Status == PositionStatus.Disabled)
            {
                locationServicesEnabled = false;
            }
            else
            {
                locationServicesEnabled = true;
            }
        }

        private bool gotLocationOnce = false;

        protected override async void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (!locationMessageShown)
            {
                locationMessageShown = true;
                if (!settingsManager.IsLocationServiceAllowed)
                {
                    MessageBoxClosedEventArgs result = await RadMessageBox.ShowAsync("Where am I need your location in order to work. You can turn this off at any time under the settings menu.", "Allow Where Am I to access and use your location?", MessageBoxButtons.YesNo);
                    if (result.ButtonIndex == 0)
                    {
                        settingsManager.IsLocationServiceAllowed = true;
                        if (!gotLocationOnce)
                        {
                            await GetLocation();
                            gotLocationOnce = true;
                        }
                    }
                }
                else
                {
                    if (!gotLocationOnce)
                    {
                        await GetLocation();
                        gotLocationOnce = true;
                    }
                }
            }
            else
            {
                if (settingsManager.IsLocationServiceAllowed)
                {
                    if (!gotLocationOnce)
                    {
                        await GetLocation();
                        gotLocationOnce = true;
                    }
                }
            }
        }

        public void ViewChanged(MapViewChangedEventArgs e)
        {
            if (shareSessionsStorage.GetNumberOfShareSessions() == 0)
            {
                if (view.map.ActualWidth != 0)
                {
                    screenshotExtractor.Save(view.map, (int)view.map.ActualWidth, (int)view.map.ActualWidth, shareSessionsStorage.GetNumberOfShareSessionsInTheLast24Hour());
                    flipTileCreator.UpdateDefaultTile();
                }
            }
        }

        void reverseGeocode_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    view.map.SetView(centerOfMap, 13, Microsoft.Phone.Maps.Controls.MapAnimationKind.Parabolic);
                    DrawMapMarkers();
                });
                locationAddress = e.Result[0].Information.Address;
            }
            catch (Exception error)
            {
                logger.Error(error);
            }
            finally
            {
                ShowBusyIndicator = false;
            }
        }

        private MainPage view;

        protected override void OnViewLoaded(object loadedView)
        {
            base.OnViewLoaded(loadedView);
            view = loadedView as MainPage;
        }

        private void DrawMapMarkers()
        {
            // Draw marker for current position
            if (CenterOfMap != null)
            {
                Pushpin pushpin = (Pushpin)view.FindName("locationPushPin");
                if (pushpin != null)
                {
                    pushpin.GeoCoordinate = CenterOfMap;
                    pushpin.Content = CreatePushPinMessage();
                }
            }
        }

        private object CreatePushPinMessage()
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(locationAddress.Street))
            {
                builder.AppendFormat("{0} {1}, ", locationAddress.Street, locationAddress.HouseNumber);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(locationAddress.District))
                {
                    builder.AppendFormat("{0}, ", locationAddress.District);
                }
            }

            if (!string.IsNullOrWhiteSpace(locationAddress.City))
            {
                builder.AppendFormat("{0}", locationAddress.City);
            }

            return builder.ToString();
        }

        private GeoCoordinate centerOfMap = new GeoCoordinate();

        public GeoCoordinate CenterOfMap
        {
            get { return centerOfMap; }
            set 
            {
                centerOfMap = value;
                NotifyOfPropertyChange(() => CenterOfMap);
            }
        }

        private bool showBusyIndicator;

        public bool ShowBusyIndicator
        {
            get { return showBusyIndicator; }
            set
            {
                showBusyIndicator = value;
                NotifyOfPropertyChange(() => ShowBusyIndicator);
            }
        }

        public ImageBrush BackgroundImageBrush
        {
            get { return backgroundImageBrush.GetBackground(); }
        }

        public string ApplicationName
        {
            get { return AppResources.ApplicationTitle;  }
        }

        public string PageTitle
        {
            get { return AppResources.MainPageTitle; }
        }

        public void Privacy()
        {
            var uri = navigationService.UriFor<PrivacyViewModel>().BuildUri();
            navigationService.Navigate(uri);
        }

        public void About()
        {
            navigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        public void Settings()
        {
            var uri = navigationService.UriFor<SettingsViewModel>().BuildUri();
            navigationService.Navigate(uri);
        }

        public void Help()
        {
            var uri = navigationService.UriFor<HelpViewModel>().BuildUri();
            navigationService.Navigate(uri);
        }

        public async void Location()
        {
            if (settingsManager.IsLocationServiceAllowed)
            {
                await GetLocation();
            }
        }

        private bool alreadyRunning = false;

        private bool locationServicesEnabledMessageShown;

        private async Task GetLocation()
        {
            if (!alreadyRunning)
            {
                alreadyRunning = true;
                ShowBusyIndicator = true;
                try
                {
                    var result = await GetSinglePositionAsync();
                    if (result != null)
                    {
                        CenterOfMap = CoordinateConverter.ConvertGeocoordinate(result);

                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            if (!reverseGeocode.IsBusy)
                            {
                                reverseGeocode.GeoCoordinate = centerOfMap;
                                reverseGeocode.QueryAsync();
                            }
                        });
                    }
                    else
                    {
                        if (!locationServicesEnabled)
                        {
                            if (!locationServicesEnabledMessageShown)
                            {
                                locationServicesEnabledMessageShown = true;
                                await RadMessageBox.ShowAsync("Your location services are turned off. To turn them on, go to location in your phone settings", "Location services are off", MessageBoxButtons.OK);
                            }
                        }
                    }
                }
                finally
                {
                    ShowBusyIndicator = false;
                    alreadyRunning = false;
                }
            }
        }

        private string aboutButtonText;

        public string AboutButtonText 
        {
            get { return aboutButtonText; }
            set
            {
                aboutButtonText = value;
                NotifyOfPropertyChange(() => AboutButtonText);
            }
        }

        private string shareButtonText;

        public string ShareButtonText 
        {
            get { return shareButtonText; }
            set
            {
                shareButtonText = value;
                NotifyOfPropertyChange(() => ShareButtonText);
            }
        }

        private string privacyButtonText;

        public string PrivacyButtonText
        {
            get { return privacyButtonText; }
            set
            {
                privacyButtonText = value;
                NotifyOfPropertyChange(() => PrivacyButtonText);
            }
        }

        private string pinButtonText;

        public string PinButtonText
        {
            get { return pinButtonText; }
            set
            {
                pinButtonText = value;
                NotifyOfPropertyChange(() => PinButtonText);
            }
        }

        private string locationButtonText;

        public string LocationButtonText
        {
            get { return locationButtonText; }
            set
            {
                locationButtonText = value;
                NotifyOfPropertyChange(() => PinButtonText);
            }
        }

        private string settingsButtonText;

        public string SettingsButtonText
        {
            get { return settingsButtonText; }
            set
            {
                settingsButtonText = value;
                NotifyOfPropertyChange(() => SettingsButtonText);
            }
        }

        private string helpButtonText;

        public string HelpButtonText
        {
            get { return helpButtonText; }
            set
            {
                helpButtonText = value;
                NotifyOfPropertyChange(() => HelpButtonText);
            }
        }

        private Uri locationButtonIcon;

        public Uri LocationButtonIcon
        {
            get { return locationButtonIcon; }
            set
            {
                locationButtonIcon = value;
                NotifyOfPropertyChange(() => LocationButtonIcon);
            }
        }

        private Uri shareButtonIcon;

        public Uri ShareButtonIcon
        {
            get { return shareButtonIcon; }
            set
            {
                shareButtonIcon = value;
                NotifyOfPropertyChange(() => ShareButtonIcon);
            }
        }

        private Uri pinButtonIcon;

        public Uri PinButtonIcon
        {
            get { return pinButtonIcon; }
            set
            {
                pinButtonIcon = value;
                NotifyOfPropertyChange(() => ShareButtonIcon);
            }
        }

        private string clearSharesText;

        public string ClearSharesText
        {
            get { return clearSharesText; }
            set
            {
                clearSharesText = value;
                NotifyOfPropertyChange(() => ClearSharesText);
            }
        }

        public void ClearShares()
        {
            shareSessionsStorage.Reset();
            screenshotExtractor.Save(view.map, (int)view.map.ActualWidth, (int)view.map.ActualWidth, shareSessionsStorage.GetNumberOfShareSessionsInTheLast24Hour());
            flipTileCreator.UpdateTile();
            flipTileCreator.UpdateDefaultTile();
            MessageBox.Show("The number of shares have been reset.");
        }

        public async void ShareIt()
        {
            List<string> buttons = new List<string>();
            buttons.Add("Email");
            buttons.Add("Sms");
            buttons.Add("Social");

            MessageBoxClosedEventArgs result = await RadMessageBox.ShowAsync( buttons, "Share your location via?", vibrate: true);

            string message = BuildMessage();
            if (result.ButtonIndex == 0)
            {
                UpdateSessionAndTile();
                share.ShareEmail(message);
            }
            else if (result.ButtonIndex == 1)
            {
                UpdateSessionAndTile();
                share.ShareSMS(message);
            }
            else if (result.ButtonIndex == 2)
            {
                UpdateSessionAndTile();
                share.ShareStatus(message);
            }
        }

        private void UpdateSessionAndTile()
        {
            shareSessionsStorage.AddSession();
            screenshotExtractor.Save(view.map, (int)view.map.ActualWidth, (int)view.map.ActualWidth, shareSessionsStorage.GetNumberOfShareSessionsInTheLast24Hour());
            flipTileCreator.UpdateTile();
            flipTileCreator.UpdateDefaultTile();
        }

        public void PinIt()
        {
            flipTileCreator.CreateTile();
        }

        private string BuildMessage()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("I am at ");

            if (locationAddress != null)
            {
                if (!string.IsNullOrWhiteSpace(locationAddress.Street))
                {
                    builder.AppendFormat("{0}, ", locationAddress.Street);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(locationAddress.District))
                    {
                        builder.AppendFormat("{0}, ", locationAddress.District);
                    }
                }

                if (!string.IsNullOrWhiteSpace(locationAddress.City))
                {
                    builder.AppendFormat("{0}, ", locationAddress.City);
                }

                if (!string.IsNullOrWhiteSpace(locationAddress.Country))
                {
                    builder.AppendFormat("{0} ", locationAddress.Country);
                }

                builder.AppendFormat("(Lat {1:0.000}, Long {0:0.000})", CenterOfMap.Longitude, CenterOfMap.Latitude);
            }

            return builder.ToString();
        }

        public async Task<Windows.Devices.Geolocation.Geocoordinate> GetSinglePositionAsync()
        {
            IAsyncOperation<Geoposition> locationTask = null;
            try
            {
                locationTask = geolocator.GetGeopositionAsync();
                Geoposition myGeoposition = await locationTask;
                return myGeoposition.Coordinate;
            }
            catch (Exception error)
            {
                logger.Error(error);
                return null;
            }
            finally
            {
                if (locationTask != null)
                {
                    if (locationTask.Status == AsyncStatus.Started)
                        locationTask.Cancel();

                    locationTask.Close();
                }
            }

        }
    }
}