using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TaoOfLeo.Clipboard.Views
{
	public sealed partial class GeneralSettings : Page
	{
		public GeneralSettings()
		{
			ApplicationViewBehavior.SetApplicationViewBehavior(this, new ApplicationViewBehavior());
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}

		private void GoBack(object sender, RoutedEventArgs e)
		{
			(Window.Current.Content as Frame).GoBack();
		}
	}
}
