using Classes.Common;
using System.Collections.ObjectModel;

namespace Classes
{
    public class CategoryBucket : BindableBase
    {
        public string Category { get; set; }
        public ObservableCollection<Anime> AnimeInsideBucket { get; set; }

        public CategoryBucket(string category)
        {
            Category = category;
            AnimeInsideBucket = new ObservableCollection<Anime>();
        }

        public CategoryBucket(string category, List<Anime> animes)
        {
            Category = category;
            AnimeInsideBucket = new ObservableCollection<Anime>(animes);
        }

        public void AddAnime(Anime anime)
        {
            AnimeInsideBucket.Add(anime);
        }

        public void RemoveAnime(Anime anime)
        {
            AnimeInsideBucket.Remove(anime);
        }
    }
}
