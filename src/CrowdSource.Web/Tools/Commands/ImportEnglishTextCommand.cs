using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using CrowdSource.Data;
using Microsoft.Extensions.DependencyInjection;
using CsvHelper;
using System.IO;
using CrowdSource.Models.CoreModels;
using Newtonsoft.Json;
using CrowdSource.Services;

namespace CrowdSource.Tools.Commands
{
    public class ImportEnglishTextCommand : ICommand
    {
        public async Task RunAsync(IWebHost host, string[] args)
        {
            Console.WriteLine("Load English Text and Groups [Collection 1]");

            if (args.Count() != 1)
            {
                Console.WriteLine("Invalid Arguments");
                return ;
            }
            TextReader file;
            try
            {
                file = File.OpenText(args[0]);
            } catch(Exception)
            {
                Console.WriteLine($"Error opening file: {args[0]}.");
                return;
            }

            var csv = new CsvReader(file);
            
            var services = (IServiceScopeFactory)host.Services.GetService(typeof(IServiceScopeFactory));
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var firstCollection = context.Collections.First();
                var englishType = context.FieldTypes.Single(t => t.Name == "TextEnglish" && t.Collection.CollectionId == firstCollection.CollectionId);
                int i = 0;
                if (context.Groups.Any())
                {
                    Console.WriteLine("Rows existed in Groups. Exiting...");
                    return;
                }
                while (csv.Read())
                {
                    var now = DateTime.UtcNow;
                    ++i;
                    var filename = csv.GetField<string>("FileName");
                    var eng = csv.GetField<string>("Text");
                    Console.WriteLine($"[{i}] {filename} - {eng}");
                    //if (i == 100) break;
                    var newGroup = new Group()
                    {
                        Collection = firstCollection,
                        GroupMetadata = JsonConvert.SerializeObject(new Dictionary<string, string>
                        {
                            {"ImgFileName", filename }
                        }),
                        GroupId = i,
                    };
                    context.Groups.Add(newGroup);
                    var newVersion = new GroupVersion()
                    {
                        Created = now,
                        Group = newGroup,
                    };
                    context.GroupVersions.Add(newVersion);
                    var newSuggestion = new Suggestion()
                    {
                        Content = eng,
                        Created = now
                    };
                    context.Suggestions.Add(newSuggestion);
                    var newGVSuggestion = new GroupVersionRefersSuggestion()
                    {
                        GroupVersion = newVersion,
                        FieldType = englishType,
                        Suggestion = newSuggestion
                    };
                    context.GVSuggestions.Add(newGVSuggestion);

                }

                context.SaveChanges();
                Console.WriteLine($"{i} rows imported.");
            }// using scope
            
        } // method
    }
}
