using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Snoopy.Windows.Services;

namespace Snoopy.Windows;

public partial class MainWindow : Window
{
    private readonly Random _random = new();
    private readonly ClipService _clipService = new();
    private List<string> _playlist = new();
    private int _playlistIndex;

    private readonly Color[] _colors =
    {
        Color.FromRgb(50, 60, 47),
        Color.FromRgb(5, 168, 157),
        Color.FromRgb(65, 176, 246),
        Color.FromRgb(238, 95, 167),
        Colors.Black,
    };

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TryLoadPattern();
        RefreshBackground();
        BuildAndPlayPlaylist();
    }

    private void BuildAndPlayPlaylist()
    {
        var clips = _clipService.LoadClips();
        _playlist = PlaybackSequenceBuilder.Build(clips, _random);
        _playlistIndex = 0;

        if (_playlist.Count == 0)
        {
            MessageBox.Show(
                "未找到视频素材。请将 tvOS 提取的视频放到 Assets/Videos。",
                "Snoopy for Windows",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            Close();
            return;
        }

        PlayCurrent();
    }

    private void PlayCurrent()
    {
        var relativePath = _playlist[_playlistIndex];
        var absolutePath = Path.Combine(AssetLocator.VideosDirectory, relativePath);

        if (!File.Exists(absolutePath))
        {
            _playlistIndex = (_playlistIndex + 1) % _playlist.Count;
            PlayCurrent();
            return;
        }

        VideoLayer.Source = new Uri(absolutePath, UriKind.Absolute);
        VideoLayer.Position = TimeSpan.Zero;
        VideoLayer.Play();
    }

    private void OnVideoEnded(object sender, RoutedEventArgs e)
    {
        _playlistIndex = (_playlistIndex + 1) % _playlist.Count;

        if (_playlistIndex == 0)
        {
            RefreshBackground();
            var clips = _clipService.LoadClips();
            _playlist = PlaybackSequenceBuilder.Build(clips, _random);
        }

        PlayCurrent();
    }

    private void RefreshBackground()
    {
        BackgroundColorLayer.Fill = new SolidColorBrush(_colors[_random.Next(_colors.Length)]);

        var image = _clipService.RandomBackgroundPath();
        if (image is null)
        {
            BackgroundImageLayer.Source = null;
            return;
        }

        BackgroundImageLayer.Source = new BitmapImage(new Uri(image, UriKind.Absolute));
    }

    private void TryLoadPattern()
    {
        if (!File.Exists(AssetLocator.HalftonePatternPath))
        {
            return;
        }

        PatternLayer.Source = new BitmapImage(new Uri(AssetLocator.HalftonePatternPath, UriKind.Absolute));
    }

    private void OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
    {
        OnVideoEnded(sender, new RoutedEventArgs());
    }

    private void OnExitInput(object sender, InputEventArgs e)
    {
        Close();
    }
}
