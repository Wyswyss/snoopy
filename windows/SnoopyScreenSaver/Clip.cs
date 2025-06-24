using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SnoopyScreenSaver
{
    public class Clip
    {
        public string Name { get; set; } = string.Empty;
        public string? StartURL { get; set; }
        public string? LoopURL { get; set; }
        public string? EndURL { get; set; }
        public int Repeat { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public List<string>? Others { get; set; }

        public static List<Clip> LoadClips(string resourcePath)
        {
            if (!Directory.Exists(resourcePath))
                return new List<Clip>();
            var movFiles = Directory.GetFiles(resourcePath, "*.mov").Select(Path.GetFileName).ToList();
            var clipsDict = new Dictionary<string, Clip>();
            var rnd = new Random();
            foreach (var file in movFiles)
            {
                var groupName = file.Length >= 9 ? file.Substring(0, 9) : file;
                if (!clipsDict.TryGetValue(groupName, out var clip))
                {
                    clip = new Clip { Name = groupName };
                    clipsDict[groupName] = clip;
                }
                bool checkedFlag = false;
                if (file.Contains("Intro"))
                {
                    checkedFlag = true;
                    clip.StartURL = Path.Combine(resourcePath, file);
                    if (file.Contains("From"))
                    {
                        var nameWithout = Path.GetFileNameWithoutExtension(file);
                        clip.From = nameWithout.Substring(nameWithout.Length - 5);
                    }
                }
                if (file.Contains("Loop"))
                {
                    checkedFlag = true;
                    clip.LoopURL = Path.Combine(resourcePath, file);
                    clip.Repeat = rnd.Next(3, 6);
                }
                if (file.Contains("Outro"))
                {
                    checkedFlag = true;
                    clip.EndURL = Path.Combine(resourcePath, file);
                    if (file.Contains("To"))
                    {
                        var nameWithout = Path.GetFileNameWithoutExtension(file);
                        clip.To = nameWithout.Substring(nameWithout.Length - 5);
                    }
                }
                if (!checkedFlag)
                {
                    clip.Others ??= new List<string>();
                    clip.Others.Add(Path.Combine(resourcePath, file));
                }
            }
            return clipsDict.Values.ToList();
        }

        public static List<string> RandomClipUrls(List<Clip> clips)
        {
            var rnd = new Random();
            var mutable = clips.ToList();
            for (int i = mutable.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (mutable[i], mutable[j]) = (mutable[j], mutable[i]);
            }
            var shuffled = new List<Clip>();
            Clip? last = null;
            while (mutable.Count > 0)
            {
                bool enforce = rnd.Next(2) == 0;
                Clip next = null!;
                if (enforce && last != null && last.To != null)
                {
                    int matchIndex = mutable.FindIndex(c => c.From != null && c.From == last.To);
                    if (matchIndex != -1)
                    {
                        next = mutable[matchIndex];
                        mutable.RemoveAt(matchIndex);
                    }
                }
                if (next == null)
                {
                    next = mutable[0];
                    mutable.RemoveAt(0);
                }
                shuffled.Add(next);
                last = next;
            }
            var urlList = new List<string>();
            foreach (var clip in shuffled)
            {
                if (clip.StartURL != null) urlList.Add(clip.StartURL);
                if (clip.LoopURL != null)
                    for (int i = 0; i < clip.Repeat; i++)
                        urlList.Add(clip.LoopURL);
                if (clip.EndURL != null) urlList.Add(clip.EndURL);
                if (clip.Others != null) urlList.AddRange(clip.Others);
            }
            return urlList;
        }
    }
}
