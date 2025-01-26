using Bogus;
using WaffleGenerator;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Collections.Generic;

namespace WoasBookApp.Services
{
    public class Book
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public int[] ISBN { get; set; }
        public string CoverURI { get; set; }
    }

    class BookAux
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
    };

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

        private List<BookAux> auxBooks;

        private void LoadAuxBookData(string locale = "en")
        {
            string curDir = System.IO.Directory.GetCurrentDirectory();
            string filepath = System.IO.Path.Combine(curDir, @$"Services\FakeBookGen\books_{locale}.json");
            if (System.IO.File.Exists(filepath))
                auxBooks = JsonSerializer.Deserialize<List<BookAux>>(System.IO.File.ReadAllText(filepath));
            else
                auxBooks = new List<BookAux>() { 
                    new BookAux() { 
                        Title       = "UNSUPPORTED LOCALE", 
                        Description = "UNSUPPORTED LOCALE", 
                        Author      = "UNSUPPORTED LOCALE", 
                        Genre       = "UNSUPPORTED LOCALE" } };
        }

        public BookFaker(int seed, string locale = "en")
        {
            auxBooks = new List<BookAux>();
            LoadAuxBookData(locale);
            Randomizer.Seed = new Random((seed.ToString() + locale).GetHashCode());
            
            StrictMode(true);
            RuleFor(b => b.Title, f => f.PickRandom(auxBooks).Title);
            RuleFor(b => b.Description, f => f.PickRandom(auxBooks).Description);
            RuleFor(b => b.Author, f => f.PickRandom(auxBooks).Author);
            RuleFor(b => b.Genre, f => f.PickRandom(auxBooks).Genre);
            RuleFor(b => b.Year, f => f.Date.Past(100).Year);
            RuleFor(b => b.ISBN, f => f.Random.Digits(13).ToArray());
            RuleFor(b => b.CoverURI, f => f.Image.PicsumUrl(200,300));
        }

    }
}
