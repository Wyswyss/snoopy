using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnoopyScreenSaver
{
    public partial class MainWindow : Window
    {
        private readonly Random _random = new Random();
        private List<string> _videos = new();
        private List<string> _backgrounds = new();
        private int _index;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            _videos = Clip.RandomClipUrls(Clip.LoadClips(resourcePath));
            if (Directory.Exists(resourcePath))
                _backgrounds = Directory.GetFiles(resourcePath, "*.heic").ToList();
            UpdateBackground();
            PlayVideo();
        }

        private void PlayVideo()
        {
            if (_videos.Count == 0)
                return;
            if (_index >= _videos.Count)
            {
                _index = 0;
                UpdateBackground();
            }
            VideoPlayer.Source = new Uri(_videos[_index]);
            VideoPlayer.MediaEnded += VideoPlayer_MediaEnded;
            VideoPlayer.Play();
            _index++;
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            PlayVideo();
        }

        private void UpdateBackground()
        {
            if (_backgrounds.Count > 0)
            {
                var path = _backgrounds[_random.Next(_backgrounds.Count)];
                BackgroundImage.Source = new BitmapImage(new Uri(path));
            }
            var colors = new[]
            {
                Color.FromRgb(50,60,47),
                Color.FromRgb(5,168,157),
                Color.FromRgb(65,176,246),
                Color.FromRgb(238,95,167),
                Colors.Black
            };
            Background = new SolidColorBrush(colors[_random.Next(colors.Length)]);
        }

        protected override void OnKeyDown(KeyEventArgs e) => Close();
        protected override void OnMouseMove(MouseEventArgs e) => Close();
    }
}
