using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace WindowsStoreSamples.AutoCompleteBox {
    public class MainPageViewModel  : INotifyPropertyChanged {
        
        // Setup the subjet for the enter key on the textbox
        private readonly Subject<Unit> enterKeyPressed = new Subject<Unit>();

        public MainPageViewModel() {
            // Listen to all property change events on SearchText
            var searchTextChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                ev => PropertyChanged += ev,
                ev => PropertyChanged -= ev
                )
                .Where(ev => ev.EventArgs.PropertyName == "SearchText");

            // Transform the event stream into a stream of strings (the input values)
            var input = 
                searchTextChanged
                .Where(ev => string.IsNullOrWhiteSpace(SearchText))
                .Merge(searchTextChanged
                    .Where(ev => SearchText == null || SearchText.Length < 4)
                    .Throttle(TimeSpan.FromSeconds(3)))
                .Merge(searchTextChanged
                    .Where(ev => SearchText != null && SearchText.Length >= 4)
                    .Throttle(TimeSpan.FromMilliseconds(400)))
                .Select(args => SearchText)
                .Merge(
                    enterKeyPressed.Select(e => SearchText))
                .DistinctUntilChanged();

            // Log all events in the event stream to the Log viewer
            input.ObserveOnDispatcher()
                .Subscribe(e => LogOutput.Insert(0, 
                    string.Format("Text Changed. Current Value - {0}", e)));

            // Setup an Observer for the search operation
            var search = Observable.ToAsync<string, SearchResult>(DoSearch);

            
            searchTextChanged.ObserveOnDispatcher().Subscribe(s => IsInResultsMode = !string.IsNullOrEmpty(SearchText));

            // Chain the input event stream and the search stream, cancelling searches when input is received
            var results = from searchTerm in input
                          from result in search(searchTerm).TakeUntil(searchTextChanged)
                          select result;

            // Log the search result and add the results to the results collection
            results.ObserveOnDispatcher()
            .Subscribe(result => {
                searchResults.Clear();
                LogOutput.Insert(0, string.Format("Search for '{0}' returned '{1}' items", result.SearchTerm, result.Results.Count()));
                foreach(var item in result.Results) {
                    searchResults.Add(item);
                }
                // Make the popup hide when the textblock is blanked out
                IsInResultsMode = !string.IsNullOrEmpty(SearchText);
                NotifyPropertyChanged("ResultsAreEmpty");
            });
        }


        private readonly Random random = new Random(5);
        private SearchResult DoSearch(string searchTerm) {
            if(string.IsNullOrEmpty(searchTerm)) {
                return new SearchResult() {
                    Results = new string[0],
                    SearchTerm = searchTerm
                };
            }
            new System.Threading.ManualResetEvent(false).WaitOne(random.Next(100, 500)); // Simulate latency
            return new SearchResult {
                SearchTerm = searchTerm,
                Results =
                    phrases.Where(item => item.ToUpperInvariant().Contains(searchTerm.ToUpperInvariant())).ToArray()
            };
        }

        private readonly string[] phrases = new[] {
            "The badger knows something",
            "Your head looks like a pineapple",
            "Crazy like a box of frogs",
            "Can you smell toast?",
            "We're going to need some golf shoes",
            "I think I'm getting the Fear",
            "There's someone at the door",
            "We can't stop here. This is bat country.",
            "It's okay. He's just admiring the shape of your skull.",
            "Let's get down to brass tacks. How much for the ape?",
            "We want... a shrubbery",
            "What is the airspeed velocity of an unladen swallow?",
            "I unplug my nose in your general direction, sons-of-a-windowdresser! ",
            "Nobody expects the Spanish Inquisition",
            "Well, that's no ordinary rabbit",
            "Dale dug a hole. Tell 'em Dale - Dale: I dug a hole",
            "Hows the serenity?",
            "Dad, he reckons powerlines are a reminder of man's ability to generate electricity.",
            "Tell 'em they're dreamin'.",
            "This is going straight to the pool room",
            "We're going to Bonnie Doon!",
            "Steve is also an ideas man. That's why Dad calls him the Ideas Man. He has lots of ideas.",
            "It's a motorcycle helmet with a built-in brake light",
            "Hansel, so hot right now...Hansel.",
            "ORANGE MOCHA FRAPPUCCINO!!!",
            "I'm not an ambi-turner",
            "If you can dodge a wrench, you can dodge a ball.",
            "If you can dodge traffic, you can dodge a ball."
        };

        private string searchText;

        public string SearchText {
            get { return searchText; }
            set {
                if (searchText == value) {
                    return;
                }
                searchText = value;
                NotifyPropertyChanged("SearchText");
            }
        }

        private readonly ObservableCollection<string> logOutput = new ObservableCollection<string>();

        public ObservableCollection<string> LogOutput {
            get { return logOutput; }
        }

        private readonly ObservableCollection<string> searchResults = new ObservableCollection<string>();
        

        public ObservableCollection<string> SearchResults {
            get { return searchResults; }
        }

        private bool isInResultsMode;
        public bool IsInResultsMode {
            get { return isInResultsMode; }
            set {
                isInResultsMode = value;
                NotifyPropertyChanged("IsInResultsMode");
            }
        }

        public bool ResultsAreEmpty {
            get { return SearchResults.Count == 0 && IsInResultsMode; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void OnEnterKeyPressed() {
            enterKeyPressed.OnNext(Unit.Default);
        }
    }

    public struct SearchResult {
        public string SearchTerm { get; set; }
        public IEnumerable<string> Results { get; set; }
    }


    
}