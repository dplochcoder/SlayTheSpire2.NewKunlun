using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CardArtClipper;

internal static class VideoThumbnailer
{
    public static async Task<BitmapSource?> CreateAsync(
        string path,
        int size,
        Dispatcher dispatcher,
        CancellationToken cancellationToken
    )
    {
        await dispatcher.InvokeAsync(
            () => { },
            DispatcherPriority.ApplicationIdle,
            cancellationToken
        );
        cancellationToken.ThrowIfCancellationRequested();

        var completion = new TaskCompletionSource<BitmapSource?>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        MediaElement? media = null;
        DispatcherTimer? timeout = null;

        await dispatcher.InvokeAsync(
            () =>
            {
                media = new MediaElement
                {
                    LoadedBehavior = MediaState.Manual,
                    UnloadedBehavior = MediaState.Manual,
                    ScrubbingEnabled = true,
                    IsMuted = true,
                    Volume = 0,
                    Width = size,
                    Height = size,
                    Stretch = Stretch.UniformToFill,
                    Source = new Uri(path),
                };
                media.MediaOpened += async (_, _) =>
                {
                    media.Play();
                    await Task.Delay(120, cancellationToken).ConfigureAwait(true);
                    media.Pause();
                    media.Measure(new Size(size, size));
                    media.Arrange(new Rect(0, 0, size, size));
                    media.UpdateLayout();
                    var bitmap = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
                    bitmap.Render(media);
                    bitmap.Freeze();
                    completion.TrySetResult(bitmap);
                };
                media.MediaFailed += (_, _) => completion.TrySetResult(null);
                timeout = new DispatcherTimer(
                    TimeSpan.FromSeconds(8),
                    DispatcherPriority.ApplicationIdle,
                    (_, _) => completion.TrySetResult(null),
                    dispatcher
                );
                timeout.Start();
                media.Play();
            },
            DispatcherPriority.ApplicationIdle,
            cancellationToken
        );

        using var registration = cancellationToken.Register(() =>
            completion.TrySetCanceled(cancellationToken)
        );
        try
        {
            return await completion.Task.ConfigureAwait(false);
        }
        finally
        {
            await dispatcher.InvokeAsync(() =>
            {
                timeout?.Stop();
                media?.Stop();
                if (media is not null)
                    media.Source = null;
            });
        }
    }
}
