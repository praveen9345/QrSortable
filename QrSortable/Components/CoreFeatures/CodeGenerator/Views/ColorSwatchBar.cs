namespace QrSortable.Components.CoreFeatures.CodeGenerator.Views
{
    using Microsoft.Maui.Controls.Shapes;
    using System.Windows.Input;

    /// <summary>
    /// One horizontally scrolling row of preset color swatches (same style as SuggestionChipBar:
    /// no CollectionView / templates, controls are built once).
    /// Tapping a swatch executes Command with the hex string; the swatch matching SelectedHex is outlined.
    /// </summary>
    public class ColorSwatchBar : ContentView
    {
        public static readonly string[] Presets =
        {
            "#D01C1D", // red
            "#CF09C9", // pink
            "#3254DB", // blue
            "#E28F13", // orange
            "#6A2BD9", // violet
            "#7B4A1E", // brown
            "#000000", // black
            "#00A08A", // teal
            "#F2D200", // yellow
            "#CA9B84", // light dusty tan
            "#A84D6E", // dusty rose
            "#276D5E", // deep muted teal green
        };

        private const double Size = 36;
        private readonly Border[] _swatches = new Border[Presets.Length];

        public static readonly BindableProperty SelectedHexProperty = BindableProperty.Create(
            nameof(SelectedHex), typeof(string), typeof(ColorSwatchBar), null,
            propertyChanged: (b, _, _) => ((ColorSwatchBar)b).UpdateSelection());

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command), typeof(ICommand), typeof(ColorSwatchBar));

        /// <summary>Currently selected hex (e.g. "#D01C1D"); compared case-insensitively.</summary>
        public string? SelectedHex
        {
            get => (string?)GetValue(SelectedHexProperty);
            set => SetValue(SelectedHexProperty, value);
        }

        /// <summary>Executed with the swatch hex as parameter.</summary>
        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public ColorSwatchBar()
        {
            var stack = new HorizontalStackLayout { Spacing = 10, Padding = new Thickness(12, 6) };

            for (int i = 0; i < Presets.Length; i++)
            {
                string hex = Presets[i];
                var swatch = new Border
                {
                    WidthRequest = Size,
                    HeightRequest = Size,
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(Size / 2) },
                    Background = new SolidColorBrush(Color.FromArgb(hex)),
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = 1
                };
                SemanticProperties.SetDescription(swatch, hex);

                var tap = new TapGestureRecognizer();
                tap.Tapped += (_, _) =>
                {
                    var cmd = Command;
                    if (cmd != null && cmd.CanExecute(hex)) cmd.Execute(hex);
                };
                swatch.GestureRecognizers.Add(tap);

                _swatches[i] = swatch;
                stack.Add(swatch);
            }

            Content = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                Content = stack
            };

            UpdateSelection();
        }

        private void UpdateSelection()
        {
            string? selected = SelectedHex?.Trim();
            for (int i = 0; i < Presets.Length; i++)
            {
                bool on = string.Equals(selected, Presets[i], StringComparison.OrdinalIgnoreCase);
                _swatches[i].Stroke = new SolidColorBrush(on ? Colors.White : Colors.Gray);
                _swatches[i].StrokeThickness = on ? 3 : 1;
            }
        }
    }
}
