# AutoScraper

A web scraping solution that collects news articles from BBC News, categorizes them, and displays them through a clean web interface. The project combines Python for web scraping and C# ASP.NET Core for the web application.

## Components

1. **WebScraper (Python)**
   - Located in `/WebScraping/`
   - Uses Selenium for web scraping
   - Automatically categorizes articles
   - Outputs structured JSON data

2. **DataLink (C# ASP.NET Core)**
   - Located in `/DataLink/`
   - Web application to display scraped articles
   - Clean and responsive interface
   - Category-based article organization

## Prerequisites

- Python 3.8 or later
- .NET 8.0 SDK
- Chrome browser (for web scraping)

## Setup Instructions

### 1. WebScraper Setup

The Python scraper executable is pre-built and ready to use in `/WebScraping/dist/Main.exe`.

To rebuild the scraper (optional):
```bash
cd WebScraping
python -m venv .venv
.venv\Scripts\activate
pip install -r requirements.txt
pyinstaller --onefile Main.py
```

### 2. DataLink Setup

The web application is pre-built and ready to use in `/DataLink/DataLink/bin/Release/net8.0/DataLink.exe`.

To rebuild the web application (optional):
```bash
cd DataLink
dotnet build
dotnet publish -c Release
```

## Running the Application

1. **Start the Web Scraper**
   ```bash
   cd WebScraping/dist
   ./Main.exe
   ```
   This will scrape the latest articles and output JSON data.

2. **Start the Web Application**
   ```bash
   cd DataLink/DataLink/bin/Release/net8.0
   ./DataLink.exe
   ```
   The web interface will be available at `http://localhost:5000`

## Testing

1. **Test the Web Scraper**
   - Run the scraper: `./WebScraping/dist/Main.exe`
   - Check the console output for JSON data
   - Verify that articles are properly categorized

2. **Test the Web Application**
   - Start the application
   - Open `http://localhost:5000` in your browser
   - Check if articles are displayed correctly
   - Verify category navigation

## Project Structure

```
AutoScraper/
├── WebScraping/
│   ├── Main.py              # Scraping logic
│   ├── requirements.txt     # Python dependencies
│   └── dist/
│       └── Main.exe         # Built executable
├── DataLink/
│   ├── DataLink.sln
│   └── DataLink/
│       ├── Program.cs       # Application entry
│       ├── Models/          # Data models
│       └── Pages/           # Web pages
└── README.md
```

## Development Notes

- The scraper is configured to run in headless mode
- Web application uses minimal API style
- JSON data format matches between Python and C# models
- Error handling is implemented in both components
