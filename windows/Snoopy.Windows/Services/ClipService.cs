using System.IO;
using Snoopy.Windows.Models;

namespace Snoopy.Windows.Services;

public class ClipService
{
    public List<Clip> LoadClips()
    {
        if (!Directory.Exists(AssetLocator.VideosDirectory))
        {
            return new List<Clip>();
        }

        var files = Directory
            .GetFiles(AssetLocator.VideosDirectory)
            .Where(x => x.EndsWith(".mov", StringComparison.OrdinalIgnoreCase) || x.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
            .Select(Path.GetFileName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .ToList();

        var clips = new Dictionary<string, Clip>();

        foreach (var file in files)
        {
            var groupName = file.Length >= 9 ? file[..9] : file;

            if (!clips.TryGetValue(groupName, out var clip))
            {
                clip = new Clip { Name = groupName };
                clips[groupName] = clip;
            }

            var withoutExt = Path.GetFileNameWithoutExtension(file);

            if (file.Contains("Intro", StringComparison.OrdinalIgnoreCase))
            {
                clip.StartUrl = file;
                if (file.Contains("From", StringComparison.OrdinalIgnoreCase) && withoutExt.Length >= 5)
                {
                    clip.From = withoutExt[^5..];
                }
                continue;
            }

            if (file.Contains("Loop", StringComparison.OrdinalIgnoreCase))
            {
                clip.LoopUrl = file;
                clip.Repeat = Random.Shared.Next(3, 6);
                continue;
            }

            if (file.Contains("Outro", StringComparison.OrdinalIgnoreCase))
            {
                clip.EndUrl = file;
                if (file.Contains("To", StringComparison.OrdinalIgnoreCase) && withoutExt.Length >= 5)
                {
                    clip.To = withoutExt[^5..];
                }
                continue;
            }

            clip.Others.Add(file);
        }

        return clips.Values.ToList();
    }

    public string? RandomBackgroundPath()
    {
        if (!Directory.Exists(AssetLocator.BackgroundsDirectory))
        {
            return null;
        }

        var files = Directory
            .GetFiles(AssetLocator.BackgroundsDirectory)
            .Where(x => x.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                     || x.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                     || x.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                     || x.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (files.Count == 0)
        {
            return null;
        }

        return files[Random.Shared.Next(files.Count)];
    }
}
