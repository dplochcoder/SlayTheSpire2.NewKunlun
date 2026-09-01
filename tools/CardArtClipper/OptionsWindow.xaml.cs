using System.Globalization;
using System.Windows;

namespace CardArtClipper;

public partial class OptionsWindow : Window
{
    public OptionsWindow(double fontSize, int thumbnailSize)
    {
        InitializeComponent();
        FontSizeBox.Text = fontSize.ToString(CultureInfo.CurrentCulture);
        ThumbnailSizeBox.Text = thumbnailSize.ToString(CultureInfo.CurrentCulture);
    }

    public double SelectedFontSize { get; private set; }
    public int SelectedThumbnailSize { get; private set; }

    private void Save_OnClick(object sender, RoutedEventArgs e)
    {
        if (
            !double.TryParse(FontSizeBox.Text, out var fontSize)
            || fontSize is < 9 or > 36
            || !int.TryParse(ThumbnailSizeBox.Text, out var thumbnailSize)
            || thumbnailSize is < 48 or > 400
        )
        {
            MessageBox.Show(
                this,
                "Font size must be 9–36 and thumbnail size must be 48–400.",
                "Invalid options",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        SelectedFontSize = fontSize;
        SelectedThumbnailSize = thumbnailSize;
        DialogResult = true;
    }
}
