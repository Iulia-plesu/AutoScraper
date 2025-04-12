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

            string pythonPath = Path.Combine(currentDirectory, @"..\..\..\..\..\Python\Python312\python.exe");
            string scriptPath = Path.Combine(currentDirectory, @"..\..\..\..\..\WebScraping\Main.py");

            try
            {
                var result = await RunPythonScriptAsync(pythonPath, scriptPath);
                Articles = ParseArticles(result);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it or show error)
            }
        }

        private async Task<string> RunPythonScriptAsync(string pythonPath, string scriptPath)
        {
            var start = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = start };
            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);

            if (!string.IsNullOrEmpty(errorTask.Result))
                throw new Exception($"Python error: {errorTask.Result}");

            return outputTask.Result;
        }

        private List<Article> ParseArticles(string result)
        {
            var articles = new List<Article>();

            try
            {
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
            catch
            {
                // Handle exception (e.g., log it or show error)
            }

            return articles;
        }
    }
}
