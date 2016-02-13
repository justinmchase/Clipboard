using System;
using System.Collections.Generic;
using TaoOfLeo.Clipboard.Common;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using System.Diagnostics;

namespace TaoOfLeo.Clipboard.ViewModels
{
	public class ShareTargetViewModel : ObservableObject
	{
        private ShareOperation _share;
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public Resources Resources
        {
            get { return Resources.Current; }
        }

        public async void Initialize(ShareOperation share)
        {
            _share = share;
            _share.ReportStarted();
            await _share.Data.CopyTo(_values);
            _share.ReportDataRetrieved();
        }

        public void Copy()
		{
			try
			{
                if (_share.Data.Properties.ApplicationName != Constants.ApplicationName)
                {
                    _share.ReportSubmittedBackgroundTask();

                    var content = new DataPackage();
                    _values.CopyTo(content);

                    Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
                    Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
                    Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += OnClipboardContentChanged;

                    _share.DismissUI();
                }
                else
                {
                    _share.ReportCompleted();
                }
			}
			catch (Exception ex)
			{
                _share.ReportError(ex.Message);
            }
		}

        private void OnClipboardContentChanged(object sender, object e)
        {
            if (_share != null)
            {
                try
                {
                    _share.ReportCompleted();
                    _share = null;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                    _share.ReportError(ex.ToString());
                }
            }
        }
	}
}
