using System;
using TaoOfLeo.Clipboard.ViewModels;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TaoOfLeo.Clipboard.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ShareTarget : Page
	{
		private ShareTargetViewModel _viewModel;

		public ShareTarget()
		{
			this.InitializeComponent();
			this.DataContext = (_viewModel = new ShareTargetViewModel());
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			var args = (ShareOperation)e.Parameter;
			_viewModel.Initialize(args);
		}

		private void Copy(object sender, RoutedEventArgs e)
		{
			_viewModel.Copy();
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			object autocopy;
			if (!ApplicationData.Current.LocalSettings.Values.TryGetValue("AutoCopy", out autocopy))
			{
				autocopy = false;
			}

			if (autocopy is bool && (bool)autocopy)
			{
				_viewModel.Copy();
				Window.Current.Close();
			}
		}
	}
}
