using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CrowdSource.Data;
using CrowdSource.Models.CoreModels;

namespace CrowdSource.Tools.Commands
{
    public class SeedDbCommand : ICommand
    {
        public async Task RunAsync(IWebHost host, string[] args)
        {
            var services = (IServiceScopeFactory)host.Services.GetService(typeof(IServiceScopeFactory));
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                Console.WriteLine("Seeding Database...");

                // Add Collections
                if (context.Collections.Count() == 0)
                {
                    context.Collections.Add(new Collection { Name = "ADFD" });
                    await context.SaveChangesAsync();
                }
                else
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

                    await context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Skipped: FieldTypes");
                }

                Console.WriteLine($"Count: {context.FieldTypes.Count()}");
            }
        }
    }
}
