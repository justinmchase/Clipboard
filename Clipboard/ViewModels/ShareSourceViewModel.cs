using System.Collections.Generic;
using System.Threading.Tasks;
using TaoOfLeo.Clipboard.Common;
using Windows.ApplicationModel.DataTransfer;
using SystemClipboard = Windows.ApplicationModel.DataTransfer.Clipboard;

namespace TaoOfLeo.Clipboard.ViewModels
{
	public class ShareSourceViewModel : ObservableObject
    {
        public Resources Resources
        {
            get { return Resources.Current; }
        }
	}
}
