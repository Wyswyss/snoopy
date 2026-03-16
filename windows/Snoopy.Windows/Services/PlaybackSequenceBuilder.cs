using Snoopy.Windows.Models;

namespace Snoopy.Windows.Services;

public static class PlaybackSequenceBuilder
{
    public static List<string> Build(List<Clip> clips, Random random)
    {
        var pool = clips.OrderBy(_ => random.Next()).ToList();
        var ordered = new List<Clip>();
        Clip? last = null;

        while (pool.Count > 0)
        {
            var enforceMatching = random.Next(2) == 0;
            Clip? next = null;

            if (enforceMatching && !string.IsNullOrWhiteSpace(last?.To))
            {
                next = pool.FirstOrDefault(x => x.From == last!.To);
                if (next is not null)
                {
                    pool.Remove(next);
                }
            }

            if (next is null)
            {
                next = pool[0];
                pool.RemoveAt(0);
            }

            ordered.Add(next);
            last = next;
        }

        var urls = new List<string>();

        foreach (var clip in ordered)
        {
            if (!string.IsNullOrWhiteSpace(clip.StartUrl))
            {
                urls.Add(clip.StartUrl);
            }

            if (!string.IsNullOrWhiteSpace(clip.LoopUrl))
            {
                var repeat = clip.Repeat <= 0 ? random.Next(3, 6) : clip.Repeat;
                for (var i = 0; i < repeat; i++)
                {
                    urls.Add(clip.LoopUrl);
                }
            }

            if (!string.IsNullOrWhiteSpace(clip.EndUrl))
            {
                urls.Add(clip.EndUrl);
            }

            if (clip.Others.Count > 0)
            {
                urls.AddRange(clip.Others);
            }
        }

        return urls;
    }
}
