using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TaoOfLeo.Clipboard.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class About : Page
	{
		public About()
		{
			ApplicationViewBehavior.SetApplicationViewBehavior(this, new ApplicationViewBehavior());
			this.InitializeComponent();
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}

		private void GoBack(object sender, RoutedEventArgs e)
		{
			(Window.Current.Content as Frame).GoBack();
		}

		private async void hyperlinkButton_Click(object sender, RoutedEventArgs e)
		{
			var success = await Windows.System.Launcher.LaunchUriAsync(new Uri("http://winrtclipboard.codeplex.com/discussions"));
			if (!success)
			{
				(Window.Current.Content as Frame).Navigate(typeof(MainPage)); // unexpected error
			}
		}
	}
}
