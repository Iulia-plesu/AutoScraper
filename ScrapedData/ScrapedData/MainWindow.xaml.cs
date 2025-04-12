using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Navigation;



namespace ScrapedData
{
    public partial class MainWindow : Window
    {
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnScrape_Click(object sender, RoutedEventArgs e)
        {
            btnScrape.IsEnabled = false;
            progressBar.IsIndeterminate = true;
            txtStatus.Text = "Scraping...";
            lstResults.Items.Clear();

            string pythonPath = @"C:\Users\Plesu\AppData\Local\Programs\Python\Python312\python.exe";
            string scriptPath = @"C:\Users\Plesu\Desktop\AutoScraper\WebScraping\Main.py";

            try
            {
                var result = await RunPythonScriptAsync(pythonPath, scriptPath);
                DisplayResults(result);
                txtStatus.Text = $"Done. Found {lstResults.Items.Count} items.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Error occurred";
            }
            finally
            {
                btnScrape.IsEnabled = true;
                progressBar.IsIndeterminate = false;
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

            using (var process = new Process { StartInfo = start })
            {
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                await Task.WhenAll(outputTask, errorTask);

                process.WaitForExit();

                if (!string.IsNullOrEmpty(errorTask.Result))
                {
                    throw new Exception($"Python error: {errorTask.Result}");
                }

                return outputTask.Result;
            }
        }

        public class Article
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }

        private void DisplayResults(string result)
        {
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<dynamic>(result);
                if (jsonResult?.headlines != null)
                {
                    foreach (var item in jsonResult.headlines)
                    {
                        string title = item.title != null ? item.title.ToString() : "";
                        string url = item.url != null ? item.url.ToString() : "";

                        // Only add if URL is not null/empty
                        if (!string.IsNullOrWhiteSpace(url))
                        {
                            lstResults.Items.Add(new Article
                            {
                                Title = title,
                                Url = url
                            });
                        }
                    }
                    return;
                }
            }
            catch
            {
                // Optional: log error or show debug info
            }

            // Fallback mode — skip displaying anything if no URLs
            var titles = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var title in titles)
            {
                // If no URL fallback is wanted, comment this section
                // lstResults.Items.Add(new Article { Title = title.Trim(), Url = "" });
            }
        }

    }
}