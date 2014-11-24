using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WhereAmI.Resources;

namespace WhereAmI.ViewModels
{
    public class HelpViewModel : Screen
    {
        private readonly BackgroundImageBrush backgroundImageBrush;

        public HelpViewModel(BackgroundImageBrush backgroundImageBrush)
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
            get { return AppResources.HelpPageTitle; }
        }

    }
}
