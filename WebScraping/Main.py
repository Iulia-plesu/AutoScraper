import sys
import time
import json
import re
from urllib.parse import urljoin
from dataclasses import dataclass, asdict, field
from typing import List, Optional, Dict

from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from webdriver_manager.chrome import ChromeDriverManager

url = "https://www.bbc.com/news"

@dataclass
class ScrapedData:
    categories: Dict[str, List['Article']] = field(default_factory=dict)
    other: List['Article'] = field(default_factory=list)

EXCLUDED_TERMS = {
    'home', 'privacy', 'sign up', 'datalink', 'about us', 'contact', 
    'news briefing', '© 2025', 'terms', 'cookies', 'advertisement'
}

@dataclass
class Article:
    title: str
    url: Optional[str] = None
    category: Optional[str] = None
    timestamp: Optional[str] = None
    tags: List[str] = None

    def __post_init__(self):
        if self.tags is None:
            self.tags = []

def clean_title(text: str) -> Article:
    """Clean and parse article title text into structured data."""
    tags = []
    clean_text = text.strip()

    # Check if this is a navigation or footer item
    if any(term.lower() in clean_text.lower() for term in EXCLUDED_TERMS):
        return None

    # Extract media type tags
    media_prefixes = {
        'watch:': 'Video',
        'listen:': 'Audio',
        'analysis:': 'Analysis'
    }
    
    for prefix, tag in media_prefixes.items():
        if clean_text.lower().startswith(prefix):
            tags.append(tag)
            clean_text = re.sub(f'^{prefix}\\s*', '', clean_text, flags=re.IGNORECASE).strip()

    # Extract LIVE tag
    if "LIVE" in clean_text.upper():
        tags.append("LIVE")
        clean_text = re.sub(r'\bLIVE\b', '', clean_text, flags=re.IGNORECASE).strip()

    # Extract time information
    time_match = re.search(r'(\d{1,2}\s?(hrs?|h|min)s? ago|\d+\s?days? ago)', clean_text.lower())
    timestamp = time_match.group(0) if time_match else None
    if timestamp:
        clean_text = clean_text.replace(timestamp, '').strip()

    # Clean up quotes and normalize spaces
    clean_text = re.sub(r'[\'"`]([^\'"`]+)[\'"`]', r'\1', clean_text)  # Remove quotes around text
    clean_text = re.sub(r'\s+', ' ', clean_text)  # Normalize spaces
    
    # Only extract categories for specific patterns that are clearly category markers
    category_patterns = [
        r'\s+-\s+(?:live|analysis|latest|updates)$',  # Must be at the end
        r'(?:^|\s)in\s+(?:pictures|detail)$',
        r':\s*(?:live|latest)$'
    ]
    
    category = None
    for pattern in category_patterns:
        category_match = re.search(pattern, clean_text, re.IGNORECASE)
        if category_match:
            category = clean_text[category_match.start():].strip('- :').strip()
            clean_text = clean_text[:category_match.start()].strip()
            break
    
    # Categorize by common news sections if no category found
    if not category:
        news_categories = {
            'World': ['world', 'international', 'global'],
            'Politics': ['politics', 'election', 'parliament', 'congress'],
            'Technology': ['tech', 'technology', 'cyber', 'AI', 'digital'],
            'Science': ['science', 'research', 'study', 'scientists'],
            'Entertainment': ['entertainment', 'film', 'movie', 'music', 'celebrity'],
            'Sports': ['sport', 'football', 'soccer', 'rugby', 'tennis']
        }
        
        for cat, keywords in news_categories.items():
            if any(keyword.lower() in clean_text.lower() for keyword in keywords):
                category = cat
                break

    # Skip if title is too short after cleaning or is likely a navigation item
    if len(clean_text) < 15 or clean_text.count(' ') < 2:
        return None

    return Article(
        title=clean_text,
        category=category,
        timestamp=timestamp,
        tags=tags
    )

def article_to_dict(obj):
    """Convert Article instances to dictionaries for JSON serialization"""
    if isinstance(obj, (Article, ScrapedData)):
        return {k: v for k, v in asdict(obj).items() if v is not None}
    raise TypeError(f'Object of type {obj.__class__.__name__} is not JSON serializable')

def scrape_data() -> ScrapedData:
    options = webdriver.ChromeOptions()
    options.add_argument('--disable-blink-features=AutomationControlled')
    options.add_argument('--headless')
    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=options)

    try:
        driver.get(url)
        wait = WebDriverWait(driver, 10)

        # Scroll to load more content
        for _ in range(3):
            driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
            time.sleep(2)

        # Wait for articles to load
        wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, "article")))

        # Exclude navigation and footer areas
        main_content = driver.find_element(By.TAG_NAME, "main")
        
        # First, collect all links to avoid duplicate titles
        links = main_content.find_elements(By.CSS_SELECTOR, "a[href*='/news/']")
        seen_urls = set()
        articles_list = []
        
        for link in links:
            try:
                article_url = link.get_attribute("href")
                # Skip if we've seen this URL or if it's not a proper article URL
                if not article_url or article_url in seen_urls or not '/news/' in article_url:
                    continue
                    
                # Get the complete title from the link
                title_text = link.text.strip()
                if not title_text or len(title_text) < 10:
                    continue
                    
                # Skip navigation and footer links
                if any(term.lower() in title_text.lower() for term in EXCLUDED_TERMS):
                    continue
                    
                seen_urls.add(article_url)
                article_data = clean_title(title_text)
                if article_data:
                    article_data.url = article_url
                    articles_list.append(article_data)
            except Exception as e:
                print(f"Error processing link: {str(e)}")
                continue

        # Organize articles by category
        result = ScrapedData()
        
        for article in articles_list:
            if article.category:
                if article.category not in result.categories:
                    result.categories[article.category] = []
                result.categories[article.category].append(article)
            else:
                result.other.append(article)

        return result
    finally:
        driver.quit()

if __name__ == "__main__":
    try:
        data = scrape_data()
        json_data = json.dumps(data, default=article_to_dict, indent=2)
        print(json_data)
    except Exception as e:
        print(json.dumps({"error": str(e)}, indent=2))
        sys.exit(1)
