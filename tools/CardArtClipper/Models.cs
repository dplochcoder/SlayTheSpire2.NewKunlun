using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace CardArtClipper;

public sealed class BulkObservableCollection<T> : ObservableCollection<T>
{
    public void ReplaceAll(IEnumerable<T> items)
    {
        Items.Clear();
        foreach (var item in items)
            Items.Add(item);
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
        );
    }
}

public sealed class CardEntry(
    string className,
    string title,
    AssetKind kind,
    string smallPath,
    string largePath,
    int smallWidth,
    int smallHeight,
    int largeWidth,
    int largeHeight
) : INotifyPropertyChanged
{
    private bool _isComplete;

    public string ClassName { get; } = className;
    public string Title { get; } = title;
    public AssetKind Kind { get; } = kind;
    public string SmallPath { get; } = smallPath;
    public string LargePath { get; } = largePath;
    public int SmallWidth { get; } = smallWidth;
    public int SmallHeight { get; } = smallHeight;
    public int LargeWidth { get; } = largeWidth;
    public int LargeHeight { get; } = largeHeight;

    public bool IsComplete
    {
        get => _isComplete;
        private set
        {
            if (_isComplete == value)
                return;
            _isComplete = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusGlyph));
        }
    }

    public string StatusGlyph => IsComplete ? "✓" : "✕";

    public void RefreshStatus() => IsComplete = File.Exists(SmallPath) && File.Exists(LargePath);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class GalleryEntry(string path, bool isVideo) : INotifyPropertyChanged
{
    private BitmapSource? _thumbnail;

    public string Path { get; } = path;
    public string Name => System.IO.Path.GetFileName(Path);
    public bool IsVideo { get; } = isVideo;
    public string VideoBadge => IsVideo ? "▶" : "";

    public BitmapSource? Thumbnail
    {
        get => _thumbnail;
        set
        {
            _thumbnail = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thumbnail)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public enum CompletionFilter
{
    Both,
    Completed,
    Incomplete,
}

public enum AssetKind
{
    Card,
    Power,
    Relic,
}
