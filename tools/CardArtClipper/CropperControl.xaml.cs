using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CardArtClipper;

public partial class CropperControl : UserControl
{
    private double _aspectRatio = 25d / 19d;
    private BitmapSource? _sourceBitmap;
    private bool _isVideo;
    private bool _isDragging;
    private bool _updatingSlider;
    private Point _lastPointer;
    private double _zoom = 1;
    private Vector _pan;
    private int _sourceWidth;
    private int _sourceHeight;
    private double _frameRate = 30;

    public CropperControl()
    {
        InitializeComponent();
        PreviewKeyDown += OnPreviewKeyDown;
    }

    public event EventHandler? SaveRequested;

    public bool HasSource => _sourceWidth > 0 && _sourceHeight > 0;

    public async Task LoadAsync(string path, bool isVideo, CancellationToken cancellationToken)
    {
        ResetMedia();
        _isVideo = isVideo;
        if (isVideo)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                SourceVideo.Source = new Uri(path);
                SourceVideo.Visibility = Visibility.Visible;
                VideoControls.Visibility = Visibility.Visible;
                SourceVideo.Play();
                SourceVideo.Pause();
            });
        }
        else
        {
            var bitmap = await Task.Run(() => LoadBitmap(path), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            _sourceBitmap = bitmap;
            _sourceWidth = bitmap.PixelWidth;
            _sourceHeight = bitmap.PixelHeight;
            SourceImage.Source = bitmap;
            SourceImage.Visibility = Visibility.Visible;
            InitializeMediaLayout();
        }
    }

    public BitmapSource RenderCrop(int width, int height)
    {
        if (!HasSource)
            throw new InvalidOperationException("No gallery content is selected.");

        BitmapSource source;
        if (_isVideo)
        {
            SourceVideo.Position = TimeSpan.FromSeconds(VideoSlider.Value);
            SourceVideo.Width = _sourceWidth;
            SourceVideo.Height = _sourceHeight;
            SourceVideo.Measure(new Size(_sourceWidth, _sourceHeight));
            SourceVideo.Arrange(new Rect(0, 0, _sourceWidth, _sourceHeight));
            SourceVideo.UpdateLayout();
            var videoFrame = new RenderTargetBitmap(
                _sourceWidth,
                _sourceHeight,
                96,
                96,
                PixelFormats.Pbgra32
            );
            videoFrame.Render(SourceVideo);
            videoFrame.Freeze();
            source = videoFrame;
        }
        else
        {
            source = _sourceBitmap!;
        }

        var frameWidth = CropFrame.ActualWidth;
        var frameHeight = CropFrame.ActualHeight;
        var frameLeft = (Viewport.ActualWidth - frameWidth) / 2;
        var frameTop = (Viewport.ActualHeight - frameHeight) / 2;
        var mediaLeft = Viewport.ActualWidth / 2 + _pan.X - _sourceWidth * _zoom / 2;
        var mediaTop = Viewport.ActualHeight / 2 + _pan.Y - _sourceHeight * _zoom / 2;
        var targetScaleX = width / frameWidth;
        var targetScaleY = height / frameHeight;
        var destination = new Rect(
            (mediaLeft - frameLeft) * targetScaleX,
            (mediaTop - frameTop) * targetScaleY,
            _sourceWidth * _zoom * targetScaleX,
            _sourceHeight * _zoom * targetScaleY
        );

        var drawing = new DrawingVisual();
        using (var context = drawing.RenderOpen())
        {
            context.DrawRectangle(Brushes.Black, null, new Rect(0, 0, width, height));
            context.DrawImage(source, destination);
        }

        var result = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        result.Render(drawing);
        result.Freeze();
        return result;
    }

    public void SetInteractionEnabled(bool enabled) => IsEnabled = enabled;

    public void SetAspectRatio(double aspectRatio)
    {
        if (aspectRatio <= 0 || double.IsNaN(aspectRatio) || double.IsInfinity(aspectRatio))
            throw new ArgumentOutOfRangeException(nameof(aspectRatio));
        _aspectRatio = aspectRatio;
        ResizeCropFrame(Viewport.ActualWidth, Viewport.ActualHeight);
        if (HasSource)
            InitializeMediaLayout();
    }

    private static BitmapImage LoadBitmap(string path)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = stream;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    private void ResetMedia()
    {
        SourceVideo.Stop();
        SourceVideo.Source = null;
        SourceVideo.Visibility = Visibility.Collapsed;
        SourceImage.Source = null;
        SourceImage.Visibility = Visibility.Collapsed;
        VideoControls.Visibility = Visibility.Collapsed;
        EmptyMessage.Visibility = Visibility.Visible;
        _sourceBitmap = null;
        _sourceWidth = 0;
        _sourceHeight = 0;
        _zoom = 1;
        _pan = default;
    }

    private void InitializeMediaLayout()
    {
        EmptyMessage.Visibility = Visibility.Collapsed;
        var frameWidth = Math.Max(1, CropFrame.ActualWidth);
        var frameHeight = Math.Max(1, CropFrame.ActualHeight);
        _zoom = Math.Max(frameWidth / _sourceWidth, frameHeight / _sourceHeight);
        _pan = default;
        UpdateMediaTransform();
        Focus();
    }

    private void UpdateMediaTransform()
    {
        if (!HasSource)
            return;
        var element = _isVideo ? (FrameworkElement)SourceVideo : SourceImage;
        element.Width = _sourceWidth;
        element.Height = _sourceHeight;
        Canvas.SetLeft(element, Viewport.ActualWidth / 2 - _sourceWidth / 2);
        Canvas.SetTop(element, Viewport.ActualHeight / 2 - _sourceHeight / 2);
        element.RenderTransformOrigin = new Point(0.5, 0.5);
        element.RenderTransform = new TransformGroup
        {
            Children = { new ScaleTransform(_zoom, _zoom), new TranslateTransform(_pan.X, _pan.Y) },
        };
    }

    private void Viewport_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ResizeCropFrame(e.NewSize.Width, e.NewSize.Height);
        UpdateMediaTransform();
    }

    private void ResizeCropFrame(double viewportWidth, double viewportHeight)
    {
        var availableWidth = Math.Max(1, viewportWidth - 36);
        var availableHeight = Math.Max(1, viewportHeight - 36);
        if (availableWidth / availableHeight > _aspectRatio)
        {
            CropFrame.Height = availableHeight;
            CropFrame.Width = availableHeight * _aspectRatio;
        }
        else
        {
            CropFrame.Width = availableWidth;
            CropFrame.Height = availableWidth / _aspectRatio;
        }
    }

    private void Viewport_OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!HasSource)
            return;
        _zoom = Math.Clamp(_zoom * (e.Delta > 0 ? 1.1 : 1 / 1.1), 0.02, 40);
        UpdateMediaTransform();
        e.Handled = true;
    }

    private void Viewport_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!HasSource)
            return;
        _isDragging = true;
        _lastPointer = e.GetPosition(Viewport);
        Viewport.CaptureMouse();
        Focus();
    }

    private void Viewport_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging)
            return;
        var current = e.GetPosition(Viewport);
        _pan += current - _lastPointer;
        _lastPointer = current;
        UpdateMediaTransform();
    }

    private void Viewport_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        Viewport.ReleaseMouseCapture();
    }

    private void SourceVideo_OnMediaOpened(object sender, RoutedEventArgs e)
    {
        _sourceWidth = SourceVideo.NaturalVideoWidth;
        _sourceHeight = SourceVideo.NaturalVideoHeight;
        if (SourceVideo.NaturalDuration.HasTimeSpan)
        {
            VideoSlider.Maximum = SourceVideo.NaturalDuration.TimeSpan.TotalSeconds;
            DurationText.Text = FormatTime(SourceVideo.NaturalDuration.TimeSpan);
        }
        InitializeMediaLayout();
    }

    private void SourceVideo_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
    {
        EmptyMessage.Text = $"Could not open video: {e.ErrorException.Message}";
        EmptyMessage.Visibility = Visibility.Visible;
    }

    private void VideoSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_isVideo || _updatingSlider || SourceVideo.Source is null)
            return;
        SourceVideo.Position = TimeSpan.FromSeconds(e.NewValue);
        CurrentTimeText.Text = FormatTime(SourceVideo.Position);
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (!_isVideo)
            return;
        var delta = e.Key switch
        {
            Key.Left => -1,
            Key.Right => 1,
            Key.OemComma => -1 / _frameRate,
            Key.OemPeriod => 1 / _frameRate,
            _ => 0,
        };
        if (delta == 0)
            return;
        _updatingSlider = true;
        VideoSlider.Value = Math.Clamp(VideoSlider.Value + delta, 0, VideoSlider.Maximum);
        _updatingSlider = false;
        SourceVideo.Position = TimeSpan.FromSeconds(VideoSlider.Value);
        CurrentTimeText.Text = FormatTime(SourceVideo.Position);
        e.Handled = true;
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e) =>
        SaveRequested?.Invoke(this, EventArgs.Empty);

    private static string FormatTime(TimeSpan time) =>
        $"{(int)time.TotalMinutes}:{time.Seconds:00}";
}
