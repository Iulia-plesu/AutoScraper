# AutoScraper

A web scraping solution that collects news articles from BBC News, categorizes them, and displays them through a clean web interface. The project combines Python for web scraping and C# ASP.NET Core for the web application with email digest functionality.

## Prerequisites

- Python 3.8 or later
- .NET 8.0 SDK
- Chrome browser
- Gmail account with 2-factor authentication enabled

## Step-by-Step Setup Guide

### 1. Initial Setup

```powershell
# Clone the repository (if not already done)
git clone https://github.com/Iulia-plesu/AutoScraper.git
cd AutoScraper
```

### 2. Python Web Scraper Setup

```powershell
# Navigate to WebScraping directory
cd WebScraping

# Create Python virtual environment
python -m venv .venv

# Activate virtual environment
.venv\Scripts\activate

# Install required packages
pip install selenium
pip install webdriver_manager
pip install pyinstaller
pip install newtonsoft-json

# Build the executable
pyinstaller --onefile Main.py

# Verify the executable was created
Test-Path dist\Main.exe
```

### 3. C# Web Application Setup

```powershell
# Navigate to DataLink project
cd ..\DataLink\DataLink

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Create development certificate
dotnet dev-certs https --trust
```

### 4. Email Configuration

1. Open `/DataLink/DataLink/appsettings.json`
2. Update the SmtpSettings section with your email credentials:
   ```json
   "SmtpSettings": {
     "FromEmail": "your-email@example.com",
     "ToEmail": "recipient@example.com",
     "SmtpServer": "smtp.gmail.com",
     "Port": 587,
     "Username": "your-email@gmail.com",
     "Password": "your-app-specific-password"
   }
   ```
   For Gmail, you'll need to:
   1. Enable 2-factor authentication
   2. Generate an App Password
   3. Use the App Password in the configuration

## Running the Application

### First Run (Testing Each Component)

1. Test the Web Scraper:
   ```powershell
   # Navigate to WebScraping directory
   cd WebScraping\dist
   
   # Run the scraper
   .\Main.exe
   
   # Check if JSON output is generated correctly
   ```

2. Test the Web Application:
   ```powershell
   # Navigate to DataLink project
   cd ..\..\DataLink\DataLink
   
   # Run the application
   dotnet run --launch-profile "http"
   
   # Application will be available at http://localhost:5000
   ```

### Regular Usage

1. Start the Web Application:
   ```powershell
   cd DataLink\DataLink
   dotnet run --launch-profile "http"
   ```

2. Open your browser:
   - Go to http://localhost:5000
   - The application will automatically:
     1. Run the web scraper
     2. Parse the articles
     3. Send email digest
     4. Display articles on the page

## Troubleshooting

1. If the scraper fails:
   ```powershell
   # Check Chrome version
   # Ensure Chrome is installed and up to date
   # Try running with visible browser
   cd WebScraping
   python Main.py
   ```

2. If the web application fails:
   ```powershell
   # Clean and rebuild
   cd DataLink\DataLink
   dotnet clean
   dotnet build
   dotnet run --launch-profile "http" --verbosity detailed
   ```

3. If email sending fails:
   - Verify app password is correct
   - Check email settings in appsettings.json
   - Ensure 2FA is enabled on Gmail account

## Project Structure

```
AutoScraper/
├── WebScraping/
│   ├── Main.py              # Scraping logic
│   ├── dist/
│   │   └── Main.exe         # Built executable
│   └── .venv/               # Python virtual environment
├── DataLink/
│   └── DataLink/
│       ├── Program.cs       # Application entry
│       ├── Services/        # Email service
│       ├── Models/          # Data models
│       ├── Pages/          # Web pages
│       └── appsettings.json # Configuration
└── README.md
```

## Development Notes

- Web scraper runs in headless mode by default
- Email digests are sent automatically when articles are scraped
- JSON format is consistent between Python and C# components
- All components include comprehensive error handling
