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
                Console.WriteLine(
                    $"Fullname: {person.Fullname}, Email: {person.Email}, Telefon: {person.Telefon}, Adresse: {person.Adresse}, Unicode: {person.unicode}"
                );
            }
        }
    }
}
