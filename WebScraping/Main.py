from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from webdriver_manager.chrome import ChromeDriverManager
import json
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
        unique_titles = [x for x in titles if not (x in seen or seen.add(x))]

        print(f"\nFound {len(unique_titles)} unique titles:")
        for i, title in enumerate(unique_titles[:20], 1):
            print(f"{i}. {title}")

        return json.dumps({"titles": unique_titles}, indent=2)

    except Exception as e:
        print(f"Error during scraping: {str(e)}")
        return json.dumps({"error": str(e)})
    finally:
        driver.quit()


if __name__ == "__main__":
    result = scrape_data()
    print("\nFinal Output:")
    print(result)