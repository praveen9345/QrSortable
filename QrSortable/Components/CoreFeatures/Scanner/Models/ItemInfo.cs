namespace QrSortable.Components.CoreFeatures.Scanner.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class ItemInfo : INotifyPropertyChanged
    {
        private string _itemName;
        private ImageSource _imageSource;
        private Color _fileLayoutBackgroundColor;

        /// <summary>
        /// Gets or sets the name of the item. Raises a PropertyChanged event when the value changes.
        /// </summary>
        public string ItemName
        {
            get => _itemName;
            set
            {
                if (_itemName != value)
                {
                    _itemName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the source of the image associated with the item.
        /// </summary>
        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the item layout. 
        /// </summary>
        public Color FileLayoutBackgroundColor
        {
            get => _fileLayoutBackgroundColor;
            set
            {
                if (_fileLayoutBackgroundColor != value)
                {
                    _fileLayoutBackgroundColor = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. Defaults to the caller's member name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
