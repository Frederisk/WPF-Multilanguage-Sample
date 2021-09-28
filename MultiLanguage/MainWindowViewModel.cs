using MultiLanguage.Common;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace MultiLanguage {
    public class MainWindowViewModel : BindableBase {
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
            this.SelectedLanguage = LoadLanguageResourceDictionary(cultureName) is null ? "en-US" : cultureName;
            this.UpdateApplicationLanguage();
        }

        #region Fields

        public ObservableCollection<LanguageTypeInfo> _languageCollection;
        public String? _selectedLanguage;

        #endregion Fields

        #region BindingSource

        /// <summary>
        /// The Collection of optional language.
        /// </summary>
        public ObservableCollection<LanguageTypeInfo> LanguageCollection {
            get => this._languageCollection;
            set => this._languageCollection = value;
        }

        /// <summary>
        /// The Language which was selected.
        /// </summary>
        public String? SelectedLanguage {
            get => this._selectedLanguage;
            set => this.SetProperty(ref this._selectedLanguage, value);

        }

        /// <summary>
        /// Apply language change.
        /// </summary>
        public DelegateCommand ApplyLanguage => new(_ => this.UpdateApplicationLanguage());
        // summary: If the activity language is the same as
        // <see cref="MainWindowViewModel.SelectedLanguage"/>, Application
        // is not allowed.
        //
        //_ => {
        //var dictionarySourceString = Application.Current.Resources.MergedDictionaries.Last();
        //var activityLanguage = Regex.Match(dictionarySourceString, @"(?<=\\)[^\\]*?\.xaml$");
        //return SelectedLanguage != activityLanguage.Value;
        //}

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
        /// <param name="lang">Laguage Name, pass <see langword="null"/> to get the default language.</param>
        /// <returns>a <see cref="ResourceDictionary"/> which path is
        /// <c>$"/Resource/Language/{<paramref name="lang"/>}.xaml"</c>.
        /// if it does not exist, return default language one.</returns>
        private static ResourceDictionary? LoadLanguageResourceDictionary(String? lang = null) {
            try {
                // if is null or blank string, set lang as default language (English (US)).
                lang = String.IsNullOrWhiteSpace(lang) ? "en-US" : lang;
                var langUri = new Uri($@"\Resource\Language\{lang}.xaml", UriKind.Relative);
                return Application.LoadComponent(langUri) as ResourceDictionary;
            }
            // The file does not exist.
            catch (IOException) {
                return null;
            }
        }

        /// <summary>
        /// Updata the Application Language to <see cref="MainWindowViewModel.SelectedLanguage"/>.
        /// </summary>
        private void UpdateApplicationLanguage() {
            ResourceDictionary? langResource = LoadLanguageResourceDictionary(this.SelectedLanguage);
            if (langResource is null) {
                // Default lang
                langResource = LoadLanguageResourceDictionary();
            }
            // If you have used other languages, clear it first.
            Application.Current.Resources.MergedDictionaries.Clear();
            // Add new language.
            Application.Current.Resources.MergedDictionaries.Add(langResource);
        }
    }
}
