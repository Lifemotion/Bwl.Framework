
// Copyright 2016 Igor Koshelev (igor@lifemotion.ru)

// Licensed under the Apache License, Version 2.0 (the "License");
// you may Not use this file except In compliance With the License.
// You may obtain a copy Of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law Or agreed To In writing, software
// distributed under the License Is distributed On an "AS IS" BASIS,
// WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
// See the License For the specific language governing permissions And
// limitations under the License.

using System.Runtime.Versioning;
using Microsoft.Win32;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public class RegistryStorage
    {
        public class Setting
        {
            private RegistryStorage _storage;
            internal Setting(RegistryStorage storage, string name)
            {
                _storage = storage;
                Name = name;
            }
            public string Name { get; private set; }
            public string DefaultValue { get; set; }
            public string Value
            {
                get
                {
                    return (string)(_storage.Key.GetValue(Name, DefaultValue));
                }
                set
                {
                    _storage.Key.SetValue(Name, value);
                }
            }
        }

        private RegistryKey _key;

        public RegistryStorage()
        {
            _key = GetRegistryAppKey();
        }

        public RegistryStorage(string keyName)
        {
            _key = GetRegistryKey(keyName);
        }

        public RegistryKey Key
        {
            get
            {
                return _key;
            }
        }

        public Setting CreateSetting(string name, string defaultValue)
        {
            var setting = new Setting(this, name);
            setting.DefaultValue = defaultValue;
            return setting;
        }

        public static RegistryKey GetRegistryKey(string name)
        {
            var key = Registry.CurrentUser.OpenSubKey(name, true);
            if (key is null)
            {
                key = Registry.CurrentUser.CreateSubKey(name);
            }
            return key;
        }

        public static RegistryKey GetRegistryAppKey()
        {
            return GetRegistryKey("Bwl " + Application.ProductName);
        }
    }
}