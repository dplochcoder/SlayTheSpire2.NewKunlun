# Card Art Clipper

Windows desktop utility for selecting and cropping image or video frames into New Kunlun card
portraits.

Run it from Rider, or from the repository root with:

```powershell
dotnet run --project tools/CardArtClipper/CardArtClipper.csproj
```

The tool discovers cards and localized titles from `NewKunlunCode/Cards`. Saving writes both
the 250×190 portrait and its 1000×760 `big` counterpart. Gallery path, font size, and thumbnail
size are stored under the current user's local application-data directory.

Video playback and thumbnail support use Windows Media Foundation through WPF, so available
video formats depend on the codecs installed in Windows.
