from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from webdriver_manager.chrome import ChromeDriverManager
import json

url = "https://www.thetimes.com/"

def scrape_data():

    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()))
    driver.get(url)

    try:
        wait = WebDriverWait(driver, 10)
        titles_element = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "article-title")))

        titles = [title.text for title in titles_element]

    except Exception as e:
        print(f"Error: {e}")
        titles = []

    driver.quit()

    return json.dumps(titles)

if __name__ == "__main__":
    print(scrape_data())
