import sys
import time

from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from webdriver_manager.chrome import ChromeDriverManager

url = "https://www.bbc.com/news"

def scrape_data():
    options = webdriver.ChromeOptions()
    options.add_argument('--disable-blink-features=AutomationControlled')
    options.add_argument('--headless')  # Optional: comment this out if you want to see the browser
    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=options)

    try:
        driver.get(url)

        wait = WebDriverWait(driver, 10)

        # Scroll to load more articles
        for _ in range(3):
            driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
            time.sleep(2)

        # Wait for at least one article to show up
        wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, "article")))

        # Take screenshot to debug what’s visible
        driver.save_screenshot("debug_screenshot.png")

        # Try to extract titles from article tags
        titles = []
        articles = driver.find_elements(By.CSS_SELECTOR, "article")

        for article in articles:
            possible_titles = article.find_elements(By.CSS_SELECTOR, "h2, h3, a")
            for pt in possible_titles:
                text = pt.text.strip()
                if text and len(text) > 10 and text not in titles:
                    titles.append(text)

        return titles

    except Exception as e:
        print(f"Error during scraping: {str(e)}", file=sys.stderr)
        return []
    finally:
        driver.quit()

if __name__ == "__main__":
    titles = scrape_data()

    for title in titles:
        print(title)
