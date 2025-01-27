using Bogus;
using WaffleGenerator;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

namespace WoasBookApp.Services.FakeBookGen
{
    public record Book
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; }
        public string CoverURI { get; set; }
        public int Likes { get; set; }
        public List<ReviewAux> Reviews { get; set; }
    }

    public record BookAux
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
    };

    public record ReviewAux
    {
        public string Critic { get; set; }
        public string Text { get; set; }
    }

    public static class DisplayLocalesClass
    {
        public static readonly Dictionary<string, string> DisplayLocales = new Dictionary<string, string>
        {
            { "en", "English" },
            { "ru", "Русский" },
            { "fr", "Français" },
            { "eo", "Esperanto" },
        };
    }


    public class BookFaker : Faker<Book>
    {
        private List<string> auxPublishers = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), @"Services\FakeBookGen\publishers.json")));
        private List<BookAux> auxBooks;
        private List<string> auxCritics;
        private List<string> auxReviewTexts;

        private void LoadAuxBookData(string locale = "en")
        {
            string curDir = Directory.GetCurrentDirectory();
            string filepath = Path.Combine(curDir, @$"Services\FakeBookGen\books_{locale}.json");
            if (!File.Exists(filepath)) return;
                
            auxBooks = JsonSerializer.Deserialize<List<BookAux>>(File.ReadAllText(filepath));
        }

        private void LoadAuxReviews(string locale = "en")
        {
            string curDir = Directory.GetCurrentDirectory();
            string filepath = Path.Combine(curDir, @$"Services\FakeBookGen\reviews_{locale}.json");
            if (!File.Exists(filepath)) return;
            
            var jsrevs = JsonSerializer.Deserialize<Dictionary<string,List<string>>>(File.ReadAllText(filepath));
            auxCritics = jsrevs["critics"];
            auxReviewTexts = jsrevs["reviews"];
        }

        public BookFaker(int seed, float likes, float reviews, string locale = "en")
        {
            auxBooks = new List<BookAux>() {
                    new BookAux() {
                        Title       = "UNSUPPORTED LOCALE",
                        Description = "UNSUPPORTED LOCALE",
                        Author      = "UNSUPPORTED LOCALE",
                        Genre       = "UNSUPPORTED LOCALE" } };
            LoadAuxBookData(locale);
            auxCritics = new List<string>() { "UNSUPPORTED LOCALE" };
            auxReviewTexts = new List<string>() { "UNSUPPORTED LOCALE" };
            LoadAuxReviews(locale);

            Randomizer.Seed = new Random(($"{seed}{likes}{reviews}{locale}").GetHashCode());

            StrictMode(true);
            RuleFor(b => b.Title, f => f.PickRandom(auxBooks).Title);
            RuleFor(b => b.Description, f => f.PickRandom(auxBooks).Description);
            RuleFor(b => b.Author, f => f.PickRandom(auxBooks).Author);
            RuleFor(b => b.Genre, f => f.PickRandom(auxBooks).Genre);
            RuleFor(b => b.Publisher, f => f.PickRandom(auxPublishers));
            RuleFor(b => b.Year, f => f.Date.Past(100).Year);
            RuleFor(b => b.ISBN, f => f.Random.ReplaceNumbers("978-#-###-#####-#"));
            RuleFor(b => b.CoverURI, f => f.Image.PicsumUrl(200, 300));
            RuleFor(b => b.Reviews, f => GenerateReviews(f, reviews));
            RuleFor(b => b.Likes, f => GenerateRandomIntAtleast(f, likes));
        }

        private List<ReviewAux> GenerateReviews(Faker f, float amt)
        {
            var res = new List<ReviewAux>();
            var reviewsAmt = GenerateRandomIntAtleast(f, amt);
            res.AddRange(
                Enumerable.Range(0, reviewsAmt).
                Select(_ => new ReviewAux { 
                    Critic = f.PickRandom(auxCritics), 
                    Text = f.PickRandom(auxReviewTexts) 
                }));
            return res;
        }

        private int GenerateRandomIntAtleast(Faker f, float amt)
        {
            int res = (int)Math.Floor(amt);
            res += f.Random.Float() < (amt - Math.Floor(amt)) ? 1 : 0;
            Console.WriteLine($"GRIAL: ({amt}) => {res}");
            return res;
        }



    }
}
