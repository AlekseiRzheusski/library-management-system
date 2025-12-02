using LibraryManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Migrations.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[]
                {
                    "AuthorId",
                    "FirstName",
                    "LastName",
                    "Biography",
                    "DateOfBirth",
                    "IsActive" },
                values: new object[,]
                {
                    {
                        1,
                        "Kobo",
                        "Abe",
                        "Was a Japanese writer, playwright and director. His 1962 novel The Woman in the Dunes was made into an award-winning film by Hiroshi Teshigahara in 1964.",
                        DateTime.SpecifyKind(new DateTime(1924,3,7), DateTimeKind.Utc),
                        false
                    },
                    {
                        2,
                        "Andrew",
                        "Tanenbaum",
                        "Is an American-born Dutch computer scientist and retired professor emeritus of computer science at the Vrije Universiteit Amsterdam in the Netherlands.",
                        DateTime.SpecifyKind(new DateTime(1944,3,16), DateTimeKind.Utc),
                        true
                    },
                    {
                        3,
                        "Boris",
                        "Demidovich",
                        "Soviet Belarusian mathematician",
                        DateTime.SpecifyKind(new DateTime(1906,3,2), DateTimeKind.Utc),
                        false
                    },
                    {
                        4,
                        "George",
                        "R.R. Martin",
                        "is an American author, screenwriter, and television producer. He is best known as the author of the series of epic fantasy novels A Song of Ice and Fire.",
                        DateTime.SpecifyKind(new DateTime(1948,7,20), DateTimeKind.Utc),
                        true
                    },
                    {
                        5,
                        "Anton",
                        "Chekhov",
                        "Was a Russian playwright and short-story writer, widely considered to be one of the greatest writers of all time.",
                        DateTime.SpecifyKind(new DateTime(1860,1,29), DateTimeKind.Utc),
                        false
                    },
                    {
                        6,
                        "Empty",
                        "Author",
                        "this is author entity for books without one, or test book entities",
                        DateTime.SpecifyKind(new DateTime(1860,1,29), DateTimeKind.Utc),
                        true
                    }
                }
            );

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[]
                {
                    "CategoryId",
                    "Name",
                    "Description",
                    "ParentCategoryId",
                    "SortOrder",
                    "IsActive"
                },
                values: new object[,]
                {
                    {
                        1,
                        "Fiction",
                        "Any creative work, chiefly any narrative work, portraying individuals, events, or places that are imaginary or in ways that are imaginary.",
                        null,
                        0,
                        true
                    },
                    {
                        2,
                        "Non-fiction",
                        "Any document or media content that attempts, in good faith, to convey information only about the real world, rather than being grounded in imagination.",
                        null,
                        0,
                        true
                    },
                    {
                        3,
                        "Math",
                        "Math Literature",
                        2,
                        1,
                        true
                    },
                    {
                        4,
                        "Programming",
                        "Programming Literature",
                        2,
                        1,
                        true
                    },
                    {
                        5,
                        "Literary realism",
                        "Is a movement and genre of literature that attempts to represent mundane and ordinary subject-matter in a faithful and straightforward way, avoiding grandiose or exotic subject-matter, exaggerated portrayals, and speculative elements such as supernatural events and alternative worlds.",
                        1,
                        1,
                        true
                    },
                    {
                        6,
                        "Fantasy",
                        "is a genre of speculative fiction that involves supernatural or magical elements, often including completely imaginary realms and creatures.",
                        1,
                        1,
                        true
                    },
                    {
                        7,
                        "Urban fantasy",
                        "Is a subgenre of fantasy, placing supernatural elements in a contemporary urban-affected setting.",
                        6,
                        1,
                        true
                    },
                    {
                        8,
                        "Dark fantasy",
                        "Is a subgenre of literary, artistic, and cinematic fantasy works that incorporates disturbing and frightening themes.",
                        6,
                        1,
                        false
                    }
                }
            );

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[]
                {
                    "BookId",
                    "Title",
                    "ISBN",
                    "Description",
                    "AuthorId",
                    "CategoryId",
                    "PublishedDate",
                    "PageCount",
                    "IsAvailable",
                    "CreatedDate"
                },
                values: new object[,]
                {
                    {
                        1,
                        "Exercises and Problems in Mathematical Analysis",
                        "9785507508853",
                        "Exercises and Problems in Mathematical Analysis",
                        3,
                        3,
                        DateTime.SpecifyKind(new DateTime(2017,3,12), DateTimeKind.Utc),
                        625,
                        true,
                        DateTime.UtcNow
                    },
                    {
                        2,
                        "Structured computer architecture",
                        "9780131859401",
                        "Structured Computer Organization, specifically written for undergraduate students, is a best-selling guide that provides an accessible introduction to computer hardware and architecture. This text will also serve as a useful resource for all computer professionals and engineers who need an overview or introduction to computer architecture.",
                        2,
                        4,
                        DateTime.SpecifyKind(new DateTime(2012,7,25), DateTimeKind.Utc),
                        808,
                        true,
                        DateTime.UtcNow
                    },
                    {
                        3,
                        "Computer networks",
                        "9780131668362",
                        "Is appropriate for Computer Networking or Introduction to Networking courses at both the undergraduate and graduate level in Computer Science, Electrical Engineering, CIS, MIS, and Business Departments.",
                        2,
                        4,
                        DateTime.SpecifyKind(new DateTime(2010,5,25), DateTimeKind.Utc),
                        960,
                        true,
                        DateTime.UtcNow
                    },
                    {
                        4,
                        "The Woman in the Dunes",
                        "9788478441969",
                        "The novel is intended as a commentary on the claustrophobic and limiting nature of existence, as well as a critique of certain aspects of Japanese social behavior.",
                        1,
                        5,
                        DateTime.SpecifyKind(new DateTime(1991,4,16), DateTimeKind.Utc),
                        256,
                        true,
                        DateTime.UtcNow
                    },
                    {
                        5,
                        "The Box Man",
                        "9780307813695",
                        "The novel is intended as a commentary on the claustrophobic and limiting nature of existence, as well as a critique of certain aspects of Japanese social behavior.",
                        1,
                        5,
                        DateTime.SpecifyKind(new DateTime(2001,7,10), DateTimeKind.Utc),
                        192,
                        false,
                        DateTime.UtcNow
                    },
                    {
                        6,
                        "A Game of Thrones",
                        "9780553808049",
                        "Published in celebration of the twentieth anniversary of George R. R. Martins landmark series, this lavishly illustrated special edition of A Game of Thrones—featuring gorgeous full-page artwork as well as black-and-white illustrations in every chapter—revitalizes the fantasy masterpiece that became a cultural phenomenon.",
                        4,
                        6,
                        DateTime.SpecifyKind(new DateTime(2016,10,18), DateTimeKind.Utc),
                        896,
                        false,
                        DateTime.UtcNow
                    },
                    {
                        7,
                        "A Clash of Kings",
                        "9781984821157",
                        "Continuing the celebration of the twentieth anniversary of George R. R. Martins landmark series, this gorgeously illustrated special edition of A Clash of Kings features over twenty all-new illustrations from Lauren K. Cannon, both color and black-and-white, bringing glorious new life to this modern classic.",
                        4,
                        6,
                        DateTime.SpecifyKind(new DateTime(2019,11,5), DateTimeKind.Utc),
                        896,
                        false,
                        DateTime.UtcNow
                    },
                    {
                        8,
                        "The Complete Short Novels (Everyman's Library)",
                        "9781400040490",
                        "Anton Chekhov, widely hailed as the supreme master of the short story, also wrote five works long enough to be called short novels–here brought together in one volume for the first time, in a masterly new translation by the award-winning translators Richard Pevear and Larissa Volokhonsky.",
                        5,
                        5,
                        DateTime.SpecifyKind(new DateTime(2004,8,3), DateTimeKind.Utc),
                        600,
                        false,
                        DateTime.UtcNow
                    },
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("Books", "BookId", 1);
            migrationBuilder.DeleteData("Books", "BookId", 2);
            migrationBuilder.DeleteData("Books", "BookId", 3);
            migrationBuilder.DeleteData("Books", "BookId", 4);
            migrationBuilder.DeleteData("Books", "BookId", 5);
            migrationBuilder.DeleteData("Books", "BookId", 6);
            migrationBuilder.DeleteData("Books", "BookId", 7);
            migrationBuilder.DeleteData("Books", "BookId", 8);

            migrationBuilder.DeleteData("Categories", "CategoryId", 1);
            migrationBuilder.DeleteData("Categories", "CategoryId", 2);
            migrationBuilder.DeleteData("Categories", "CategoryId", 3);
            migrationBuilder.DeleteData("Categories", "CategoryId", 4);
            migrationBuilder.DeleteData("Categories", "CategoryId", 5);
            migrationBuilder.DeleteData("Categories", "CategoryId", 6);
            migrationBuilder.DeleteData("Categories", "CategoryId", 7);
            migrationBuilder.DeleteData("Categories", "CategoryId", 8);

            migrationBuilder.DeleteData("Authors", "AuthorId", 1);
            migrationBuilder.DeleteData("Authors", "AuthorId", 2);
            migrationBuilder.DeleteData("Authors", "AuthorId", 3);
            migrationBuilder.DeleteData("Authors", "AuthorId", 4);
            migrationBuilder.DeleteData("Authors", "AuthorId", 5);
            migrationBuilder.DeleteData("Authors", "AuthorId", 6);
        }
    }
}
