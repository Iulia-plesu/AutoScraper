from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from webdriver_manager.chrome import ChromeDriverManager
import time

url = "https://www.thetimes.com/world"

def scrape_data():
    options = webdriver.ChromeOptions()
    options.add_argument('--disable-blink-features=AutomationControlled')
    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=options)

    try:
        driver.get(url)
        time.sleep(3)

        for _ in range(3):
            driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
            time.sleep(2)

        selectors = [
            "h3",
            "h2",
            "[data-testid*='headline']",
            ".js_article-headline",
            "article h3",
            "a[class*='headline']"
        ]

        titles = []
        for selector in selectors:
            try:
                elements = driver.find_elements(By.CSS_SELECTOR, selector)
                for element in elements:
                    title = element.text.strip()
                    if title and len(title) > 10:
                        titles.append(title)
                if titles:
                    break
            except:
                continue

        if not titles:
            headings = driver.find_elements(By.XPATH, "//h1 | //h2 | //h3 | //h4")
            titles = [h.text.strip() for h in headings if h.text.strip()]

        seen = set()
        unique_titles = []
        for title in titles:
            if title not in seen:
                seen.add(title)
                unique_titles.append(title)

        return unique_titles

    except Exception as e:
        print(f"Error during scraping: {str(e)}", file=sys.stderr)
        return []
    finally:
        driver.quit()

if __name__ == "__main__":
    titles = scrape_data()

    for title in titles:
        print(title)