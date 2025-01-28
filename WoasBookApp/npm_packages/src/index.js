import { faker, Faker, fakerPT_BR, fakerPL, fakerEN } from '@faker-js/faker';


window.GenerateFakeBookInfo = function (seed, locale, info) {
    let newFaker;
    switch (locale) {
        case 'en':
            newFaker = fakerEN;
            break;
        case 'pt_BR':
            newFaker = fakerPT_BR
            break;
        case 'pl':
            newFaker = fakerPL
            break;
    }
    newFaker.seed(seed);
    switch (info) {
        case 'title':
            return newFaker.book.title();
        case 'author':
            return newFaker.book.author();
        case 'genre':
            return newFaker.book.genre();
        case 'publisher':
            return newFaker.book.publisher();
        case 'words':
            return newFaker.word.words({ count: { min: 10, max: 30 } })
        case 'fullname':
            return newFaker.person.fullName();
    }
}