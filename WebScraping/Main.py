import sys
import time
import json
from urllib.parse import urljoin

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
    options.add_argument('--headless')  # comment this line if you want to see the browser
    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=options)

    try:
        driver.get(url)

        wait = WebDriverWait(driver, 10)

        for _ in range(3):
            driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
            time.sleep(2)

        wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, "article")))

        articles = driver.find_elements(By.CSS_SELECTOR, "article")
        results = []
        seen_titles = set()

        for article in articles:
            possible_links = article.find_elements(By.CSS_SELECTOR, "a")

            for link in possible_links:
                text = link.text.strip()
                href = link.get_attribute("href")

                if text and len(text) > 10 and href:
                    full_url = urljoin(url, href)
                    if text not in seen_titles:
                        results.append({
                            "title": text,
                            "url": full_url
                        })
                        seen_titles.add(text)

        return results

    except Exception as e:
        print(json.dumps({"error": str(e)}))
        return []
    finally:
        driver.quit()

if __name__ == "__main__":
    headlines = scrape_data()

    output = {
        "headlines": headlines
    }

    print(json.dumps(output, ensure_ascii=False))
