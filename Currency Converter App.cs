using System;
using System.Globalization;

namespace ByDenisRafi
{
    class Program
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Title = "Currency Converter App";
            string Currency1 = "EUR";
            string Currency2 = "USD";
            int amount = 1;
            string[] availableCurrency = CurrencyConverter.GetCurrencyTags();
            Console.WriteLine("Currencies");
            Console.WriteLine(string.Join(",", availableCurrency));
            Console.WriteLine("\n");

            Console.WriteLine("Convert From...");
            Currency1 = Console.ReadLine();
            Console.WriteLine("\n");

            Console.WriteLine("Convert To...");
            Currency2 = Console.ReadLine();
            Console.WriteLine("\n");

            float exchangeRate = CurrencyConverter.GetExchangeRate(Currency1, Currency2, amount);
            Console.WriteLine("From " + amount + " " + Currency1.ToUpper()
                + " To " + Currency2.ToUpper() + " = " + exchangeRate);
            Console.ReadLine();
        }
        class CurrencyConverter
        {
            public static string[] GetCurrencyTags()
            {
                return new string[] {"EUR", "USD", "JPY", "BGN", "CZK", "DKK", "GBP", "HUF", "ITL", "LVL"
            , "PLN", "RON", "SEK", "CHF", "NOK", "HRK", "RUB", "TRY", "AUD", "BRL", "CAD", "CNY", "HKD", "IDR", "ILS"
            , "INR", "KRW", "MXN", "MYR", "NZD", "PHP", "SGD", "ZAR"};
            }
            public static float GetCurrencyRateInEuro(string currency)
            {
                if (currency.ToLower() == "")
                    throw new ArgumentException("Invalid Argument!");
                if (currency.ToLower() == "eur")
                    throw new ArgumentException("Invalid Argument!");
                try
                {
                    string rssUrl = string.Concat("http://www.ecb.int/rss/fxref-", currency.ToLower() + ".html");

                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.Load(rssUrl);
                    System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("rdf", "http://purl.org/rss/1.0/");
                    nsmgr.AddNamespace("cb", "http://www.cbwiki.net/wiki/index.php/Specification_1.1");

                    System.Xml.XmlNodeList nodeList = doc.SelectNodes("//rdf:item", nsmgr);
                    foreach (System.Xml.XmlNode node in nodeList)
                    {
                        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                        ci.NumberFormat.CurrencyDecimalSeparator = ".";
                        try
                        {
                            float exchangeRate = float.Parse(
                                node.SelectSingleNode("//cb:statistics//cb:exchangeRate//cb:value", nsmgr).InnerText,
                                NumberStyles.Any,
                                ci);
                            return exchangeRate;
                        }
                        catch {}
                    }
                    return 0;
                }
                catch
                {
                    return 0;
                }
            }
            public static float GetExchangeRate(string from, string to, float amount = 1)
            {
                if (from == null || to == null)
                    return 0;
                if (from.ToLower() == "EUR" && to.ToLower() == "EUR")
                    return amount;
                try
                {
                    float toRate = GetCurrencyRateInEuro(to);
                    float fromRate = GetCurrencyRateInEuro(from);
                    if (from.ToLower() == "EUR")
                    {
                        return (amount * toRate);
                    }
                    else if (to.ToLower() == "EUR")
                    {
                        return (amount / fromRate);
                    }
                    else
                    {
                        return (amount * toRate) / fromRate;
                    }
                }
                catch { return 0; }
            }
        }
    }
}