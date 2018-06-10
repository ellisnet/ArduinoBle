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
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace KeyboardMenu.ViewModels
{
    public abstract class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService;
        protected IUserDialogs UserDialogService;

        protected bool IsDestroyed;

        private TaskCompletionSource<Tuple<bool, Exception>> _navCompletionSource;
        private SemaphoreSlim _navLocker = new SemaphoreSlim(1, 1);

        public string PageTitle { get; set; } = String.Empty;

        #region Helper methods

        protected void NotifyPropertyChanged(string propertyName) => RaisePropertyChanged(propertyName);

        protected async Task ShowOkMessage(string message, string title = null)
        {
            if (message == null) { throw new ArgumentNullException(nameof(message));}
            if (!String.IsNullOrWhiteSpace(message))
            {
                title = (String.IsNullOrWhiteSpace(title)) ? "App Message" : title.Trim();
                await UserDialogService.AlertAsync(message.Trim(), title, "OK");
            }
        }

        protected string DefaultPageTitle
        {
            get
            {
                string result = String.Empty;
                string name = GetType().Name.Replace("PageViewModel", "").Replace("ViewModel", "").Trim();

                foreach (char letter in name)
                {
                    result += (result != String.Empty && letter.ToString() == letter.ToString().ToUpper())
                        ? $" {letter}"
                        : letter.ToString();
                }

                return result;
            }
        }

        protected async Task NavigateTo(
            string name,
            NavigationParameters parameters = null,
            bool withNavigationPage = false,
            bool? useModalNavigation = null,
            bool animated = true)
        {
            if ((!String.IsNullOrWhiteSpace(name)) && _navLocker != null)
            {
                try
                {
                    await _navLocker.WaitAsync();

                    _navCompletionSource = new TaskCompletionSource<Tuple<bool, Exception>>();

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            if (withNavigationPage)
                            {
                                await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{name}",
                                    parameters, useModalNavigation, animated);
                            }
                            else
                            {
                                await NavigationService.NavigateAsync(name, parameters, useModalNavigation, animated);
                            }

                            _navCompletionSource.SetResult(Tuple.Create<bool, Exception>(true, null));
                        }
                        catch (Exception e)
                        {
                            _navCompletionSource.SetResult(Tuple.Create(false, e));
                        }
                    });

                    Tuple<bool, Exception> invokeResult = await _navCompletionSource.Task;
                    if (!invokeResult.Item1)
                    {
                        string exceptionMessage = $"The Navigation to '{name}' could not be completed.";
                        Exception exception = (invokeResult.Item2 == null)
                            ? new InvalidOperationException(exceptionMessage)
                            : new InvalidOperationException(exceptionMessage, invokeResult.Item2);
                        throw exception;
                    }
                }
                // ReSharper disable once RedundantCatchClause
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _navLocker?.Release();
                }
            }
        }

        protected Task NavigateToPage<T>(
            NavigationParameters parameters = null,
            bool withNavigationPage = false,
            bool? useModalNavigation = null,
            bool animated = true) where T : Page
            => NavigateTo(typeof(T).Name, parameters, withNavigationPage, useModalNavigation, animated);

        protected Task NavigateReplaceWith(
            string name,
            NavigationParameters parameters = null,
            bool withNavigationPage = false,
            bool? useModalNavigation = null,
            bool animated = true)
            => NavigateTo($"../{name}", parameters, withNavigationPage, useModalNavigation, animated);

        protected Task NavigateReplaceWithPage<T>(
            NavigationParameters parameters = null,
            bool withNavigationPage = false,
            bool? useModalNavigation = null,
            bool animated = true) where T : Page
            => NavigateReplaceWith(typeof(T).Name, parameters, withNavigationPage, useModalNavigation, animated);

        protected Task NavigateToNewRoot(
            string name,
            NavigationParameters parameters = null,
            bool withNavigationPage = false,
            bool? useModalNavigation = null,
            bool animated = true)
            => NavigateTo($"app:///{name}", parameters, withNavigationPage, useModalNavigation, animated);

        protected Task NavigateToNewRootPage<T>(
            NavigationParameters parameters = null,
            bool withNavigationPage = false,
            bool? useModalNavigation = null,
            bool animated = true) where T : Page
            => NavigateToNewRoot(typeof(T).Name, parameters, withNavigationPage, useModalNavigation, animated);

        protected async Task GoBack(
            NavigationParameters parameters = null,
            bool? useModalNavigation = null,
            bool animated = true)
        {
            if (_navLocker != null)
            {
                try
                {
                    await _navLocker.WaitAsync();

                    _navCompletionSource = new TaskCompletionSource<Tuple<bool, Exception>>();

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await NavigationService.GoBackAsync(parameters, useModalNavigation, animated);
                            _navCompletionSource.SetResult(Tuple.Create<bool, Exception>(true, null));
                        }
                        catch (Exception e)
                        {
                            _navCompletionSource.SetResult(Tuple.Create(false, e));
                        }
                    });

                    Tuple<bool, Exception> invokeResult = await _navCompletionSource.Task;
                    if (!invokeResult.Item1)
                    {
                        string exceptionMessage = $"Unable to navigate back.";
                        Exception exception = (invokeResult.Item2 == null)
                            ? new InvalidOperationException(exceptionMessage)
                            : new InvalidOperationException(exceptionMessage, invokeResult.Item2);
                        throw exception;
                    }
                }
                // ReSharper disable once RedundantCatchClause
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _navLocker?.Release();
                }
            }
        }

        protected async Task GoBackToRoot(NavigationParameters parameters = null)
        {
            if (_navLocker != null)
            {
                try
                {
                    await _navLocker.WaitAsync();

                    _navCompletionSource = new TaskCompletionSource<Tuple<bool, Exception>>();

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await NavigationService.GoBackToRootAsync(parameters);
                            _navCompletionSource.SetResult(Tuple.Create<bool, Exception>(true, null));
                        }
                        catch (Exception e)
                        {
                            _navCompletionSource.SetResult(Tuple.Create(false, e));
                        }
                    });

                    Tuple<bool, Exception> invokeResult = await _navCompletionSource.Task;
                    if (!invokeResult.Item1)
                    {
                        string exceptionMessage = $"Unable to navigate back.";
                        Exception exception = (invokeResult.Item2 == null)
                            ? new InvalidOperationException(exceptionMessage)
                            : new InvalidOperationException(exceptionMessage, invokeResult.Item2);
                        throw exception;
                    }
                }
                // ReSharper disable once RedundantCatchClause
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _navLocker?.Release();
                }
            }
        }

        protected T Resolve<T>(string name = null)
            => (String.IsNullOrWhiteSpace(name))
                ? (T) Resolve(typeof(T))
                : (T) Resolve(typeof(T), name);

        protected object Resolve(Type type) => ((App) Application.Current).AppConfigService.Resolve(type);

        protected object Resolve(Type type, string name) => ((App)Application.Current).AppConfigService.Resolve(type, name);

        #endregion

        protected ViewModelBase(
            INavigationService navigationService,
            IUserDialogs userDialogService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            UserDialogService = userDialogService ?? throw new ArgumentNullException(nameof(userDialogService));
        }

        #region INavigationAware implementation

        public virtual void OnNavigatedFrom(NavigationParameters parameters) { }

        public virtual void OnNavigatedTo(NavigationParameters parameters) { }

        public virtual void OnNavigatingTo(NavigationParameters parameters) { }

        #endregion

        #region IDestructible implementation

        public virtual async void Destroy()
        {
            if ((!IsDestroyed) && _navLocker != null)
            {
                await _navLocker.WaitAsync();
                _navLocker.Release();
                _navLocker.Dispose();
            }
            IsDestroyed = true;

            _navCompletionSource = null;
            _navLocker = null;
            NavigationService = null;
            UserDialogService = null;
        }

        #endregion
    }

    //  This special class will allow us to have IntelliSense while we are editing our XAML view files in Visual
    //  Studio with ReSharper.  I.e. it is for design-time only, and does nothing at compile-time or run-time.
    //  For more info, check these pages -
    //  https://github.com/PrismLibrary/Prism/issues/986
    //  https://gist.github.com/nuitsjp/7478bfc7eba0f2a25b866fa2e7e9221d
    //  https://blog.nuits.jp/enable-intellisense-for-viewmodel-members-with-prism-for-xamarin-forms-2f274e7c6fb6
    public static class DesignTimeViewModelLocator
    {
        public static MainPageViewModel MainPage => null;
        //public static ItemDetailViewModel ItemDetailPage => null;
        //public static ItemsViewModel ItemsPage => null;
        //public static NewItemViewModel NewItemPage => null;
    }
}
