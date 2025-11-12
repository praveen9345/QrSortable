namespace QrSortable.Components.CoreFeatures.DataManagement.General.Models
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System.Collections.ObjectModel;

    public class StorageGroup : ObservableObject
    {
        public string Category { get; set; }
        public ObservableCollection<StorageEntry> Items { get; set; } = new();

        // For incremental loading
        private int _loadedItemCount;
        public int LoadedItemCount
        {
            get => _loadedItemCount;
            set => SetProperty(ref _loadedItemCount, value);
        }

        public ObservableCollection<StorageEntry> VisibleItems { get; set; } = new();

        public AsyncRelayCommand LoadMoreCommand => new AsyncRelayCommand(async () =>
        {
            const int pageSize = 20;
            var nextItems = Items.Skip(LoadedItemCount).Take(pageSize).ToList();
            foreach (var item in nextItems)
                VisibleItems.Add(item);

            LoadedItemCount += nextItems.Count;
            await Task.CompletedTask;
        });
    }
}
