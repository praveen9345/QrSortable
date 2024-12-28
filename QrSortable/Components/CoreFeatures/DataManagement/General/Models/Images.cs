namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Images : INotifyPropertyChanged
    {
        private ImageSource _image;
        
        private int _rotate;

        /// <summary>
        /// Gets or sets the source of the image.
        /// </summary>
        public ImageSource Image
        {
            get => _image;
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the image rotation.
        /// </summary>
        public int Rotate
        {
            get => _rotate;
            set
            {
                if (_rotate != value)
                {
                    _rotate = value;
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
