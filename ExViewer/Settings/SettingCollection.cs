﻿using ApplicationDataManager.Settings;
using ExClient;
using ExClient.Settings;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace ExViewer.Settings
{
    public class SettingCollection : ApplicationSettingCollection
    {
        public static SettingCollection Current
        {
            get;
            private set;
        } = new SettingCollection();

        private SettingCollection()
            : base("Settings") { }

        public void Apply()
        {
            var clientSettings = Client.Current.Settings;
            this.HahAddress = this.HahAddress;
            this.ExcludedTagNamespaces = this.ExcludedTagNamespaces;
            this.ExcludedLanguages = this.ExcludedLanguages;
            this.VisitEx = this.VisitEx;
        }

        [Setting("Searching", Index = 10)]
        public string DefaultSearchString
        {
            get
            {
                return GetRoaming("");
            }
            set
            {
                SetRoaming(value);
            }
        }

        [Setting("Searching", Index = 20)]
        [CustomTemplate("CatagorySettingTemplate")]
        public Category DefaultSearchCategory
        {
            get
            {
                return GetRoaming(Category.All);
            }
            set
            {
                SetRoaming(value);
            }
        }

        [Setting("Searching", Index = 30)]
        public bool SaveLastSearch
        {
            get
            {
                return GetRoaming(false);
            }
            set
            {
                SetRoaming(value);
            }
        }

        [Setting("Searching", Index = 35)]
        [CustomTemplate("ExcludedTagNamespacesTemplate")]
        public Namespace ExcludedTagNamespaces
        {
            get
            {
                return GetRoaming(Namespace.Unknown);
            }
            set
            {
                SetRoaming(value);
                Client.Current.Settings.ExcludedTagNamespaces.Value = value;
            }
        }

        [Setting("Searching", Index = 40)]
        [CustomTemplate("ExcludedLanguagesTemplate")]
        public string ExcludedLanguages
        {
            get
            {
                return GetRoaming("");
            }
            set
            {
                SetRoaming(value);
                var el = Client.Current.Settings.ExcludedLanguages;
                el.Clear();
                el.AddRange(ExcludedLanguagesSettingProvider.FromString(value));
            }
        }

        [Setting("Global", Index = -10)]
        [EnumRepresent("ApplicationTheme")]
        public ApplicationTheme Theme
        {
            get
            {
                return GetLocal(ApplicationTheme.Dark);
            }
            set
            {
                SetLocal(value);
                ((FrameworkElement)Window.Current.Content).RequestedTheme = value.ToElementTheme();
                Themes.ThemeExtention.SetTitleBar();
            }
        }

        [Setting("Global", Index = 10)]
        public bool NeedVerify
        {
            get
            {
                return GetLocal(false);
            }
            set
            {
                SetLocal(value);
            }
        }

        [Setting("Global", Index = 15)]
        [ToggleSwitchRepresent("BooleanEx", "BooleanEh")]
        public bool VisitEx
        {
            get
            {
                return GetRoaming(false);
            }
            set
            {
                SetRoaming(value);
                Client.Current.Host = value ? HostType.Exhentai : HostType.Ehentai;
            }
        }

        [Setting("Global", Index = 20)]
        [ToggleSwitchRepresent("BooleanJT", "BooleanDT")]
        public bool UseJapaneseTitle
        {
            get
            {
                return GetRoaming(false);
            }
            set
            {
                SetRoaming(value);
            }
        }

        [Setting("Global", Index = 40)]
        [ToggleSwitchRepresent(PredefinedToggleSwitchRepresent.EnabledDisabled)]
        public bool UseChineseTagTranslation
        {
            get
            {
                return GetRoaming(false);
            }
            set
            {
                SetRoaming(value);
            }
        }

        [Setting("Global", Index = 50)]
        [ToggleSwitchRepresent(PredefinedToggleSwitchRepresent.EnabledDisabled)]
        public bool UseJapaneseTagTranslation
        {
            get
            {
                return GetRoaming(false);
            }
            set
            {
                SetRoaming(value);
            }
        }

        [Setting("ImageViewing", Index = 10)]
        [SingleRange(1, 4, Small = 0.1)]
        public float DefaultFactor
        {
            get
            {
                return GetLocal(2f);
            }
            set
            {
                SetLocal(value);
            }
        }

        [Setting("ImageViewing", Index = 20)]
        [SingleRange(4, 10, Small = 0.1)]
        public float MaxFactor
        {
            get
            {
                return GetLocal(8f);
            }
            set
            {
                SetLocal(value);
            }
        }

        [Setting("ImageViewing", Index = 30)]
        [ToggleSwitchRepresent(PredefinedToggleSwitchRepresent.EnabledDisabled)]
        public bool MouseInertial
        {
            get
            {
                return GetLocal(false);
            }
            set
            {
                SetLocal(value);
            }
        }

        [Setting("ImageViewing", Index = 35)]
        [ToggleSwitchRepresent("BooleanRightToLeft", "BooleanLeftToRight")]
        public bool ReverseFlowDirection
        {
            get
            {
                return GetLocal(false);
            }
            set
            {
                SetLocal(value);
            }
        }

        [Setting("ImageViewing", Index = 40)]
        [Int32Range(0, 1000, Tick = 100, Small = 10, Large = 100)]
        public int ChangeCommandBarDelay
        {
            get
            {
                return GetLocal(150);
            }
            set
            {
                SetLocal(value);
            }
        }

        [Setting("ImageViewing", Index = 45)]
        public bool KeepScreenOn
        {
            get
            {
                return GetLocal(false);
            }
            set
            {
                SetLocal(value);
            }
        }

        [Setting("Connection", Index = 10)]
        [ToggleSwitchRepresent(PredefinedToggleSwitchRepresent.YesNo)]
        public bool LoadLofiOnMeteredInternetConnection
        {
            get
            {
                return GetLocal(true);
            }
            set
            {
                if(this.LoadLofiOnAllInternetConnection)
                    ForceSetLocal(true);
                else
                    ForceSetLocal(value);
            }
        }

        [Setting("Connection", Index = 20)]
        [ToggleSwitchRepresent(PredefinedToggleSwitchRepresent.YesNo)]
        public bool LoadLofiOnAllInternetConnection
        {
            get
            {
                return GetLocal(false);
            }
            set
            {
                SetLocal(value);
                if(value)
                    this.LoadLofiOnMeteredInternetConnection = true;
            }
        }

        [Setting("Hah", Index = 10)]
        public string HahAddress
        {
            get
            {
                return GetLocal("");
            }
            set
            {
                var old = GetLocal("");
                if(string.IsNullOrWhiteSpace(value))
                    value = "";
                try
                {
                    Client.Current.Settings.HahProxy.AddressAndPort = value;
                    ForceSetLocal(Client.Current.Settings.HahProxy.AddressAndPort);
                }
                catch(System.Exception ex)
                {
                    ForceSetLocal(old);
                    Views.RootControl.RootController.SendToast(ex, null);
                }
            }
        }

        [Setting("About", Index = int.MaxValue)]
        [CustomTemplate("AboutContentTemplate")]
        public object AboutContent
        {
            get; set;
        }
    }
}
