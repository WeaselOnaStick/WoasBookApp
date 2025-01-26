using Bogus;

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
    }

    public enum Locales
    {
        en,
        ru,
        fr,
    }

    public class BookFaker : Faker<Book>
    {
        private Dictionary<Locales, string> LocaleToString = new Dictionary<Locales, string>
        {
            { Locales.en, "en" },
            { Locales.ru, "ru" },
            { Locales.fr, "fr" },
        };

        public BookFaker(int seed, Locales locale = Locales.en)
        {
            Randomizer.Seed = new Random(seed);
            Locale = LocaleToString[locale];
            StrictMode(true);
            RuleFor(b => b.Title, f => String.Join(" ",f.Lorem.Words(3)));
            RuleFor(b => b.Description, f => f.Lorem.Paragraph());
            RuleFor(b => b.Author, f => f.Name.FullName());
            RuleFor(b => b.Genre, f => f.Lorem.Word());
            RuleFor(b => b.Year, f => f.Date.Past(100).Year);
            RuleFor(b => b.ISBN, f => f.Random.Digits(13).ToArray());
        }
    }
}
