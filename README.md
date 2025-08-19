# BBC News Scraper

A web application that scrapes and displays the latest news from BBC News website.

## Structure

- `WebScraping/` - Python scraper using Selenium
  - `Main.py` - Main scraping script
  - `dist/Main.exe` - Compiled executable

- `DataLink/` - ASP.NET Core web application
  - Displays scraped articles in a clean, newspaper-style layout
  - Automatically categorizes articles
  - Handles article metadata (timestamps, tags)

## Setup

1. Requirements:
   - .NET 8.0 SDK
   - Chrome browser (for the scraper)
   - Python 3.x (only for development)

2. Installation:
   ```bash
   # Clone the repository
   git clone https://github.com/Iulia-plesu/AutoScraper.git
   cd AutoScraper

   # Run the web application
   cd DataLink/DataLink
   dotnet run
   ```

3. Access the application at `http://localhost:5000`

## Features

- Clean, responsive newspaper-style layout
- Article categorization
- Tag support for video/audio content
- Timestamp display
- Automatic duplicate removal
- Link to original articles
