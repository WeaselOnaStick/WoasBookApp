using Bogus;
using WaffleGenerator;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

using Microsoft.JSInterop;

namespace WoasBookApp.Services.FakeBookGen
{
    public record Book
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public string? Publisher { get; set; }
        public int? Year { get; set; }
        public string? ISBN { get; set; }
        public string? CoverURI { get; set; }
        public int? Likes { get; set; }
        public List<Review>? Reviews { get; set; }
    }


    public record Review
    {
        public string? Critic { get; set; }
        public string? Text { get; set; }
    }

    public record LocaleInfoData
    {
        public string DisplayName { get; set; }
        public string FlagURL { get; set; }
    }

    public static class SupportedLocales
    {
        public static readonly Dictionary<string, LocaleInfoData> LocalesInfo = new Dictionary<string, LocaleInfoData>()
        {
            {
                "en", new LocaleInfoData(){
                DisplayName = "English",
                FlagURL = @"https://upload.wikimedia.org/wikipedia/en/a/a4/Flag_of_the_United_States.svg"}
            },
            {
                "pt_BR", new LocaleInfoData(){
                DisplayName = "Brasileira",
                FlagURL = @"https://upload.wikimedia.org/wikipedia/en/0/05/Flag_of_Brazil.svg"}
            },
            {
                "pl", new LocaleInfoData(){
                DisplayName = "Polski",
                FlagURL = @"https://upload.wikimedia.org/wikipedia/en/1/12/Flag_of_Poland.svg"}
            },
        };

        public static readonly Dictionary<string, string> DisplayLocales = new Dictionary<string, string>
        {
            { "en", "English" },
        };
    }


    public class BookFaker : Faker<Book>
    {
        private readonly IJSRuntime _js;

        public BookFaker(int seed, IJSRuntime jS, float likes, float reviews, string locale = "en")
        {
            _js = jS;

            _js.InvokeVoidAsync("console.log", "Hello from Faker!");
            _js.InvokeVoidAsync("WoasFunc");

            Randomizer.Seed = new Random(seed);

            Locale = locale;

            var reviewsFaker = new Faker();
            reviewsFaker.Random = new Randomizer(seed);
            

            StrictMode(true);
            RuleFor(b => b.Title, f => locale == "en" ? f.WaffleTitle() : $"{f.Hacker.IngVerb()} {f.Hacker.Adjective()} {f.Hacker.Noun()}");
            RuleFor(b => b.Description, f => locale == "en"? f.WaffleText() : f.Lorem.Paragraph());
            RuleFor(b => b.Author, f => f.Name.FullName());
            RuleFor(b => b.Genre, f => f.Commerce.ProductAdjective());
            RuleFor(b => b.Publisher, f => f.Company.CompanyName());
            RuleFor(b => b.Year, f => f.Date.Past(100).Year);
            RuleFor(b => b.ISBN, f => f.Random.ReplaceNumbers("978-#-###-#####-#"));
            RuleFor(b => b.CoverURI, f => f.Image.PicsumUrl(200, 300));
            RuleFor(b => b.Likes, f => GenerateRandomIntAtleast(f, likes));
            RuleFor(b => b.Reviews, f => GenerateReviews(reviewsFaker, reviews));
        }

        public void RegenerateLikes(List<Book> books, float newAmt, int seed)
        {
            var faker = new Faker();
            faker.Random = new Randomizer(seed);
            foreach (var book in books)
                book.Likes = GenerateRandomIntAtleast(faker, newAmt);
        }

        public void RegenerateReviews(List<Book> books, float newAmt, int seed)
        {
            var faker = new Faker();
            faker.Random = new Randomizer(seed);
            foreach (var book in books)
                book.Reviews = GenerateReviews(faker, GenerateRandomIntAtleast(faker, newAmt));
        }

        private List<Review> GenerateReviews(Faker f, float amt)
        {
            var res = new List<Review>();
            var reviewsAmt = GenerateRandomIntAtleast(f, amt);
            res.AddRange(
                Enumerable.Range(0, reviewsAmt).
                Select(_ => new Review { 
                    Critic = f.Name.FullName(), 
                    Text = f.Rant.Review()
                }));
            return res;
        }

        private int GenerateRandomIntAtleast(Faker f, float amt)
        {
            int res = (int)Math.Floor(amt);
            res += f.Random.Float() < (amt - Math.Floor(amt)) ? 1 : 0;
            return res;
        }



    }
}
