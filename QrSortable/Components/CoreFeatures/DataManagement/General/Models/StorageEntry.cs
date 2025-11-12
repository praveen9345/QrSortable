namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using QrSortable.Components.CoreFeatures.DataManagement.Models;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StorageEntry : DatabaseEntry
    {
        public Guid StorageId { get; private set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public string BarcodeValue { get; set; }
        public string BarcodeType { get; set; }
        public string Location { get; set; }
        public string SearchInfo { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public IList<byte[]> ImageList { get; set; }

        public string BackgroundColorHex { get; set; }

        [NotMapped] // This tells EF Core to ignore it for database mapping
        public Color BackgroundColor
        {
            get => Color.FromArgb(BackgroundColorHex ?? "#FFFFFFFF");
            set => BackgroundColorHex = value.ToHex();
        }

        private static readonly Random _random = new Random();

        public StorageEntry() 
        {
            StorageId = Guid.NewGuid();

            BackgroundColor = RandomColor();
        }

        private Color RandomColor()
        {
            // Random RGB
            return Color.FromRgb(
                _random.Next(50, 256), // Avoid very dark colors
                _random.Next(50, 256),
                _random.Next(50, 256)
            );
        }
    }
}
