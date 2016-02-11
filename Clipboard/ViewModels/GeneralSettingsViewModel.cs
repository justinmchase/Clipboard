using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoOfLeo.Clipboard.Common;
using Windows.Storage;

namespace TaoOfLeo.Clipboard.ViewModels
{
    class GeneralSettingsViewModel
    {
        public Resources Resources
        {
            get { return Resources.Current; }
        }

        public bool AutoCopy
        {
            get
            {
                object value;
                if (!ApplicationData.Current.LocalSettings.Values.TryGetValue("AutoCopy", out value))
                    value = false;

                return (bool)value;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["AutoCopy"] = value;
            }
        }
    }
}
