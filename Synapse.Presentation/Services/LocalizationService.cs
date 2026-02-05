using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Synapse.Presentation.Services
{
    public class LocalizationService : ILocalizationService
    {
        private static readonly ResourceManager _resourceManager = 
            new ResourceManager("Synapse.Presentation.Resources.Languages.AppResources", typeof(LocalizationService).Assembly);
        
        private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

        public event EventHandler<CultureInfo>? LanguageChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public string this[string key]
        {
            get
            {
                try
                {
                    var value = _resourceManager.GetString(key, _currentCulture);
                    return value ?? key;
                }
                catch
                {
                    return key;
                }
            }
        }

        public void SetLanguage(string cultureCode)
        {
            try
            {
                _currentCulture = new CultureInfo(cultureCode);
                CultureInfo.CurrentUICulture = _currentCulture;
                CultureInfo.CurrentCulture = _currentCulture;

                OnPropertyChanged("Item[]");
                LanguageChanged?.Invoke(this, _currentCulture);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting language: {ex.Message}");
            }
        }

        /// <summary>
        /// Get culture code from display name
        /// </summary>
        public static string GetCultureCode(string displayName)
        {
            return displayName switch
            {
                "English" => "en-US",
                "日本語" => "ja-JP",
                _ => "en-US"
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
