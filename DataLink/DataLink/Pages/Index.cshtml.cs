using Microsoft.AspNetCore.Mvc.RazorPages;
using DataLink.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace DataLink.Pages
{
    public class IndexModel : PageModel
    {
        public List<Article> Articles { get; set; } = new();

        public async Task OnGetAsync()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            string pythonScriptPath = Path.Combine(currentDirectory, @"..\..\..\AutoScraper\WebScraping\dist\Main.exe");  

            try
            {
                var result = await RunPythonScriptAsync(pythonScriptPath);  
                Articles = ParseArticles(result);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<string> RunPythonScriptAsync(string exePath)
        {
            var start = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = false  
            };

            using var process = new Process { StartInfo = start };
            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);

            process.WaitForExit();

            string output = outputTask.Result;
            string error = errorTask.Result;

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Python error: " + error); 
                throw new Exception($"Python error: {error}");
            }

            Console.WriteLine("Python output: " + output);

            Console.WriteLine("Press Enter to close...");
            Console.ReadLine(); 

            return output;
        }

        private List<Article> ParseArticles(string result)
        {
            var articles = new List<Article>();

            try
            {
                Console.WriteLine("Raw JSON result: " + result);

                var jsonResult = JsonConvert.DeserializeObject<dynamic>(result);
                if (jsonResult?.headlines != null)
                {
                    foreach (var item in jsonResult.headlines)
                    {
                        string title = item.title?.ToString().Trim() ?? "";
                        string url = item.url?.ToString().Trim() ?? "";

                        if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(url))
                        {
                            articles.Add(new Article { Title = title, Url = url });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing JSON: " + ex.Message);
            }

            return articles;
        }
    }
}
