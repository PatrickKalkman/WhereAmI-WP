using System.Windows.Media;
using Caliburn.Micro;
using WhereAmI.Resources;

namespace WhereAmI.ViewModels
{
    public class PrivacyViewModel : Screen
    {
        private readonly BackgroundImageBrush backgroundImageBrush;

        public PrivacyViewModel(BackgroundImageBrush backgroundImageBrush)
        {
            this.backgroundImageBrush = backgroundImageBrush;
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
            get { return AppResources.PrivacyPageTitle; }
        }
    }
}