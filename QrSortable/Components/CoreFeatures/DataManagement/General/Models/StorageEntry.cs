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
            Color color;
            do
            {
                color = Color.FromRgb(
                    _random.Next(0, 256),
                    _random.Next(0, 256),
                    _random.Next(0, 256)
                );
            }
            while (IsTooDark(color)); // Repeat until it's not black or near-black

            return color;
        }

        private bool IsTooDark(Color color)
        {
            // MAUI Colors use double precision 0–1 range
            double brightness = (0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue);
            return brightness < 0.15; // exclude dark colors (tweak threshold)
        }

    }
}
