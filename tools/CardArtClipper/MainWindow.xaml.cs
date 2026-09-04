using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace CardArtClipper;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private static readonly HashSet<string> ImageExtensions = new(
        [".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff", ".webp"],
        StringComparer.OrdinalIgnoreCase
    );
    private static readonly HashSet<string> VideoExtensions = new(
        [".mp4", ".m4v", ".mov", ".avi", ".wmv", ".mkv", ".webm"],
        StringComparer.OrdinalIgnoreCase
    );

    private readonly AppSettings _settings;
    private readonly string _repositoryRoot;
    private readonly ObservableCollection<CardEntry> _cards = [];
    private ObservableCollection<GalleryEntry> _gallery = [];
    private CancellationTokenSource? _galleryLoadCancellation;
    private CancellationTokenSource? _cropLoadCancellation;
    private double _thumbnailSize;
    private bool _suppressCardSelection;
    private readonly Dictionary<AssetKind, string?> _selectedClassNames = [];

    public MainWindow()
    {
        _settings = AppSettings.Load();
        _thumbnailSize = _settings.ThumbnailSize;
        _repositoryRoot = CardDiscovery.FindRepositoryRoot();

        InitializeComponent();
        DataContext = this;
        FontSize = _settings.FontSize;
        CardList.ItemsSource = _cards;
        GalleryList.ItemsSource = _gallery;
        CollectionViewSource.GetDefaultView(_cards).Filter = FilterCard;
        Cropper.SaveRequested += Cropper_OnSaveRequested;

        foreach (var card in CardDiscovery.ReadEntries(_repositoryRoot, AssetKind.Card))
            _cards.Add(card);
        UpdatePreviewLayout(_cards.FirstOrDefault());
        CardList.SelectedIndex = _cards.Count > 0 ? 0 : -1;

        if (Directory.Exists(_settings.GalleryPath))
        {
            GalleryPathText.Text = _settings.GalleryPath;
            Loaded += async (_, _) => await RefreshGalleryAsync();
        }
    }

    public double ThumbnailSize
    {
        get => _thumbnailSize;
        private set
        {
            _thumbnailSize = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool FilterCard(object value)
    {
        if (value is not CardEntry card)
            return false;
        var filter = ((CompletionFilterBox.SelectedItem as ComboBoxItem)?.Tag as string) switch
        {
            "Completed" => CompletionFilter.Completed,
            "Incomplete" => CompletionFilter.Incomplete,
            _ => CompletionFilter.Both,
        };
        if (filter == CompletionFilter.Completed && !card.IsComplete)
            return false;
        if (filter == CompletionFilter.Incomplete && card.IsComplete)
            return false;

        var tokens = SearchBox.Text.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );
        return tokens.All(token =>
            card.Title.Contains(token, StringComparison.CurrentCultureIgnoreCase)
        );
    }

    private async void CardList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_suppressCardSelection)
            await LoadSelectedCardPreviewsAsync();
    }

    private async Task LoadSelectedCardPreviewsAsync()
    {
        if (CardList.SelectedItem is not CardEntry card)
            return;
        SetBusy(true, $"Loading {card.Title}…");
        try
        {
            UpdatePreviewLayout(card);
            var smallPath = File.Exists(card.SmallPath)
                ? card.SmallPath
                : GetPlaceholder(card, false);
            var largePath = File.Exists(card.LargePath)
                ? card.LargePath
                : GetPlaceholder(card, true);
            var smallTask = Task.Run(() => LoadBitmap(smallPath));
            var largeTask = Task.Run(() => LoadBitmap(largePath));
            await Task.WhenAll(smallTask, largeTask);
            SmallPreview.Source = smallTask.Result;
            LargePreview.Source = largeTask.Result;
            card.RefreshStatus();
            CollectionViewSource.GetDefaultView(_cards).Refresh();
        }
        catch (Exception exception)
        {
            ShowError("Could not load art", exception);
        }
        finally
        {
            SetBusy(false, "");
        }
    }

    private async void RefreshCards_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedClassName = (CardList.SelectedItem as CardEntry)?.ClassName;
        IReadOnlyList<CardEntry> refreshedCards;
        try
        {
            refreshedCards = await Task.Run(() =>
                CardDiscovery.ReadEntries(_repositoryRoot, ActiveAssetKind)
            );
        }
        catch (Exception exception)
        {
            ShowError("Could not refresh assets", exception);
            return;
        }

        _suppressCardSelection = true;
        try
        {
            using (CollectionViewSource.GetDefaultView(_cards).DeferRefresh())
            {
                _cards.Clear();
                foreach (var card in refreshedCards)
                    _cards.Add(card);
            }
            CardList.SelectedItem =
                _cards.FirstOrDefault(card => card.ClassName == selectedClassName)
                ?? _cards.FirstOrDefault();
        }
        finally
        {
            _suppressCardSelection = false;
        }

        if (
            selectedClassName is null
            || (CardList.SelectedItem as CardEntry)?.ClassName != selectedClassName
        )
            await LoadSelectedCardPreviewsAsync();
    }

    private async void AssetKindTabs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CardList is null || e.Source != AssetKindTabs)
            return;

        var previous = _cards.FirstOrDefault()?.Kind;
        if (previous is not null)
            _selectedClassNames[previous.Value] = (CardList.SelectedItem as CardEntry)?.ClassName;

        IReadOnlyList<CardEntry> entries;
        try
        {
            entries = await Task.Run(() =>
                CardDiscovery.ReadEntries(_repositoryRoot, ActiveAssetKind)
            );
        }
        catch (Exception exception)
        {
            ShowError("Could not load assets", exception);
            return;
        }

        _suppressCardSelection = true;
        try
        {
            using (CollectionViewSource.GetDefaultView(_cards).DeferRefresh())
            {
                _cards.Clear();
                foreach (var entry in entries)
                    _cards.Add(entry);
            }

            _selectedClassNames.TryGetValue(ActiveAssetKind, out var selectedClassName);
            CardList.SelectedItem =
                _cards.FirstOrDefault(entry => entry.ClassName == selectedClassName)
                ?? _cards.FirstOrDefault();
        }
        finally
        {
            _suppressCardSelection = false;
        }

        UpdatePreviewLayout(_cards.FirstOrDefault());
        await LoadSelectedCardPreviewsAsync();
    }

    private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e) =>
        CollectionViewSource.GetDefaultView(_cards).Refresh();

    private void CompletionFilterBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CardList is not null && _cards is not null)
            CollectionViewSource.GetDefaultView(_cards).Refresh();
    }

    private async void SelectGalleryFolder_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select image and video gallery folder",
            InitialDirectory = Directory.Exists(_settings.GalleryPath)
                ? _settings.GalleryPath
                : Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        };
        if (dialog.ShowDialog(this) != true)
            return;
        _settings.GalleryPath = dialog.FolderName;
        _settings.Save();
        GalleryPathText.Text = dialog.FolderName;
        await RefreshGalleryAsync();
    }

    private async void RefreshGallery_OnClick(object sender, RoutedEventArgs e) =>
        await RefreshGalleryAsync();

    private async Task RefreshGalleryAsync()
    {
        if (!Directory.Exists(_settings.GalleryPath))
            return;
        _galleryLoadCancellation?.Cancel();
        _galleryLoadCancellation?.Dispose();
        _galleryLoadCancellation = new CancellationTokenSource();
        var cancellationToken = _galleryLoadCancellation.Token;
        string[] paths;
        try
        {
            paths = await Task.Run(
                () =>
                    Directory.EnumerateFiles(_settings.GalleryPath).OrderBy(path => path).ToArray(),
                cancellationToken
            );
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            ShowError("Could not read gallery folder", exception);
            return;
        }

        var imageEntries = paths
            .Where(path => ImageExtensions.Contains(Path.GetExtension(path)))
            .Select(path => new GalleryEntry(path, false))
            .ToArray();
        var videoEntries = paths
            .Where(path => VideoExtensions.Contains(Path.GetExtension(path)))
            .Select(path => new GalleryEntry(path, true))
            .ToArray();
        var refreshedGallery = new ObservableCollection<GalleryEntry>(
            imageEntries.Concat(videoEntries)
        );
        _gallery = refreshedGallery;
        GalleryList.ItemsSource = refreshedGallery;

        using var imageSemaphore = new SemaphoreSlim(2);
        var imageTasks = imageEntries.Select(async entry =>
        {
            await imageSemaphore.WaitAsync(cancellationToken);
            try
            {
                var thumbnail = await Task.Run(
                    () => LoadThumbnailAtLowPriority(entry.Path, (int)ThumbnailSize),
                    cancellationToken
                );
                await Dispatcher.InvokeAsync(() => entry.Thumbnail = thumbnail);
            }
            catch when (cancellationToken.IsCancellationRequested) { }
            catch { }
            finally
            {
                imageSemaphore.Release();
            }
        });
        await Task.WhenAll(imageTasks);

        foreach (var entry in videoEntries)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            try
            {
                entry.Thumbnail = await VideoThumbnailer.CreateAsync(
                    entry.Path,
                    (int)ThumbnailSize,
                    Dispatcher,
                    cancellationToken
                );
            }
            catch when (cancellationToken.IsCancellationRequested) { }
            catch { }
        }
    }

    private async void GalleryList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GalleryList.SelectedItem is not GalleryEntry entry)
            return;
        _cropLoadCancellation?.Cancel();
        _cropLoadCancellation?.Dispose();
        _cropLoadCancellation = new CancellationTokenSource();
        CropTitle.Text = $"Crop — {entry.Name}";
        try
        {
            await Cropper.LoadAsync(entry.Path, entry.IsVideo, _cropLoadCancellation.Token);
        }
        catch (OperationCanceledException) { }
        catch (Exception exception)
        {
            ShowError("Could not load gallery item", exception);
        }
    }

    private async void Cropper_OnSaveRequested(object? sender, EventArgs e)
    {
        if (CardList.SelectedItem is not CardEntry card)
        {
            MessageBox.Show(this, "Select an asset first.", "Nothing selected");
            return;
        }
        if (!Cropper.HasSource)
        {
            MessageBox.Show(this, "Select an image or video first.", "No source selected");
            return;
        }

        SetBusy(true, $"Saving art for {card.Title}…");
        Cropper.SetInteractionEnabled(false);
        try
        {
            var large = Cropper.RenderCrop(card.LargeWidth, card.LargeHeight);
            var small = Cropper.RenderCrop(card.SmallWidth, card.SmallHeight);
            await Task.Run(() =>
            {
                SavePngAtomically(large, card.LargePath);
                SavePngAtomically(small, card.SmallPath);
            });
            await LoadSelectedCardPreviewsAsync();
        }
        catch (Exception exception)
        {
            ShowError("Could not save art", exception);
        }
        finally
        {
            Cropper.SetInteractionEnabled(true);
            SetBusy(false, "");
        }
    }

    private void Options_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OptionsWindow(_settings.FontSize, _settings.ThumbnailSize)
        {
            Owner = this,
        };
        if (dialog.ShowDialog() != true)
            return;
        _settings.FontSize = dialog.SelectedFontSize;
        _settings.ThumbnailSize = dialog.SelectedThumbnailSize;
        _settings.Save();
        FontSize = _settings.FontSize;
        ThumbnailSize = _settings.ThumbnailSize;
    }

    private void Window_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F11)
            e.Handled = true;
    }

    private void Exit_OnClick(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosed(EventArgs e)
    {
        _galleryLoadCancellation?.Cancel();
        _cropLoadCancellation?.Cancel();
        base.OnClosed(e);
    }

    private void SetBusy(bool busy, string message)
    {
        BusyText.Text = message;
        BusyOverlay.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
    }

    private static BitmapImage LoadBitmap(string path, int decodeWidth = 0)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        if (decodeWidth > 0)
            bitmap.DecodePixelWidth = decodeWidth;
        bitmap.StreamSource = stream;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    private static BitmapImage LoadThumbnailAtLowPriority(string path, int size)
    {
        var thread = Thread.CurrentThread;
        var originalPriority = thread.Priority;
        try
        {
            thread.Priority = ThreadPriority.BelowNormal;
            return LoadBitmap(path, size);
        }
        finally
        {
            thread.Priority = originalPriority;
        }
    }

    private static void SavePngAtomically(BitmapSource bitmap, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";
        try
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = File.Create(temporaryPath))
                encoder.Save(stream);
            File.Copy(temporaryPath, path, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }

    private void ShowError(string title, Exception exception) =>
        MessageBox.Show(this, exception.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private AssetKind ActiveAssetKind =>
        Enum.TryParse<AssetKind>(
            (AssetKindTabs.SelectedItem as TabItem)?.Tag as string,
            out var kind
        )
            ? kind
            : AssetKind.Card;

    private static string GetPlaceholder(CardEntry entry, bool large)
    {
        var name = entry.Kind switch
        {
            AssetKind.Card => "card.png",
            AssetKind.Power => "power.png",
            AssetKind.Relic => "relic.png",
            _ => throw new ArgumentOutOfRangeException(),
        };
        var directory = large
            ? Path.GetDirectoryName(entry.LargePath)!
            : Path.GetDirectoryName(entry.SmallPath)!;
        return Path.Combine(directory, name);
    }

    private void UpdatePreviewLayout(CardEntry? entry)
    {
        if (entry is null)
            return;
        var kindName = entry.Kind.ToString();
        PreviewHeading.Text = $"{kindName} previews (1:1)";
        SmallPreviewLabel.Text = $"Small — {entry.SmallWidth} × {entry.SmallHeight}";
        LargePreviewLabel.Text = $"Large — {entry.LargeWidth} × {entry.LargeHeight}";
        SmallPreview.Width = entry.SmallWidth;
        SmallPreview.Height = entry.SmallHeight;
        SmallPreviewBorder.Width = entry.SmallWidth + 2;
        SmallPreviewBorder.Height = entry.SmallHeight + 2;
        LargePreview.Width = entry.LargeWidth;
        LargePreview.Height = entry.LargeHeight;
        LargePreviewBorder.Width = entry.LargeWidth + 2;
        LargePreviewBorder.Height = entry.LargeHeight + 2;
        Cropper.SetAspectRatio((double)entry.LargeWidth / entry.LargeHeight);
    }
}
