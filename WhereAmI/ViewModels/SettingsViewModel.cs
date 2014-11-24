using Caliburn.Micro;

using System.Windows.Media;
using WhereAmI.Common;
using WhereAmI.Resources;

namespace WhereAmI.ViewModels
{
    public class SettingsViewModel: Screen
    {
        private readonly BackgroundImageBrush backgroundImageBrush;
        private readonly WhereAmISettingsManager settingsManager;

        public SettingsViewModel(
            BackgroundImageBrush backgroundImageBrush, 
            WhereAmISettingsManager settingsManager)
        {
            this.backgroundImageBrush = backgroundImageBrush;
            this.settingsManager = settingsManager;
        }

        public ImageBrush BackgroundImageBrush
        {
            get { return backgroundImageBrush.GetBackground(); }
        }

        public string ApplicationName
        {
            get { return AppResources.ApplicationTitle; }
        }

        public string PageTitle
        {
            get { return AppResources.SettingsPageTitle; }
        }

        public bool Location
        {
            get { return settingsManager.IsLocationServiceAllowed; }
            set 
            { 
                settingsManager.IsLocationServiceAllowed = value;
                NotifyOfPropertyChange(() => Location);
            }
        }

        public void ResetShares()
        {

        }
    }
}
