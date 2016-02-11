using System;
using TaoOfLeo.Clipboard.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TaoOfLeo.Clipboard.Common;
using DataTransferManager = Windows.ApplicationModel.DataTransfer.DataTransferManager;
using SystemClipboard = Windows.ApplicationModel.DataTransfer.Clipboard;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Resources.Core;

namespace TaoOfLeo.Clipboard.Views
{
    public sealed partial class ShareSource : Page
    {
        private SettingsPane pane = SettingsPane.GetForCurrentView();
        private DataTransferManager currentDataTransferManager;
        private ShareSourceViewModel viewModel;

        private bool copied;
        private Dictionary<string, object> values = new Dictionary<string, object>();

        public ShareSource()
        {
            this.InitializeComponent();

            this.viewModel = (ShareSourceViewModel)this.DataContext;
            this.Loaded += ShareSource_Loaded;
            this.Unloaded += ShareSource_Unloaded;

            this.LostFocus += UpdateContent;
            this.PointerMoved += UpdateContent;
            this.ManipulationStarted += UpdateContent;
            this.KeyDown += UpdateContent;
        }

        void ShareSource_LostFocus(object sender, RoutedEventArgs e)
        {
            copied = false;
        }

        async void UpdateContent(object sender, RoutedEventArgs e)
        {
            if (!copied) // && this.FocusState != FocusState.Unfocused)
            {
                copied = true;
                await SetContent();
            }
        }

        void OnSettingsCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand(
                "about", 
                this.viewModel.Resources.AboutTitle, 
                c => (Window.Current.Content as Frame).Navigate(typeof(About))));

            args.Request.ApplicationCommands.Add(new SettingsCommand(
                "privacy", 
                this.viewModel.Resources.PrivacyPolicyTitle, 
                c => (Window.Current.Content as Frame).Navigate(typeof(Privacy))));

            args.Request.ApplicationCommands.Add(new SettingsCommand(
                "general",
                this.viewModel.Resources.GeneralSettingsTitle,
                c => (Window.Current.Content as Frame).Navigate(typeof(GeneralSettings))));
        }

        void ShareSource_Unloaded(object sender, RoutedEventArgs e)
        {
            this.currentDataTransferManager.DataRequested -= currentDataTransferManager_DataRequested;
            this.pane.CommandsRequested -= OnSettingsCommandsRequested;
            SystemClipboard.ContentChanged -= SystemClipboard_ContentChanged;
            this.pane = null;
        }

        void ShareSource_Loaded(object sender, RoutedEventArgs e)
        {
            this.pane = SettingsPane.GetForCurrentView();
            this.pane.CommandsRequested += OnSettingsCommandsRequested;

            this.currentDataTransferManager = DataTransferManager.GetForCurrentView();
            this.currentDataTransferManager.DataRequested += currentDataTransferManager_DataRequested;

            SystemClipboard.ContentChanged += SystemClipboard_ContentChanged;
        }

        void SystemClipboard_ContentChanged(object sender, object e)
        {
            this.copied = false;
        }

        private async void Share(object sender, RoutedEventArgs e)
        {
            await SetContent();
            if (ApplicationView.Value != ApplicationViewState.Snapped)
            {
                DataTransferManager.ShowShareUI();
            }
            else if (ApplicationView.TryUnsnap())
            {
                await this.Dispatcher.RunIdleAsync(args => DataTransferManager.ShowShareUI());
            }
        }

        /// <summary>
        /// This is horribly ghetto but you can only get the clipboard content from certain events. 
        /// Get from button clicks or mouse moves or manipulation started events.
        /// If you try to do this from the wrong event you will get an AccessDenied exception... but only if
        /// the debugger is not attached. You have to get all of the content before the DataRequested event
        /// is fired, so we cache it all into the values dictionary.
        /// </summary>
        private async Task SetContent()
        {
            this.ErrorText.Text = "";
            values.Clear();
            try
            {
                var content = SystemClipboard.GetContent();
                await content.CopyTo(values);
            }
            catch (Exception ex)
            {
                this.SetErrorMessage(ex.Message);
                this.copied = false;
                values.Clear();
            }
        }

        private void currentDataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            this.ErrorText.Text = "";
            var deferral = args.Request.GetDeferral();
            try
            {
                values.CopyTo(args.Request.Data);
            }
            catch (Exception ex)
            {
                this.SetErrorMessage(ex.Message);
                args.Request.FailWithDisplayText(ex.Message);
                this.copied = false;
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void SetErrorMessage(string errorMessage)
        {
#if DEBUG
            this.ErrorText.Text = errorMessage;
#else
            this.ErrorText.Text = this.viewModel.Resources.UnexpectedError;
#endif
        }
    }
}