namespace Classes
{
    public class Anime
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public List<string> Categories { get; set; }
        public int Number { get; set; }
        public string DisplayTitle
        {
            get
            {
                return $"{Number} - {Title}";
            }
        }

        public Anime(string title, string image, params string[] categories)
        {
            Title = title;
            Image = image;
            Categories = categories.Select(x => x.ToUpper()).ToList();
        }
    }
}
