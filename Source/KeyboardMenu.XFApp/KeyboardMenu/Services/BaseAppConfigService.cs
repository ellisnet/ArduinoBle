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
using KeyboardMenu.Interfaces;
using Prism.Ioc;

namespace KeyboardMenu.Services
{
    public abstract class BaseAppConfigService : IAppConfigService
    {
        #region IPlatformInitializer implementation

        public abstract void RegisterTypes(IContainerRegistry containerRegistry);

        #endregion

        #region IContainerProvider implementation

        // ReSharper disable once InconsistentNaming
        protected IContainerProvider container;

        public IContainerProvider Container
        {
            get
            {
                if (container == null)
                {
                    throw new InvalidOperationException($"The {nameof(Container)} property has not yet been set via the {nameof(SetContainer)} method.");
                }
                return container;
            }
        }

        public void SetContainer(IContainerProvider containerProvider)
        {
            container = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));
        }

        public object Resolve(Type type) => Container.Resolve(type);

        public object Resolve(Type type, string name) => Container.Resolve(type, name);

        public T Resolve<T>(string name = null)
        {
            var result = default(T);

            object item = (name == null) ? Resolve(typeof(T)) : Resolve(typeof(T), name);

            if (item is T typedItem)
            {
                result = typedItem;
            }

            return result;
        }

        #endregion
    }
}
