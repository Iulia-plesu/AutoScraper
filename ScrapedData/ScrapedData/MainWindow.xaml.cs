using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace ScrapedData
{
    public partial class MainWindow : Window
    {
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

        private void DisplayResults(string result)
        {
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<dynamic>(result);
                if (jsonResult?.headlines != null)
                {
                    foreach (var title in jsonResult.headlines)
                    {
                        lstResults.Items.Add(title.ToString());
                    }
                    return;
                }
            }
            catch
            {
            }

            var titles = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var title in titles)
            {
                if (!string.IsNullOrWhiteSpace(title))
                {
                    lstResults.Items.Add(title.Trim());
                }
            }
        }
    }
}