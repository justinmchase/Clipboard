using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace TaoOfLeo.Clipboard.Common
{
	public static class DataExtensions
	{
        private static readonly HashSet<string> InvalidFormats = new HashSet<string>();

        public static void CopyTo(this IDictionary<string, object> values, DataPackage package)
        {
            package.RequestedOperation = DataPackageOperation.Copy;
            package.Properties.ApplicationName = Constants.ApplicationName;
            package.Properties.Title = Resources.Current.ApplicationTitle;
            package.Properties.Description = Resources.Current.ApplicationDescription;
            package.Properties.ApplicationListingUri = Constants.ApplicationListingUri;

            foreach (var format in values.Keys)
            {
                try
                {
                    if (format == StandardDataFormats.Bitmap)
                    {
                        package.SetBitmap((RandomAccessStreamReference)values[format]);
                    }
                    else if (format == StandardDataFormats.Html)
                    {
                        package.SetHtmlFormat((string)values[format]);
                    }
                    else if (format == StandardDataFormats.Rtf)
                    {
                        package.SetRtf((string)values[format]);
                    }
                    else if (format == StandardDataFormats.StorageItems)
                    {
                        var files = (IEnumerable<IStorageItem>)values[format];
                        package.SetStorageItems(files, true);
                    }
                    else if (format == StandardDataFormats.Text)
                    {
                        package.SetText((string)values[format]);
                    }
                    else if (format == StandardDataFormats.WebLink)
                    {
                        package.SetWebLink((Uri)values[format]);
                    }
                    else if (format == StandardDataFormats.ApplicationLink)
                    {
                        package.SetApplicationLink((Uri)values[format]);
                    }
                    else if (format == "extensions")
                    {
                        var extensions = (string[])values["extensions"];
                        foreach (var ext in extensions)
                            package.Properties.FileTypes.Add(ext);
                    }
                    else if (ValidFormat(format))
                    {
                        package.SetData(format, values[format]);
                    }
                }
                catch (SecurityException) { throw; }
                catch
                {
                    // Some formats are incompatible with WinRT and can cause it to throw unexpectedly.
                    // guard against it and give a best effort copy of the available formats.
                }
            }
        }

        public static async Task CopyTo(this DataPackageView view, IDictionary<string, object> values)
        {
            foreach (var format in view.AvailableFormats)
            {
                try
                {
                    if (format == StandardDataFormats.Bitmap)
                    {
                        var value = await view.GetBitmapAsync();
                        values.Add(format, value);
                    }
                    else if (format == StandardDataFormats.Html)
                    {
                        var value = await view.GetHtmlFormatAsync();
                        values.Add(format, value);

                        if (!values.ContainsKey(StandardDataFormats.Text))
                            values.Add(StandardDataFormats.Text, value);
                    }
                    else if (format == StandardDataFormats.Rtf)
                    {
                        var value = await view.GetRtfAsync();
                        values.Add(format, value);

                        if (!values.ContainsKey(StandardDataFormats.Text))
                            values.Add(StandardDataFormats.Text, value);
                    }
                    else if (format == StandardDataFormats.StorageItems)
                    {
                        var value = await view.GetStorageItemsAsync();
                        var extensions = value.Select(f => "*" + Path.GetExtension(f.Name)).ToArray();
                        values.Add("extensions", extensions);
                        values.Add(format, value);

                        if (value.Count() == 1)
                        {
                            var file = value.OfType<StorageFile>().SingleOrDefault();
                            if(file != null && file.Attributes.HasFlag(FileAttributes.Temporary))
                            {
                                var copiedFile = await file.CopyAsync(
                                    ApplicationData.Current.LocalFolder,
                                    file.Name,
                                    NameCollisionOption.ReplaceExisting);

                                values[format] = new[] { copiedFile };
                            }

                            if (!values.ContainsKey(StandardDataFormats.Bitmap))
                            {
                                var stream = RandomAccessStreamReference.CreateFromFile(file);
                                values.Add(StandardDataFormats.Bitmap, stream);
                            }
                        }
                    }
                    else if (format == StandardDataFormats.Text)
                    {
                        var value = await view.GetTextAsync();
                        values.Add(format, value);
                    }
                    else if (format == StandardDataFormats.ApplicationLink)
                    {
                        var value = await view.GetApplicationLinkAsync();
                        values.Add(format, value);
                        if (!values.ContainsKey(StandardDataFormats.Text))
                            values.Add(StandardDataFormats.Text, value.ToString());
                    }
                    else if (format == StandardDataFormats.WebLink)
                    {
                        var value = await view.GetWebLinkAsync();
                        values.Add(format, value);

                        if (!values.ContainsKey(StandardDataFormats.Text))
                            values.Add(StandardDataFormats.Text, value.ToString());
                    }
                    else if (ValidFormat(format))
                    {
                        var value = await view.GetDataAsync(format);
                        values.Add(format, value);
                    }
                }
                catch (SecurityException) { throw; }
                catch
                {
                    // Some formats are incompatible with WinRT and can cause it to throw unexpectedly.
                    // guard against it and give a best effort copy of the available formats that are supported.
                }
            }
        }

        static DataExtensions()
        {
            //-- This format comes from office documents such as OneNote. It's not
            // supported by the WinRT Clipboard bindings and therefore should be ignored when
            // attempting to be shared.
            InvalidFormats.Add(@"EnhancedMetafile");
        }

        private static bool ValidFormat(string format)
        {
            return !InvalidFormats.Contains(format);
        }
	}
}
