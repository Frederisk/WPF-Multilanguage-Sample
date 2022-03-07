using MultiLanguage.Common;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace MultiLanguage {

    public class MainWindowViewModel : BindableBase {
        private const String ApplicationDefaultLanguage = "en-US";

        public MainWindowViewModel() {
            // Load language optional item;
            this._languageCollection = new() {
                new("en-US", "English (US)"),
                new("zh-TW", "繁體中文（台灣）"),
                new("zh-CN", "简体中文（中国）")
            };
            // Initialize with the system language,
            // if it fails, use the default language.
            var cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
            // this.SelectedLanguage = LoadLanguageResourceDictionary(cultureName) is null ? "en-US" : cultureName;
            if (this._languageCollection.Any(item => item.Tag == cultureName)) {
                this.SelectedLanguage = cultureName;
            } else {
                this.SelectedLanguage = ApplicationDefaultLanguage;
            }
            this.UpdateApplicationLanguage();
        }

        #region BindingSource

        private ObservableCollection<LanguageTypeInfo> _languageCollection;

        /// <summary>
        /// The Collection of optional language.
        /// </summary>
        public ObservableCollection<LanguageTypeInfo> LanguageCollection {
            get => this._languageCollection;
            set => this._languageCollection = value;
        }

        private String? _selectedLanguage;

        /// <summary>
        /// The Language which was selected.
        /// </summary>
        public String? SelectedLanguage {
            get => this._selectedLanguage;
            set {
                if (this.SetProperty(ref this._selectedLanguage, value)) {
                    this.ApplyLanguage.RaiseCanExecuteChanged();
                }
            }
        }

        private DelegateCommand? _applyLanguage;

        /// <summary>
        /// Apply language change.
        /// If the activity language is the same as
        /// <see cref="MainWindowViewModel.SelectedLanguage"/>, Application
        /// is not allowed.
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/questions/68572536/icommand-canexecutechanged-is-always-null
        /// </remarks>
        public DelegateCommand ApplyLanguage => _applyLanguage ?? (_applyLanguage = new(() => {
            this.UpdateApplicationLanguage();
            this.ApplyLanguage.RaiseCanExecuteChanged();
        }, () => {
            var dictionaryResources = Application.Current.Resources;
            if (dictionaryResources["Language_Code"] is String lang) {
                return SelectedLanguage != lang;
            }
            return false;
        }));

        #endregion BindingSource

        /// <summary>
        /// The class which used to store language infomation.
        /// </summary>
        public class LanguageTypeInfo {

            /// <summary>
            /// Initializes a new instance of the <see cref="LanguageTypeInfo"/> class.
            /// </summary>
            /// <param name="tag"></param>
            /// <param name="content"></param>
            public LanguageTypeInfo(String tag, String content) {
                this.Tag = tag;
                this.Content = content;
            }

            /// <summary>
            /// Language name to obtain the file.
            /// </summary>
            public String Tag { get; }

            /// <summary>
            /// Language name to display.
            /// </summary>
            public String Content { get; }
        }

        /// <summary>
        /// Get the <see cref="ResourceDictionary"/> of language.
        /// </summary>
        /// <param name="lang">Language Name, pass <see langword="null"/> to get the default language.</param>
        /// <returns>a <see cref="ResourceDictionary"/> which path is
        /// <c>$"/Resource/Language/{<paramref name="lang"/>}.xaml"</c>.
        /// if it does not exist, return default language one.</returns>
        private static ResourceDictionary? LoadLanguageResourceDictionary(String? lang = null) {
            try {
                // if is null or blank string, set lang as default.
                lang = String.IsNullOrWhiteSpace(lang) ? ApplicationDefaultLanguage : lang;
                var langUri = new Uri($@"\Resource\Language\{lang}.xaml", UriKind.Relative);
                return Application.LoadComponent(langUri) as ResourceDictionary;
            }
            // The file does not exist.
            catch (IOException) {
                return null;
            }
        }

        /// <summary>
        /// Update the Application Language to <see cref="MainWindowViewModel.SelectedLanguage"/>.
        /// </summary>
        private void UpdateApplicationLanguage() {
            ResourceDictionary? langResource = LoadLanguageResourceDictionary(this.SelectedLanguage);
            if (langResource is null) {
                // Default lang
                langResource = LoadLanguageResourceDictionary();
            }
            // If you have used other languages, clear it first.
            // Since the dictionary are cleared, the output of debugging will warn "Resource not found",
            // but it is not a problem in most case.
            System.Diagnostics.Debug.WriteLine("Clearing MergedDictionaries");
            Application.Current.Resources.MergedDictionaries.Clear();
            System.Diagnostics.Debug.WriteLine("Cleared");
            // Add new language.
            Application.Current.Resources.MergedDictionaries.Add(langResource);
        }
    }
}
