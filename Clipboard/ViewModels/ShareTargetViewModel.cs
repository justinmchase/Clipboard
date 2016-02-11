using System;
using System.Collections.Generic;
using TaoOfLeo.Clipboard.Common;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;

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
            share.ReportStarted();
            await share.Data.CopyTo(_values);
        }

        public void Copy()
		{
			try
			{
                if (_share.Data.Properties.ApplicationName != Constants.ApplicationName)
                {
                    var content = new DataPackage();
                    _values.CopyTo(content);

                    Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(content);
                    Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
                }

                _share.ReportCompleted();
			}
			catch (Exception ex)
			{
                _share.ReportError(ex.Message);
            }
		}
	}
}
