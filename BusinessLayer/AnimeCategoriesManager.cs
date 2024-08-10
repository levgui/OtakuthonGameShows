using Classes;
using DataLayer.AnimeCategories;
using System.Xml.Linq;

namespace BusinessLayer
{
    public class AnimeCategoriesManager
    {
        private IEnumerable<Anime> allAnimes = new List<Anime>();
        private IEnumerable<Anime> filteredAnimes = new List<Anime>();
        private bool previousDifficulty = false;
        public AnimeCategoriesManager(bool isHardMode)
        {
            allAnimes = AnimeCategoriesDataLayer.GetAllAnimes(isHardMode);
        }

        public void ReloadAnime(bool isHardMode, int topAnimeCount, int yearMin)
        {
            if (isHardMode != previousDifficulty)
            {
                previousDifficulty = isHardMode;

                allAnimes = AnimeCategoriesDataLayer.GetAllAnimes(isHardMode);
            }

            //Apply filters...
            filteredAnimes = allAnimes.Where(x => x.Type == "TV" || x.Type == "MOVIE");
            filteredAnimes = filteredAnimes.Where(x => x.Year >= yearMin);
            filteredAnimes = filteredAnimes.OrderByDescending(x => x.Score).Take(topAnimeCount);
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
            var possibleAnime = filteredAnimes.Where(x => tags.Intersect(x.Categories).Any()).ToList();
            var selectedAnimes = new List<Anime>();

            if (possibleAnime.Count < animeCount)
            {
                throw new Exception("This configuration seems impossible to generate. Please revise the configuration.");
            }
            else if (possibleAnime.Count == animeCount)
            {
                selectedAnimes.AddRange(possibleAnime);
            }
            else
            {

                do
                {
                    Random rnd = new Random();
                    var animeToAdd = possibleAnime[rnd.Next(possibleAnime.Count)];
                    if (!selectedAnimes.Any(x => x.Title.Equals(animeToAdd.Title)))
                    {
                        selectedAnimes.Add(animeToAdd);
                    }

                } while (selectedAnimes.Count < animeCount);
            }

            //Make sure each tag has at least one anime if possible
            if (animeCount >= tagCount)
            {
                foreach (var tag in tags)
                {
                    do
                    {
                        if (!selectedAnimes.Any(x => x.Categories.Contains(tag)))
                        {
                            var tagPossibleAnime = filteredAnimes.Where(x => x.Categories.Contains(tag)).ToList();

                            if (tagPossibleAnime.Any())
                            {
                                Random rnd = new Random();
                                var animeToAdd = tagPossibleAnime[rnd.Next(tagPossibleAnime.Count)];
                                if (!selectedAnimes.Any(x => x.Title.Equals(animeToAdd.Title)))
                                {
                                    selectedAnimes.Add(animeToAdd);
                                }
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
            var possibleAnime = filteredAnimes.Where(x => tags.Any(x.Categories.Contains));

            var hasDeadTags = false;
            if (animeCount >= tags.Count)
            {
                //Don't keep tags that have no anime
                hasDeadTags = tags.Any(x => !filteredAnimes.Any(y => y.Categories.Contains(x)));

                if (hasDeadTags)
                {
                    foreach (var tag in tags)
                    {
                        var count = filteredAnimes.Count(x => x.Categories.Contains(tag));
                    }
                }
            }


            return possibleAnime.Count() >= animeCount && !hasDeadTags;

        }

        private List<string> GenerateTags(int count)
        {
            var allTags = AnimeCategoriesDataLayer.GetAllTags();

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
    }
}
