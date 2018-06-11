/* Copyright 2018 Ellisnet - Jeremy Ellis (jeremy@ellisnet.com)
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
       http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace KeyboardMenu.Converters
{
    /// <summary>
    /// Path to Embedded Resource image (e.g. images/kitten/photo.jpg) => resource:// URI based on resource ID (e.g. MyApp.images.kitten.photo.jpg)
    /// Note that the Embedded Resource must be in the same assembly as the Xamarin.Forms.Application.Current (i.e. sub-class of CodeBrixApplication).
    /// </summary>
    public class EmbeddedResourcePathToUriConverter : IValueConverter
    {
        private static Assembly appResourceAssembly;
        private static Assembly AppResourceAssembly => appResourceAssembly ??
            (appResourceAssembly = Assembly.GetExecutingAssembly());

        private static string appResourceAssemblyName;
        private static string AppResourceAssemblyName => appResourceAssemblyName ??
            (appResourceAssemblyName = AppResourceAssembly.GetName().FullName.Split(',')[0].Trim());

        public string GetAppResourcePath(string filePath)
        {
            filePath = filePath?.Trim() ?? throw new ArgumentNullException(nameof(filePath));
            if (filePath == "") { throw new ArgumentOutOfRangeException(nameof(filePath)); }
            return $"{AppResourceAssemblyName}.{filePath.Replace('/', '.')}";
        }

        private string GetAppResourceUri(string filePath)
        {
            return $"resource://{GetAppResourcePath(filePath)}?assembly={AppResourceAssembly.FullName}";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value == null)
                ? null
                : GetAppResourceUri((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
