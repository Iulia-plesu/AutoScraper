# News Headlines Scraper 

This project is a **work-in-progress**.

**DataLink** is a .NET web application that scrapes the latest article titles from the [BBC News](https://www.bbc.com/news) homepage and displays them as a list of clickable links.

## 🔍 What It Does

- Automatically runs a Python-based scraper (compiled as an `.exe`) in the background.
- Retrieves headlines from `bbc.com/news`.
- Displays them in a clean list format with hyperlinks.
- Built with ASP.NET Razor Pages and Selenium for scraping.

## 🚀 How to Run

1. **Open the Project:**
   - Open `DataLink.sln` in **Visual Studio**.

2. **Build and Run in Release Mode:**
   - Make sure the solution configuration is set to `Release`.
   - Press `Ctrl + F5` (Run Without Debugging), or click the green "Start" button.
   - Visual Studio will launch the application and open the terminal.

3. **View in Browser:**
   - Go to:
     ```
     http://localhost:5000
     ```
   - You will see a list of the latest BBC News article titles, each linking to the full article.

## 🛠️ Technologies Used

- ASP.NET Core Razor Pages (.NET 8)
- C#
- Python 3 (compiled to `.exe` using `pyinstaller`)
- Selenium + Chrome WebDriver
- Newtonsoft.Json
- WebDriver Manager


## 📌 Notes

- This project is **still in development**.
- Scraping is done with headless Chrome and may need updates if BBC changes their structure.
- Error handling and UI improvements are planned.

## 📬 Feedback & Contributions

Suggestions, pull requests, or issues are welcome. Feel free to help improve this project!

---

