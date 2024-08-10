using BusinessLayer;
using Classes;
using Classes.Common;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Text;

namespace AnimeCategories
{
    public class MainWindowViewModel : BindableBase
    {
        public int AnimeCount
        {
            get { return GetValue(() => AnimeCount); }
            set { SetValue(() => this.AnimeCount, value); }
        }

        public int TagCount
        {
            get { return GetValue(() => TagCount); }
            set { SetValue(() => this.TagCount, value); }
        }

        public bool HardMode
        {
            get { return GetValue(() => HardMode); }
            set { SetValue(() => this.HardMode, value); }
        }

        public int TopAnimeCount
        {
            get { return GetValue(() => TopAnimeCount); }
            set { SetValue(() => this.TopAnimeCount, value); }
        }

        public int YearMin
        {
            get { return GetValue(() => YearMin); }
            set { SetValue(() => this.YearMin, value); }
        }

        private AnimeCategoriesManager manager;

        public ObservableCollection<CategoryBucket> Buckets { get; set; } = new ObservableCollection<CategoryBucket>();

        public MainWindowViewModel()
        {
            HardMode = false;
            manager = new AnimeCategoriesManager(HardMode);
            AnimeCount = Convert.ToInt32(ConfigurationManager.AppSettings["ANIME_COUNT"]);
            TagCount = Convert.ToInt32(ConfigurationManager.AppSettings["TAG_COUNT"]);
            TopAnimeCount = Convert.ToInt32(ConfigurationManager.AppSettings["TOP_ANIME_COUNT"]);
            YearMin = Convert.ToInt32(ConfigurationManager.AppSettings["YearMin"]);
        }

        public string CheckResults()
        {
            var maxResult = AnimeCount;
            var result = AnimeCount;

            foreach (var bucket in Buckets)
            {
                foreach (var anime in bucket.AnimeInsideBucket)
                {
                    if (string.IsNullOrWhiteSpace(bucket.Category) || !anime.Categories.Contains(bucket.Category))
                    {
                        result--;
                    }
                }
            }

            return $"{result}/{maxResult}";
        }

        public string GetHints()
        {
            var hints = new StringBuilder();
            var animeInWrongBucket = new List<Anime>();

            foreach (var bucket in Buckets)
            {
                foreach (var anime in bucket.AnimeInsideBucket)
                {
                    if (string.IsNullOrWhiteSpace(bucket.Category) || !anime.Categories.Contains(bucket.Category))
                    {
                        animeInWrongBucket.Add(anime);
                    }
                }
            }

            //Order anime by number and generate hints
            animeInWrongBucket = animeInWrongBucket.OrderBy(x => x.Number).ToList();
            foreach (var anime in animeInWrongBucket)
            {
                hints.AppendLine($"{anime.DisplayTitle}");

            }


            if (hints.Length == 0)
            {
                hints.AppendLine("You don't need hints :)");
            }
            else
            {
                hints.Insert(0, "The following anime are in the wrong category:" + Environment.NewLine);
            }

            return hints.ToString();
        }

        public void StartGame()
        {
            Buckets.Clear();
            GenerateNewGame();
        }

        public void SolveGame()
        {
            //First bucket to last bucket
            for (int i = 0; i < Buckets.Count; i++)
            {
                var bucket = Buckets[i];
                for (int j = bucket.AnimeInsideBucket.Count - 1; j >= 0; j--)
                {
                    var anime = bucket.AnimeInsideBucket[j];

                    //Move anime to next bucket if it is in the wrong bucket
                    if (!anime.Categories.Contains(bucket.Category) && i+1 < Buckets.Count)
                    {
                        bucket.AnimeInsideBucket.Remove(anime);
                        Buckets[i + 1].AnimeInsideBucket.Add(anime);
                    }
                }
            }

            //Last bucket to first bucket
            for (int i = Buckets.Count - 1; i >= 0; i--)
            {
                var bucket = Buckets[i];
                for (int j = bucket.AnimeInsideBucket.Count - 1; j >= 0; j--)
                {
                    var anime = bucket.AnimeInsideBucket[j];

                    //Move anime to previous bucket if it is in the wrong bucket
                    if (!anime.Categories.Contains(bucket.Category) && i - 1 >= 0)
                    {
                        bucket.AnimeInsideBucket.Remove(anime);
                        Buckets[i - 1].AnimeInsideBucket.Add(anime);
                    }
                }
            }
        }

        private void GenerateNewGame()
        {
            manager.ReloadAnime(HardMode, TopAnimeCount, YearMin);
            var tags = manager.GetRandomTags(TagCount, AnimeCount);
            var animes = manager.GetRandomAnime(tags, TagCount, AnimeCount);

            //Add the starting bucket containing all the anime at first index
            var startingBucket = new CategoryBucket("", animes);
            Buckets.Add(startingBucket);

            foreach (var tag in tags)
            {
                Buckets.Add(new CategoryBucket(tag));
            }

        }
    }
}
