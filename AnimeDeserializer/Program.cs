// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;

var jsonString = new WebClient().DownloadString("https://raw.githubusercontent.com/manami-project/anime-offline-database/master/anime-offline-database.json");

var data = JsonSerializer.Deserialize<Rootobject>(jsonString).data.ToList();

var simpleData = data.Select(x => new SimpleAnimeJson() { title = x.title, image = x.picture, tags = x.tags, year = x.animeSeason.year, type = x.type }).ToList();

var options = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    WriteIndented = true
};
var simpleJson = JsonSerializer.Serialize<List<SimpleAnimeJson>>(simpleData, options);

File.WriteAllText("data.json", simpleJson);
