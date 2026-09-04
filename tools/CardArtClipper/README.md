# New Kunlun Art Clipper

Windows desktop utility for selecting and cropping image or video frames into New Kunlun card
portraits, power icons, and relic icons.

Run it from Rider, or from the repository root with:

```powershell
dotnet run --project tools/CardArtClipper/CardArtClipper.csproj
```

Use the tabs at the bottom of the left pane to switch between cards, powers, and relics. The tool
discovers localized titles and writes both the normal and `big` image for the selected model at
that asset type's native dimensions. Gallery path, font size, and thumbnail size are stored under
the current user's local application-data directory.

Video playback and thumbnail support use Windows Media Foundation through WPF, so available
video formats depend on the codecs installed in Windows.
