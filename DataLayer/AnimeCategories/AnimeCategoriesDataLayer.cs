using Classes;
using System.Text.Json;

namespace DataLayer.AnimeCategories
{
    public static class AnimeCategoriesDataLayer
    {
        public static List<Anime> GetAllAnimes()
        {
            string filename = "./Data/data.json";
            string jsonString = File.ReadAllText(filename);

            var data = JsonSerializer.Deserialize<List<SimpleAnimeJson>>(jsonString).AsEnumerable();

            //Apply filters
            data = data.Where(x => x.year > 2010);
            data = data.Where(x => x.type == "TV");

            return data.Select(x => new Anime(x.title, x.image, x.tags)).ToList();
        }
    }
}
