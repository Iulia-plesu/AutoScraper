from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from webdriver_manager.chrome import ChromeDriverManager
import json
import time

url = "https://www.thetimes.com/"

def scrape_data():

    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()))
    driver.get(url)

    time.sleep(5)

    titles = driver.find_elements(By.CLASS_NAME, 'article-title')

    title_texts = [title.text for title in titles]

    driver.quit()

    return json.dumps(title_texts)

if __name__ == "__main__":
    print(scrape_data())
