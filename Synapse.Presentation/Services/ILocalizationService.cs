using System.ComponentModel;
using System.Globalization;

namespace Synapse.Presentation.Services
{
    public interface ILocalizationService : INotifyPropertyChanged
    {
        string this[string key] { get; }
        void SetLanguage(string cultureCode);
        event EventHandler<CultureInfo>? LanguageChanged;
    }
}
