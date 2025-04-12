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

            string pythonScriptPath = Path.Combine(currentDirectory, @"..\..\..\..\..\WebScraping\dist\Main.exe");  // Path to the generated .exe

            try
            {
                var result = await RunPythonScriptAsync(pythonScriptPath);  // Call the .exe instead of python
                Articles = ParseArticles(result);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it or show error)
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
                CreateNoWindow = false  // Allow the window to remain visible
            };

            using var process = new Process { StartInfo = start };
            process.Start();

            // Asynchronously read the output and error
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            // Wait for the process to finish and get the output and error
            await Task.WhenAll(outputTask, errorTask);

            // Ensure we wait for the process to exit
            process.WaitForExit();

            string output = outputTask.Result;
            string error = errorTask.Result;

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Python error: " + error);  // Log any Python errors
                throw new Exception($"Python error: {error}");
            }

            // Log the Python output (for debugging)
            Console.WriteLine("Python output: " + output);

            // Add this line to keep the window open after the program ends for debugging
            Console.WriteLine("Press Enter to close...");
            Console.ReadLine();  // This keeps the console open until you press Enter

            return output;
        }

        private List<Article> ParseArticles(string result)
        {
            var articles = new List<Article>();

            try
            {
                // Log the raw JSON result for debugging purposes
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
