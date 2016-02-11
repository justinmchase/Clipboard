using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TaoOfLeo.Clipboard.Views
{
	class ApplicationViewBehavior : DependencyObject
	{
		public static readonly DependencyProperty ApplicationViewBehaviorProperty = DependencyProperty.RegisterAttached(
			"ApplicationViewBehavior",
			typeof(ApplicationViewBehavior),
			typeof(ApplicationViewBehavior),
			new PropertyMetadata(null, OnApplicationViewBehaviorPropertyChanged));

		private static void OnApplicationViewBehaviorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var page = (Page)sender;
			if (e.OldValue != null)
				((ApplicationViewBehavior)e.OldValue).Detach(page);

			if (e.NewValue != null)
				((ApplicationViewBehavior)e.NewValue).Attach(page);
		}

		public static ApplicationViewBehavior GetApplicationViewBehavior(Page page)
		{
			return (ApplicationViewBehavior)page.GetValue(ApplicationViewBehaviorProperty);
		}

		public static void SetApplicationViewBehavior(Page page, ApplicationViewBehavior value)
		{
			page.SetValue(ApplicationViewBehaviorProperty, value);
		}

		private Page _attached;

		private void Attach(Page page)
		{
			_attached = page;
			page.SizeChanged += OnSizeChanged;
		}

		private void Detach(Page page)
		{
			_attached = null;
			page.SizeChanged -= OnSizeChanged;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (ApplicationView.Value.ToString() == "Snapped")
			{
				VisualStateManager.GoToState(_attached, "Snapped", false);
			}
			else
			{
				VisualStateManager.GoToState(_attached, "Filled", false);
			}
		}
	}
}
