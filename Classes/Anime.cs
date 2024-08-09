namespace Classes
{
    public class Anime
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public List<string> Categories { get; set; }
        public string Score { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public int Number { get; set; }
        public string DisplayTitle
        {
            get
            {
                return $"{Number} - {Title}";
            }
        }

        public Anime() { }

        public Anime(string title, string image, string score, string type, string year, params string[] categories)
        {
            Title = title;
            Image = image;
            Score = score;
            Type = type.ToUpper();
            Year = year;
            Categories = categories.Select(x => x.ToUpper()).ToList();
        }
    }
}
