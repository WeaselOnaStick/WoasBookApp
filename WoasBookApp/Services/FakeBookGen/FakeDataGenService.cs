using Bogus;
using WaffleGenerator;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;

using Microsoft.JSInterop;
using System.ComponentModel;

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

        private Faker f;
        private Faker reviewsFaker;
        private int Seed;

        private float Likes;
        private float ReviewsAmount;

        public BookFaker(int seed, IJSRuntime jS, float likes, float reviews, string locale = "en")
        {
            _js = jS;

            Randomizer.Seed = new Random(seed);

            Locale = locale;
            Seed = seed;

            f = new Faker(locale);
            f.Random = new Randomizer(seed);

            reviewsFaker = new Faker(locale);
            reviewsFaker.Random = new Randomizer(seed);

            Likes = likes;
            ReviewsAmount = reviews;
        }

        public async Task<List<Book>> GenerateAsync(int N)
        {
            var books = new List<Book>();
            for (int i = 0; i < N; i++)
            {
                var book = new Book();
                book.Title = await _js.InvokeAsync<string>("GenerateFakeBookInfo", Seed + f.Random.Int(), Locale, "title");
                book.Description = Locale == "en" ? f.WaffleText() : await _js.InvokeAsync<string>("GenerateFakeBookInfo", Seed + f.Random.Int(), Locale, "words");
                book.Author = f.Name.FullName();
                book.Genre = await _js.InvokeAsync<string>("GenerateFakeBookInfo", Seed + f.Random.Int(), Locale, "genre");
                book.Publisher = await _js.InvokeAsync<string>("GenerateFakeBookInfo", Seed + f.Random.Int(), Locale, "publisher");
                book.Year = f.Date.Past(100).Year;
                book.ISBN = f.Random.ReplaceNumbers("978-#-###-#####-#");
                book.CoverURI = f.Image.PicsumUrl(200, 300);
                book.Likes = GenerateRandomIntAtleast(f, Likes);
                book.Reviews = GenerateReviews(reviewsFaker, ReviewsAmount);
                books.Add(book);
            }
            return books;
        }

        public List<Book> RegenerateLikes(List<Book> books, float newAmt, int seed)
        {
            var res = new List<Book>();
            var faker = new Faker();
            faker.Random = new Randomizer(seed);
            foreach (var book in books)
            {
                book.Likes = GenerateRandomIntAtleast(faker, newAmt);
                res.Add(book);
            }
            return res;
        }

        public List<Book> RegenerateReviews(List<Book> books, float newAmt, int seed)
        {
            var res = new List<Book>();
            var tfaker = new Faker();
            tfaker.Random = new Randomizer(seed);
            foreach (var book in books)
            {
                book.Reviews = GenerateReviews(tfaker, GenerateRandomIntAtleast(tfaker, newAmt));
                res.Add(book);
            }
            return res;
        }

        private List<Review> GenerateReviews(Faker f, float amt)
        {
            var res = new List<Review>();
            var reviewsAmt = GenerateRandomIntAtleast(f, amt);
            for (int i = 0; i < reviewsAmt; i++)
            {
                var newCritic = reviewsFaker.Name.FullName();
                var newReview = reviewsFaker.Rant.Review();
                res.Add(new Review()
                {
                    Critic = newCritic,
                    Text = newReview,
                });
            }
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
