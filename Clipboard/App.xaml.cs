using TaoOfLeo.Clipboard.Common;
using TaoOfLeo.Clipboard.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TaoOfLeo.Clipboard
{
	sealed partial class App : Application
	{
		public App()
		{
			this.InitializeComponent();

			this.UnhandledException += OnUnhandledException;
            this.Suspending += App_Suspending;
		}

        void App_Suspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // Fails app verification without implementing this.
            deferral.Complete();
        }

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
            // Do not crash.
            e.Handled = true;

            // Global error handler.
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
                rootFrame.Navigate(typeof(MainPage), e.Exception);
		}

		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			if (args.PreviousExecutionState == ApplicationExecutionState.Running)
			{
				Window.Current.Activate();
				return;
			}

			var rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null)
				Window.Current.Content = rootFrame = new Frame();

			if (!rootFrame.Navigate(typeof(ShareSource)))
				rootFrame.Navigate(typeof(MainPage));

			Window.Current.Activate();
		}

		protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
		{
			var rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null)
				Window.Current.Content = rootFrame = new Frame();

			if (!rootFrame.Navigate(typeof(ShareTarget), args.ShareOperation))
				rootFrame.Navigate(typeof(MainPage));

			Window.Current.Activate();
		}
	}
}
