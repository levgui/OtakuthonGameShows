using Classes;
using DataLayer.AnimeCategories;

namespace BusinessLayer
{
    public class AnimeCategoriesManager
    {
        private List<Anime> allAnimes = new List<Anime>();
        public AnimeCategoriesManager()
        {
            allAnimes = AnimeCategoriesDataLayer.GetAllAnimes();
        }

        public List<string> GetRandomTags(int tagCount, int animeCount)
        {
            var tags = new List<string>();
            var retries = 0;
            do
            {
                if (retries == 100)
                {
                    throw new Exception("This configuration seems impossible to generate. Please revise the configuration.");
                }

                tags = GenerateTags(tagCount);
                retries++;
            } while (!ValidateTags(tags, animeCount));

            return tags;
        }

        public List<Anime> GetRandomAnime(List<string> tags, int tagCount, int animeCount)
        {
            var possibleAnime = allAnimes.Where(x => tags.Intersect(x.Categories).Any()).ToList();
            var selectedAnimes = new List<Anime>();

            do
            {
                Random rnd = new Random();
                var animeToAdd = possibleAnime[rnd.Next(possibleAnime.Count)];
                if (!selectedAnimes.Any(x => x.Title.Equals(animeToAdd.Title)))
                {
                    selectedAnimes.Add(animeToAdd);
                }

            } while (selectedAnimes.Count < animeCount);

            //Make sure each tag has at least one anime if possible
            if (animeCount >= tagCount)
            {
                foreach (var tag in tags)
                {
                    do
                    {
                        if (!selectedAnimes.Any(x => x.Categories.Contains(tag)))
                        {
                            var tagPossibleAnime = allAnimes.Where(x => x.Categories.Contains(tag)).ToList();

                            Random rnd = new Random();
                            var animeToAdd = tagPossibleAnime[rnd.Next(tagPossibleAnime.Count)];
                            if (!selectedAnimes.Any(x => x.Title.Equals(animeToAdd.Title)))
                            {
                                selectedAnimes.Add(animeToAdd);
                            }
                        }
                    } while (!selectedAnimes.Any(x => x.Categories.Contains(tag)));
                }
            }

            //Remove anime where we can if we added too many
            if (selectedAnimes.Count > animeCount)
            {
                do
                {
                    foreach (var tag in tags)
                    {
                        if (selectedAnimes.Count(x => x.Categories.Contains(tag)) > 1 && selectedAnimes.Count > animeCount)
                        {
                            var canDeleteAnimes = selectedAnimes.Where(x => x.Categories.Contains(tag));
                            selectedAnimes.Remove(canDeleteAnimes.First());
                        }
                    }

                } while (selectedAnimes.Count > animeCount);
            }

            //Give number to each anime
            int i = 1;
            foreach (var anime in selectedAnimes)
            {
                anime.Number = i++;
            }

            return selectedAnimes;

        }

        private bool ValidateTags(List<string> tags, int animeCount)
        {
            //There needs to be enough different anime to match the tags
            var possibleAnime = allAnimes.Where(x => tags.Any(x.Categories.Contains));

            return possibleAnime.Count() >= animeCount;

        }

        private List<string> GenerateTags(int count)
        {
            var allTags = GetAllTags();

            var selectedTags = new List<string>();

            do
            {
                Random rnd = new Random();
                var tag = allTags[rnd.Next(allTags.Count)];
                if (!selectedTags.Any(x => x.Equals(tag)))
                {
                    selectedTags.Add(tag);
                }

            } while (selectedTags.Count < count);

            return selectedTags;
        }

        private List<string> GetAllTags()
        {
            //for all json weird tags
            //return allAnimes.SelectMany(x => x.Categories).Select(x => x.ToUpper()).Distinct().ToList();

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
                "Otaku Culture",
                "Parody",
                "Performing Arts",
                "Pets",
                "Psychological",
                "Racing",
                "Reincarnation",
                "Reverse Harem",
                "Romantic Subtext",
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
                "Visual Arts",
                "Workplace"
            };

            return tags.Select(x => x.ToUpper()).ToList();
        }
    }
}
