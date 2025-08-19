# AutoScraper

A web scraping solution that collects news articles from BBC News, categorizes them, and displays them through a clean web interface. The project combines Python for web scraping and C# ASP.NET Core for the web application with email digest functionality.

## Prerequisites

### Required Software
1. **Python Environment**:
   - Python 3.8 or later
   - pip (Python package installer)
   ```powershell
   # Check Python version
   python --version
   # Check pip version
   pip --version
   ```

2. **.NET Environment**:
   - .NET 8.0 SDK
   ```powershell
   # Check .NET version
   dotnet --version
   ```

3. **Chrome Browser**:
   - Google Chrome (latest version)
   - Chrome WebDriver (automatically managed)
   ```powershell
   # Chrome should be installed from https://www.google.com/chrome/
   ```

4. **Gmail Account**:
   - Account with 2-factor authentication enabled
   - App password generated

### Installing Prerequisites

1. **Install Python**:
   ```powershell
   # Download from https://www.python.org/downloads/
   # During installation:
   # ✓ Add Python to PATH
   # ✓ Install pip
   ```

2. **Install .NET SDK**:
   ```powershell
   # Download from https://dotnet.microsoft.com/download
   # Choose .NET 8.0 SDK
   ```

3. **Install Development Tools** (Optional):
   ```powershell
   # Install Visual Studio Code
   winget install Microsoft.VisualStudioCode

   # Install Git
   winget install Git.Git
   ```

## Step-by-Step Setup Guide

1. **Clone or Download the Repository**:
   ```powershell
   git clone <repository-url>
   cd AutoScraper
   ```

2. **Set Up Python Environment**:
   ```powershell
   # Create and activate a virtual environment (recommended)
   python -m venv venv
   .\venv\Scripts\activate

   # Install required packages
   pip install -r requirements.txt
   ```

3. **Configure Gmail Credentials**:
   - Go to your Google Account settings
   - Enable 2-Step Verification if not already enabled
   - Generate an App Password:
     1. Go to Security settings
     2. Select "App passwords"
     3. Generate a new app password
   - Update `appsettings.json` in the DataLink project with your credentials

4. **Build the Python Scraper**:
   ```powershell
   # Navigate to WebScraping directory
   cd WebScraping

   # Build the executable
   pyinstaller --onefile Main.py
   ```
   The executable will be created in `dist/Main.exe`

5. **Build and Run DataLink Service**:
   ```powershell
   # Navigate to DataLink directory
   cd ../DataLink

   # Build the project
   dotnet build

   # Run the service
   dotnet run
   ```

6. **Testing the Setup**:
   - The DataLink service will start on `https://localhost:7227`
   - The scraper executable is in `WebScraping/dist/Main.exe`
   - Test both components to ensure proper functionality

## Configuration

### DataLink Service
The DataLink service configuration is stored in `appsettings.json`:
```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com"
  }
}
```

### WebScraper
The Python scraper can be configured by modifying the following settings in `Main.py`:
- News categories to scrape
- Update frequency
- Output format

## Troubleshooting

### Common Issues

1. **Python Environment Issues**:
   - If `pip` fails to install packages, try updating pip:
     ```powershell
     python -m pip install --upgrade pip
     ```
   - If you get SSL errors, ensure your Python installation includes SSL certificates

2. **Chrome WebDriver Issues**:
   - If the scraper fails to start Chrome, ensure Chrome is up to date
   - Clear your Chrome user profile if you encounter browser automation issues

3. **DataLink Service Issues**:
   - If the service fails to start, check if port 7227 is available
   - Verify your Gmail credentials in `appsettings.json`
   - Ensure your app password is correctly configured

4. **Build Issues**:
   - For Python build errors, ensure all dependencies are installed:
     ```powershell
     pip install -r requirements.txt
     ```
   - For .NET build errors, try cleaning the solution:
     ```powershell
     dotnet clean
     dotnet build
     ```

### Getting Help
If you encounter issues not covered here:
1. Check the error messages in the console
2. Review the application logs
3. Open an issue on the project repository with:
   - Detailed error description
   - Steps to reproduce
   - Environment information

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
