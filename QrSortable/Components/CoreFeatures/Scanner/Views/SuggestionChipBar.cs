namespace QrSortable.Components.CoreFeatures.Scanner.Views
{
    using System.Windows.Input;

    /// <summary>
    /// One horizontally scrolling row of up to 6 reusable chips.
    /// No CollectionView / templates: just 6 pooled buttons whose text + visibility are updated.
    /// Hidden automatically when there is nothing to suggest.
    /// </summary>
    public class SuggestionChipBar : ContentView
    {
        private const int MaxChips = 6;
        private readonly Button[] _chips = new Button[MaxChips];

        public static readonly BindableProperty SuggestionsProperty = BindableProperty.Create(
            nameof(Suggestions), typeof(IReadOnlyList<string>), typeof(SuggestionChipBar), null,
            propertyChanged: OnSuggestionsChanged);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command), typeof(ICommand), typeof(SuggestionChipBar));

        public IReadOnlyList<string>? Suggestions
        {
            get => (IReadOnlyList<string>?)GetValue(SuggestionsProperty);
            set => SetValue(SuggestionsProperty, value);
        }

        /// <summary>Executed with the chip text as parameter.</summary>
        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public SuggestionChipBar()
        {
            var stack = new HorizontalStackLayout { Spacing = 8, Padding = new Thickness(12, 4) };

            var fill = Res("DarkGrey", Color.FromArgb("#333333"));
            var border = Res("Yellow", Color.FromArgb("#FDB927"));
            var text = Res("White", Colors.White);

            for (int i = 0; i < MaxChips; i++)
            {
                var chip = new Button
                {
                    IsVisible = false,
                    HeightRequest = 36,
                    MinimumHeightRequest = 36,
                    Padding = new Thickness(14, 0),
                    CornerRadius = 18,
                    FontSize = 14,
                    FontFamily = "ItimRegular",
                    BackgroundColor = fill,
                    BorderColor = border,
                    BorderWidth = 1,
                    TextColor = text,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    MaximumWidthRequest = 220
                };
                chip.Clicked += OnChipClicked;
                _chips[i] = chip;
                stack.Add(chip);
            }

            Content = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                Content = stack
            };

            IsVisible = false;
        }

        private static void OnSuggestionsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var bar = (SuggestionChipBar)bindable;
            var list = newValue as IReadOnlyList<string>;
            int count = list?.Count ?? 0;

            for (int i = 0; i < MaxChips; i++)
            {
                bool show = i < count;
                bar._chips[i].IsVisible = show;
                if (show) bar._chips[i].Text = list![i];
            }

            bar.IsVisible = count > 0;
        }

        private void OnChipClicked(object? sender, EventArgs e)
        {
            if (sender is not Button b) return;
            var cmd = Command;
            if (cmd != null && cmd.CanExecute(b.Text)) cmd.Execute(b.Text);
        }

        private static Color Res(string key, Color fallback)
        {
            var res = Application.Current?.Resources;
            return res != null && res.TryGetValue(key, out var v) && v is Color c ? c : fallback;
        }
    }
}
