using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Caliburn.Micro.BindableAppBar;
using System.Windows.Controls;
using WhereAmI.Common;
using WhereAmI.ViewModels;

namespace WhereAmI
{
    public class Bootstrapper : PhoneBootstrapper
    {
        private PhoneContainer container;
        
        public Bootstrapper()
        {
            LogManager.GetLog = type => new DebugLogger(type);
        }

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            return new TransitionFrame();
        }

        protected override void Configure()
        {
            container = new PhoneContainer();

            container.RegisterPhoneServices(RootFrame);
            container.PerRequest<PrivacyViewModel>();
            container.PerRequest<MainPageViewModel>();
            container.PerRequest<SettingsViewModel>();
            container.PerRequest<HelpViewModel>();
            container.PerRequest<BackgroundImageBrush>();
            container.PerRequest<Share>();
            container.PerRequest<FlipTileCreator>();
            container.PerRequest<Storage>();
            container.PerRequest<ScreenshotExtractor>();
            container.PerRequest<ShareSessionsStorage>();
            container.PerRequest<SettingsHelper>();
            container.PerRequest<WhereAmISettingsManager>();

            container.RegisterSingleton(typeof(ILog), "", typeof(DebugLogger));

            AddCustomConventions();
        }

        static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<BindableAppBarButton>(
                Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(
                Control.IsEnabledProperty, "DataContext", "Click");
        }

        protected override void OnActivate(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Bootstrapper", "Activate", null, 0);
            base.OnActivate(sender, e);
        }

        protected override void OnLaunch(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Bootstrapper", "Launch", null, 0);
            base.OnLaunch(sender, e);
        }

        protected override void OnDeactivate(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Bootstrapper", "Deactivate", null, 0);
            base.OnDeactivate(sender, e);
        }

        protected override void OnClose(object sender, Microsoft.Phone.Shell.ClosingEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Bootstrapper", "Close", null, 0);
            base.OnClose(sender, e);
        }

        protected override void OnUnhandledException(object sender, System.Windows.ApplicationUnhandledExceptionEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendException(e.ExceptionObject.Message, false);
            base.OnUnhandledException(sender, e);
        }
        
        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}