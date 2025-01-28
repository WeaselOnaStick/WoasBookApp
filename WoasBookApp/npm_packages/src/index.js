import { faker, Faker, es } from '@faker-js/faker';

window.WoasFunc = function () {
    console.log("Hello, JS world!");
    // const { faker } = require('@faker-js/faker');

    faker.seed(1234);

    console.log(faker.person.fullName());
    faker.person.firstName(); // 'John'
    faker.person.lastName(); // 'Doe'
    // const { Faker, es } = require('@faker-js/faker');

    // create a Faker instance with only es data and no en fallback (=> smaller bundle size)
    const customFaker = new Faker({ locale: [es] });
    customFaker.seed(1234);

    customFaker.person.firstName(); // 'Javier'
    customFaker.person.lastName(); // 'Ocampo Corrales'

    customFaker.music.genre(); // throws Error as this data is not available in `es`

}