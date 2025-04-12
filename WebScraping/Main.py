import sys
import time
import json
import re
from urllib.parse import urljoin

from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from webdriver_manager.chrome import ChromeDriverManager

url = "https://www.bbc.com/news"

def split_extras(text):
    """
    Clean the title text by separating time, category, and tags (e.g., "LIVE", "see more") from the main title.
    """
    tags = []
    clean_title = text.strip()

    # Detect and strip tags like "LIVE" or "See more"
    if "LIVE" in clean_title.upper():
        tags.append("LIVE")
        clean_title = re.sub(r'\bLIVE\b', '', clean_title, flags=re.IGNORECASE).strip()

    if "see more" in clean_title.lower():
        tags.append("See more")
        clean_title = re.sub(r'\bsee more\b', '', clean_title, flags=re.IGNORECASE).strip()

    # Detect time expressions like "12 hrs ago", "2h ago", "1 day ago"
    time_match = re.search(r'(\d{1,2}\s?(hrs?|h|min)s? ago|\d+\s?days? ago)', clean_title.lower())
    found_time = ""
    if time_match:
        found_time = time_match.group(0)
        clean_title = clean_title.replace(found_time, '').strip()

    # Detect category (usually found at the end of the title, after a newline or specific structure)
    category_match = re.search(r'(\n.*|\s\-\s.*)', clean_title)  # Match if there’s a hyphen or newline at the end
    found_category = ""
    if category_match:
        found_category = category_match.group(0).strip()
        clean_title = clean_title.replace(found_category, '').strip()

    return clean_title, found_time, tags, found_category

def scrape_data():
    options = webdriver.ChromeOptions()
    options.add_argument('--disable-blink-features=AutomationControlled')
    options.add_argument('--headless')
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
            heading_elements = article.find_elements(By.CSS_SELECTOR, "h3, h2")

            for heading in heading_elements:
                raw_text = heading.text.strip()
                if not raw_text or len(raw_text) < 10 or raw_text.isdigit():
                    continue

                clean_title, _, _, _ = split_extras(raw_text)

                if clean_title and clean_title not in seen_titles:
                    seen_titles.add(clean_title)

                    # Try to find a parent <a> tag or closest anchor
                    try:
                        link_element = heading.find_element(By.XPATH, ".//ancestor::a[1]")
                        href = link_element.get_attribute("href")
                        full_url = urljoin(url, href) if href else ""
                    except:
                        full_url = ""

                    results.append({
                        "title": clean_title,
                        "url": full_url
                    })

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

    # Print the results in a pretty JSON format
    print(json.dumps(output, ensure_ascii=False, indent=2))
