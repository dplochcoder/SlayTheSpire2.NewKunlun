using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace CardArtClipper;

public sealed class CardEntry(string className, string title, string smallPath, string largePath)
    : INotifyPropertyChanged
{
    private bool _isComplete;

    public string ClassName { get; } = className;
    public string Title { get; } = title;
    public string SmallPath { get; } = smallPath;
    public string LargePath { get; } = largePath;

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
