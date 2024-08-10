using Classes;
using CsvHelper;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.Json;

namespace DataLayer.AnimeCategories
{
    public static class AnimeCategoriesDataLayer
    {
        public static IEnumerable<Anime> GetAllAnimes(bool isHardMode)
        {
            IEnumerable<Anime> anime;
            if (isHardMode)
            {
                anime =  GetAnimeFromHardJsonData().AsEnumerable();
            }
            else
            {
                anime = GetAnimeFromCsv().AsEnumerable();
            }

            return anime;
        }

        public static List<string> GetAllTags()
        {
            //MAL tags
            var tags = new List<string>()
            {
                "Action",
                "Adventure",
                //"Avant Garde",
                //"Award Winning",
                "Boys Love",
                "Comedy",
                "Drama",
                "Fantasy",
                "Girls Love",
                "Gourmet",
                "Horror",
                "Mystery",
                "Romance",
                "Sci-Fi",
                "Slice of Life",
                "Sports",
                "Supernatural",
                "Suspense",
                "Ecchi",
                //"Erotica",
                //"Hentai",
                "Adult Cast",
                "Anthropomorphic",
                "CGDCT",
                "Childcare",
                //"Combat Sports",
                "Crossdressing",
                "Delinquents",
                "Detective",
                "Educational",
                "Gag Humor",
                "Gore",
                "Harem",
                "High Stakes Game",
                "Historical",
                "Idol",
                "Isekai",
                "Iyashikei",
                "Love Polygon",
                //"Magical Sex Shift",
                "Mahou Shoujo",
                "Martial Arts",
                "Mecha",
                "Medical",
                "Military",
                "Music",
                "Mythology",
                "Organized Crime",
                //"Otaku Culture",
                "Parody",
                "Performing Arts",
                "Pets",
                "Psychological",
                "Racing",
                "Reincarnation",
                "Reverse Harem",
                //"Romantic Subtext",
                "Samurai",
                "School",
                "Showbiz",
                "Space",
                "Strategy Game",
                "Super Power",
                "Survival",
                "Team Sports",
                "Time Travel",
                "Vampire",
                "Video Game",
                //"Visual Arts",
                "Workplace"
            };

            return tags.Select(x => x.ToUpper()).ToList();
        }

        private static List<Anime> GetAnimeFromHardJsonData()
        {
            string filename = "./Data/data.json";
            string jsonString = File.ReadAllText(filename);

            var data = JsonSerializer.Deserialize<List<SimpleAnimeJson>>(jsonString).AsEnumerable();

            return data.Select(x => new Anime(x.title, x.image, "0", x.type, x.year.ToString(), x.tags)).ToList();
        }

        private static List<Anime> GetAnimeFromCsv()
        {
            var data = new List<Anime>();
            using (var reader = new StreamReader("./Data/jikan_final.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    dynamic images = JObject.Parse(csv.GetField("images"));
                    string image = images.jpg.image_url;

                    var title = csv.GetField("title_english");
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        title = csv.GetField("title");
                    }
                    var type = csv.GetField("type");
                    var score = csv.GetField("score");
                    var year = csv.GetField("year");

                    dynamic genres = JArray.Parse(csv.GetField("genres"));
                    List<string> genreList = new List<string>();
                    foreach (var genre in genres)
                    {
                        genreList.Add(genre.name.ToString());
                    }

                    dynamic themes = JArray.Parse(csv.GetField("themes"));
                    List<string> themeList = new List<string>();
                    foreach (var theme in themes)
                    {
                        themeList.Add(theme.name.ToString());
                    }

                    var categories = genreList.Union(themeList);
                    data.Add(new Anime(title, image, score, type, year, categories.ToArray()));
                }
            }

            return data;
        }
    }
}
