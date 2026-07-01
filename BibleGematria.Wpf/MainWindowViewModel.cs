using BibleGematria.Core;
using BibleGematria.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace BibleGematria.Wpf
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _hebrewInput = string.Empty;
        private ObservableCollection<MatchResult> _results = new();
        private bool _isSearching;
        public bool CanSearch => !IsSearching;
        private bool _allBooksSelected = true;
        private int _gematriaValue;
        private readonly TanachRepository _repository =
            new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));

        public IEnumerable<BookSelectionItem> TorahBooks =>
            Books.Where(b => b.Section == BibleSection.Torah);

        public IEnumerable<BookSelectionItem> ProphetsBooks =>
            Books.Where(b => b.Section == BibleSection.Prophets);

        public IEnumerable<BookSelectionItem> WritingsBooks =>
            Books.Where(b => b.Section == BibleSection.Writings);
        private ObservableCollection<BookSelectionItem> _books = new();

        public ObservableCollection<BookSelectionItem> Books
        {
            get => _books;
            set
            {
                if (_books != value)
                {
                    _books = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _noCrossEtnachta;
        public bool NoCrossEtnachta
        {
            get => _noCrossEtnachta;
            set
            {
                if (_noCrossEtnachta != value)
                {
                    _noCrossEtnachta = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _hasSearched;
        public bool HasSearched
        {
            get => _hasSearched;
            set
            {
                if (_hasSearched != value)
                {
                    _hasSearched = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ShowNoResultsMessage));
                }
            }
        }

        public int ResultCount => Results.Count;

        public bool HasResults => Results.Count > 0;

        public bool ShowNoResultsMessage => HasSearched && !HasResults;
        public MainWindowViewModel()
        {
            SearchCommand = new AsyncRelayCommand(ExecuteSearchAsyncImpl, () => CanSearch);

            Books = new ObservableCollection<BookSelectionItem>(
                BibleBookCatalog.Books.Select(b =>
                    new BookSelectionItem(b.Key, b.HebrewName, b.Section, isSelected: true)));

            foreach (var book in Books)
            {
                book.PropertyChanged += Book_PropertyChanged;
            }

            GematriaValue = GematriaCalculator.Compute(HebrewInput);
            Results.CollectionChanged += Results_CollectionChanged;
        }
        private void Results_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ResultCount));
            OnPropertyChanged(nameof(HasResults));
            OnPropertyChanged(nameof(ShowNoResultsMessage));
        }
        private void Book_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BookSelectionItem.IsSelected))
            {
                bool allSelected = Books.All(b => b.IsSelected);
                if (_allBooksSelected != allSelected)
                {
                    _allBooksSelected = allSelected;
                    OnPropertyChanged(nameof(AllBooksSelected));
                }
            }
        }

        public string HebrewInput
        {
            get => _hebrewInput;
            set
            {
                if (_hebrewInput != value)
                {
                    _hebrewInput = value;
                    OnPropertyChanged();

                    GematriaValue = GematriaCalculator.Compute(_hebrewInput);
                }
            }
        }

        public ObservableCollection<MatchResult> Results
        {
            get => _results;
            set
            {
                if (_results != value)
                {
                    _results = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSearching
        {
            get => _isSearching;
            set
            {
                if (_isSearching != value)
                {
                    _isSearching = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanSearch));
                    ((AsyncRelayCommand)SearchCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool AllBooksSelected
        {
            get => _allBooksSelected;
            set
            {
                if (_allBooksSelected != value)
                {
                    _allBooksSelected = value;
                    OnPropertyChanged();

                    foreach (var book in Books)
                    {
                        book.IsSelected = value;
                    }
                }
            }
        }

        public int GematriaValue
        {
            get => _gematriaValue;
            set
            {
                if (_gematriaValue != value)
                {
                    _gematriaValue = value;
                    OnPropertyChanged();
                }

            }
        }
        public ICommand SearchCommand { get; }

        private async Task ExecuteSearchAsyncImpl()
        {
            IsSearching = true;
            HasSearched = true;
            Results.Clear();
            try
            {
                var selectedBookKeys = Books
                    .Where(b => b.IsSelected)
                    .Select(b => b.Key)
                    .ToHashSet();

                if (selectedBookKeys.Count == 0)
                {
                    return;
                }

                var allVerses = _repository.GetBooks(selectedBookKeys);

                int target = GematriaCalculator.Compute(HebrewInput);
                var service = new SearchService(allVerses)
                {
                    MaxPhraseLength = 15
                };

                var singleWordMatches = service.FindSingleWordMatches(target);
                var phraseMatches = NoCrossEtnachta
                    ? service.FindPhraseMatchesNoBoundary(target)
                    : service.FindPhraseMatches(target);

                foreach (var result in singleWordMatches.Concat(phraseMatches))
                {
                    Results.Add(result);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search Failed: {ex.Message}");
            }
            finally
            {
                IsSearching = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}