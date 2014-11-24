using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereAmI.Common
{
    public class WhereAmISettingsManager
    {
        private readonly SettingsHelper settingsHelper;

        private const string UseLocation = "UseLocation";

        public WhereAmISettingsManager(SettingsHelper settingsHelper)
        {
            this.settingsHelper = settingsHelper;
        }

        public bool IsLocationServiceAllowed
        {
            get
            {
                return this.settingsHelper.GetSetting(UseLocation, false);
            }

            set
            {
                this.settingsHelper.UpdateSetting(UseLocation, value);
            }
        }
    }

}
