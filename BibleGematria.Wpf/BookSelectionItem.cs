using System.ComponentModel;
using System.Runtime.CompilerServices;
using BibleGematria.Core.Models;

namespace BibleGematria.Wpf
{
    public class BookSelectionItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        public string Key { get; }
        public string DisplayName { get; }
        public BibleSection Section { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        public BookSelectionItem(string key, string displayName, BibleSection section, bool isSelected = true)
        {
            Key = key;
            DisplayName = displayName;
            Section = section;
            _isSelected = isSelected;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
