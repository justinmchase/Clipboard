using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Xaml;

namespace TaoOfLeo.Clipboard.Common
{
    public sealed class Resources : ObservableObject
    {
        private static Lazy<Resources> resources = new Lazy<Resources>(() => new Resources());

        public static Resources Current
        {
            get { return resources.Value; }
        }

        private Resources()
        {
            ResourceManager.Current.DefaultContext.QualifierValues.MapChanged += OnMapChanged;
        }

        private static string GetString(Expression<Func<object>> property)
        {
            var name = ((MemberExpression)property.Body).Member.Name;
            return ResourceManager.Current.MainResourceMap.GetValue("resources/" + name).ValueAsString;
        }

        private void OnMapChanged(IObservableMap<string, string> sender, IMapChangedEventArgs<string> @event)
        {
            this.UpdateAll();
        }

        public string Copy
        {
            get { return GetString(() => Copy); }
        }

        public string Share
        {
            get { return GetString(() => Share); }
        }

        public string UnexpectedError
        {
            get { return GetString(() => UnexpectedError); }
        }

        public string PrivacyPolicyTitle
        {
            get { return GetString(() => PrivacyPolicyTitle); }
        }

        public string AboutTitle
        {
            get { return GetString(() => AboutTitle); }
        }

        public string ApplicationTitle
        {
            get { return GetString(() => ApplicationTitle); }
        }

        public string ApplicationDescription
        {
            get { return GetString(() => ApplicationDescription); }
        }

        public string AboutText
        {
            get { return GetString(() => AboutText); }
        }

        public string AboutAuthors
        {
            get { return GetString(() => AboutAuthors); }
        }

        public string AboutCopyright
        {
            get { return GetString(() => AboutCopyright); }
        }

        public string PrivacyPolicyText
        {
            get { return GetString(() => PrivacyPolicyText); }
        }

        public string GeneralSettingsTitle
        {
            get { return GetString(() => GeneralSettingsTitle); }
        }

        public string GeneralSettingsAutoShare
        {
            get { return GetString(() => GeneralSettingsAutoShare); }
        }
    }
}
