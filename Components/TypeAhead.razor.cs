using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace CoViDAccountant.Components
{
    public partial class TypeAhead<TItem, TKey> : ComponentBase, IDisposable
        where TItem : SharedClasses.Abstractions.Entity<TKey>
    {
        [Parameter] public int MinimumLength { get; set; } = 3;
        [Parameter] public int MaximumSuggestions { get; set; } = 10;
        [Parameter] public int DebounceInterval { get; set; } = 300;

        [Parameter] public TItem Value { get; set; }
        [Parameter] public EventCallback<TItem> ValueChanged { get; set; }

        [Parameter] public RenderFragment<TItem> SelectedTemplate { get; set; }
        [Parameter] public RenderFragment<TItem> ResultTemplate { get; set; }
        [Parameter] public Func<string, int, Task<IEnumerable<TItem>>> SearchMethod { get; set; }
        [Parameter] public Func<TItem, string> LabelFunc { get; set; }
        [Parameter] public string CssClass { get; set; }


        [Inject] private Microsoft.JSInterop.IJSRuntime JsRuntime { get; set; }


        //protected override Task OnInitializedAsync()
        //{
        //    _debounceTimer = new Timer
        //    {
        //        Interval = DebounceInterval,
        //        AutoReset = false
        //    };
        //    _debounceTimer.Elapsed += Search;

        //    Initialize();

        //    return base.OnInitializedAsync();
        //}

        //protected override Task OnParametersSetAsync()
        //{
        //    Initialize();
        //    return base.OnParametersSetAsync();
        //}
        protected override void OnInitialized()
        {
            _debounceTimer = new Timer
            {
                Interval = DebounceInterval,
                AutoReset = false
            };
            _debounceTimer.Elapsed += Search;

            Initialize();

            base.OnInitialized();
        }
        private void Initialize()
        {
            SearchText = "";
            _isShowingSuggestions = false;
            _maskOn = Value != null;
        }

        private async Task ResetControl()
        {
            Initialize();
        }

        private async Task OnClickMask()
        {
            if (LabelFunc != null)
                SearchText = Value.HasDefaultId() == true ? "" : LabelFunc(Value);
            else
                SearchText = "";
            _maskOn = false;
            await Task.Delay(250); // Possible race condition here.
            await JsRuntime.InvokeAsync<object>("jsHelpers.setFocus", new object[] { _searchInput });
        }

        private async void Search(object sender, ElapsedEventArgs args)
        {
            if (_searchText.Length < MinimumLength)
            {
                //await InvokeAsync(StateHasChanged);
                return;
            }

            _suggestions = (await SearchMethod.Invoke(_searchText, MaximumSuggestions)).ToArray();

            _isShowingSuggestions = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task SelectResult(TItem item)
        {
            Value = item;
            await ValueChanged.InvokeAsync(item);
            Initialize();
        }

        private bool ShouldShowSuggestions()
        {
            return _isShowingSuggestions && _suggestions.Any();
        }

        private string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;

                if (value.Length == 0)
                {
                    _debounceTimer.Stop();
                }
                else
                {
                    _debounceTimer.Stop();
                    _debounceTimer.Start();
                }
                //if (value.Length > 0)
                //{
                //    _debounceTimer.Stop();
                //    _debounceTimer.Start();
                //}
            }
        }
        private string _searchText = string.Empty;
        private TItem[] _suggestions = new TItem[0];

        private bool _isShowingSuggestions;

        private Timer _debounceTimer;

        private bool _maskOn = true;

        private ElementReference _searchInput;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _debounceTimer.Dispose();
            }
        }
    }
}
