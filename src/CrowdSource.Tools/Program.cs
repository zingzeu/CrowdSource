using System;
using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using CrowdSource.Models.CoreModels;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(System.AppContext.BaseDirectory);
        //var Configuration = new ConfigurationBuilder()
        //       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        //Console.WriteLine(new ConfigurationBuilder().GetFileProvider().GetType().Name);
        
        var context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Server=127.0.0.1;Port=5432;Database=crowdsource;User Id=postgres;Password=root;")
            .Options);

        Console.WriteLine("Seeding Database...");

        // Add Collections
        if (context.Collections.Count() == 0)
        {
            context.Collections.Add(new Collection { Name = "ADFD" });
            context.SaveChanges();
        } else
        {
            Console.WriteLine("Skipped: Collections");
        }

        var ADFDCollection = context.Collections.First();


        // Add FieldTypes
        if (context.FieldTypes.Count() == 0)
        {
            context.FieldTypes.Add(new FieldType
            {
                DataType = FieldDataType.StringType,
                Name = "TextBUC",
                Collection = ADFDCollection,
                Description = ""
            });

            context.FieldTypes.Add(new FieldType
            {
                DataType = FieldDataType.StringType,
                Name = "TextChinese",
                Collection = ADFDCollection,
                Description = ""
            });

            context.FieldTypes.Add(new FieldType
            {
                DataType = FieldDataType.StringType,
                Name = "TextEnglish",
                Collection = ADFDCollection,
                Description = ""
            });

            context.FieldTypes.Add(new FieldType
            {
                DataType = FieldDataType.BooleanType,
                Name = "IsOral",
                Collection = ADFDCollection,
                Description = ""
            });

            context.FieldTypes.Add(new FieldType
            {
                DataType = FieldDataType.BooleanType,
                Name = "IsLiterary",
                Collection = ADFDCollection,
                Description = ""
            });

            context.FieldTypes.Add(new FieldType
            {
                DataType = FieldDataType.BooleanType,
                Name = "IsPivotRow",
                Collection = ADFDCollection,
                Description = ""
            });

            context.FieldTypes.Add(new FieldType
            {
                DataType = FieldDataType.StringType,
                Name = "BoPoMoFo",
                Collection = ADFDCollection,
                Description = ""
            });

            context.FieldTypes.Add(new FieldType
            {
                DataType = FieldDataType.StringType,
                Name = "Radical",
                Collection = ADFDCollection,
                Description = ""
            });

            context.SaveChanges();
        }
        else {
            Console.WriteLine("Skipped: FieldTypes");
        }

        Console.WriteLine($"Count: {context.FieldTypes.Count()}");

        Console.ReadLine();
    }
}