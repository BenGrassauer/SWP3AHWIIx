using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace CsvReaderExample
{
    public class Person
    {
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? Telefon { get; set; }
        public string? Adresse { get; set; }
        public string? unicode { get; set; }

        string[] ToArray()
        {
            return [Fullname, Email, Telefon, Adresse, unicode];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using var reader = new StreamReader(@"persons.csv");
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                MissingFieldFound = null,
            };
            using var csv = new CsvReader(reader, config);
            var records = csv.GetRecords<Person>();

            foreach (var person in records)
            {
                if (person.unicode.Length > 0)
                {
                    Console.WriteLine(
                        $"---------------------------------\nFullname: {person.Fullname}\n Email: {person.Email}\n Telefon: {person.Telefon}\n Adresse: {person.Adresse}\n Unicode: {person.unicode}"
                    );
                }
                else
                {
                    Console.WriteLine(
                        $"---------------------------------\nFullname: {person.Fullname}\n Email: {person.Email}\n Telefon: {person.Telefon}\n Adresse: {person.Adresse}"
                    );
                }
            }
            Console.WriteLine("---------------------------------");
        }
    }
}
