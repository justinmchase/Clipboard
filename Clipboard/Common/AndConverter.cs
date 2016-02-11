using System;
using Windows.UI.Xaml.Data;

namespace TaoOfLeo.Clipboard.Common
{
	public class AndConverter : IValueConverter
	{
		public IValueConverter Left { get; set; }
		public IValueConverter Right { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			value = Left.Convert(value, targetType, parameter, language);
			return Right.Convert(value, targetType, parameter, language);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			value = Right.ConvertBack(value, targetType, parameter, language);
			return Left.ConvertBack(value, targetType, parameter, language);
		}
	}
}
