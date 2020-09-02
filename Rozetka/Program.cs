using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace Test_case_1
{
    class Program
    {
        static void Main(string[] args)
        {
            var MAX_PRICE_CELL = 20000;
            //A Сhrome driver was created,was set the full screen on browser, following the link to the site
            IWebDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(@"https://rozetka.com.ua");
            
            //In the search bar, enter "смартфоны" and click on the search button
            IWebElement search_bar = driver.FindElement(By.CssSelector("body > app-root > div > div:nth-child(2) > app-rz-header > header > div > div.header-bottomline > div.header-search.js-app-search-suggest > form > div > div > input"));
            search_bar.SendKeys("смартфоны");
            IWebElement search_button = driver.FindElement(By.CssSelector("body > app-root > div > div:nth-child(2) > app-rz-header > header > div > div.header-bottomline > div.header-search.js-app-search-suggest > form > button"));
            search_button.Click();

            //Wait load page  
            new WebDriverWait(driver, new TimeSpan(0, 0, 15)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            //Applying filters
            //Find and click on Apple checkbox
            IWebElement apple_filter = driver.FindElement(By.XPath("/html/body/app-root/div/div[1]/rz-category/div/main/rz-catalog/div/div[2]/aside/rz-filter-stack/div[3]/div/div/div/div[1]/div/rz-filter-section-checkbox/ul[1]/li[1]/a/label"));
            apple_filter.Click();
            
            //Find and click on Samsung checkbox 
            var wait_apple_filter = new WebDriverWait(driver, TimeSpan.FromMilliseconds(3000));
            var samsung_filter = wait_apple_filter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/app-root/div/div[1]/rz-category/div/main/rz-catalog/div/div[2]/aside/rz-filter-stack/div[3]/div/div/div/div[1]/div/rz-filter-section-checkbox/ul[1]/li[9]/a/label")));
            samsung_filter.Click();
           
            //Find and click on 128 Gb checkbox 
            var wait_samsung_filter = new WebDriverWait(driver, TimeSpan.FromMilliseconds(4000));
            var memory_filters = wait_samsung_filter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/app-root/div/div[1]/rz-category/div/main/rz-catalog/div/div[2]/aside/rz-filter-stack/div[13]/div/div/div/div/div/rz-filter-checkbox/ul[1]/li[1]/a/label")));
            memory_filters.Click();
            
            //Аind maximum of the price field, then clear it, enter the maximum cost and click on the "OK" button
            var wait_memory_filters = new WebDriverWait(driver, TimeSpan.FromMilliseconds(5000));
            var upper_price_bounderi = wait_memory_filters.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("/html/body/app-root/div/div[1]/rz-category/div/main/rz-catalog/div/div[2]/aside/rz-filter-stack/div[4]/div/div/div/div/div/rz-filter-slider/form/fieldset/div/input[2]")));
            upper_price_bounderi.Clear();
            upper_price_bounderi.SendKeys("20000");
            IWebElement OK_button = driver.FindElement(By.XPath("/html/body/app-root/div/div[1]/rz-category/div/main/rz-catalog/div/div[2]/aside/rz-filter-stack/div[4]/div/div/div/div/div/rz-filter-slider/form/fieldset/div/button"));
            OK_button.Click();
            
            //Outputting data to the console
            var wait_applying_filters = new WebDriverWait(driver, TimeSpan.FromMilliseconds(6000));
            var tel_list1 = wait_applying_filters.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/app-root/div/div[1]/rz-category/div/main/rz-catalog/div/div[2]/section/rz-grid/ul/li")));
            var tel_list = driver.FindElements(By.XPath("/html/body/app-root/div/div[1]/rz-category/div/main/rz-catalog/div/div[2]/section/rz-grid/ul/li"));
            var result = new List<Telepfone>();
            
            //In the loop we get the value of manufacturer, model, price and output the data to the console in the form of the table “manufacturer> model> price”.
            foreach (var tel in tel_list)
            {
                var price = tel.FindElement(By.CssSelector("span.goods-tile__price-value")).Text;
                price = Regex.Replace(price, @"\s+", "");
                var title = tel.FindElement(By.CssSelector("span.goods-tile__title")).Text;
                var words = title.Trim().Split(' ');
                var company = words[2];
                var model = string.Join(" ", words.Skip(3));
                Console.WriteLine($"{company} - {model} - {price}");
                result.Add(new Telepfone { Company = company, Model = model, Price = int.Parse(price) });
            }
            //Check that the result obtained has no prices exceeding UAH 20,000.
            var wrong = new List<Telepfone>();
            foreach (var tel in result) {
                if (tel.Price > MAX_PRICE_CELL)
                {
                    wrong.Add(tel);
                }
            }

            //Save the received data to a file
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\gyzvi\OneDrive\Рабочий стол\Test\WriteLines2.txt"))
            { 
                file.WriteLine(DateTime.UtcNow);
                if (wrong.Any()) {
                    file.WriteLine("Test Failed: After filter find cell price larger than 20000 UAH");
                }
                //Sort by price
                foreach (var ElementTest in result.OrderBy(x => x.Price)) {
                    file.WriteLine($"{ElementTest.Company} - {ElementTest.Model} - {ElementTest.Price}");
                }
            }
        }
    }
}

