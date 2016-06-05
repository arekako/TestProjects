using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5
{
    class NowyProgramFirmy
    {
        public static void Zapisz(List<string> elementy, string nazwapliku)
        {
            DirectoryInfo mojFolder = new DirectoryInfo("PlikiTekstowe");
            try
            {
                if (!mojFolder.Exists)
                {
                    mojFolder.Create();
                    string sciezka = "PlikiTekstowe";
                    sciezka += "\\";
                    sciezka += nazwapliku;
                    if (!File.Exists(sciezka))
                    {
                        string[] lines = new string[elementy.Count()];
                        for (int i = 0; i < elementy.Count(); i++)
                        {
                            lines[i] = elementy[i];
                        }
                        StreamWriter sw = File.CreateText(sciezka);
                        Console.WriteLine("Utworzyłem ścieżkę");
                        for (int i = 0; i < elementy.Count(); i++)
                        {
                            sw.WriteLine(lines[i]);
                        }
                        sw.Close();
                        Console.WriteLine("Plik zapisany!!!!");
                    }
                }
                else
                {
                    /////zastępowanie////////
                    string[] lines = new string[elementy.Count()];
                    string sciezka = "PlikiTekstowe";
                    sciezka += "\\";
                    sciezka += nazwapliku;
                    for (int i = 0; i < elementy.Count(); i++)
                    {
                        lines[i] = elementy[i];
                    }
                    StreamWriter sw = File.CreateText(sciezka);
                    for (int i = 0; i < elementy.Count(); i++)
                    {
                        sw.WriteLine(lines[i]);
                    }
                    sw.Close();
                    Console.WriteLine("Plik zmieniony i zapisany!");
                }
            }
            catch
            {
                Console.WriteLine("Bład z plikiem");
            }
        }
        static void Main(string[] args)
        {
            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();

                List<string> strony = new List<string>();
                strony.Add("http://panoramafirm.pl/szukaj?k=Lekarz&l=Ko%C5%9Bcierzyna");
                strony.Add("http://panoramafirm.pl/szukaj?k=Lekarz&l=Warszawa");
                strony.Add("http://panoramafirm.pl/szukaj?k=Lekarz&l=Pozna%C5%84%22");
                strony.Add("http://panoramafirm.pl/szukaj?k=Lekarz&l=W%C4%85glikowice%22");
                strony.Add("http://panoramafirm.pl/lekarz/pomorskie,ko%C5%9Bcierski,ko%C5%9Bcierzyna?a=1");
                HtmlDocument htmlDocument = htmlWeb.Load(strony[0]);

                ////////////////////////////names/////////////////////////////////////////////
                var names = from node in htmlDocument.DocumentNode.Descendants("a")
                            where node.Attributes.Contains("href") &&
                                  node.Attributes.Contains("title") &&
                                  node.Attributes.Contains("class") &&
                                  node.ParentNode.Name == "h2" &&
                                  node.ParentNode.ParentNode.Name == "div" &&
                                  node.ParentNode.ParentNode.ParentNode.Name == "li" &&
                                  node.PreviousSibling == null &&
                                  node.ParentNode.ParentNode.ParentNode.Elements("div").Count() < 8//to wyrzuca wszystkie te co nie potrzebne
                                  //znajduje boota?puste linijki
                            select new
                            {
                                a = node.InnerText,
                                b = node.ParentNode.ParentNode.ParentNode.Attributes["cnt"].Value,
                                c = node.ParentNode.ParentNode.ParentNode.Elements("div").Count()
                            };
                Console.WriteLine("names ma {0} elementow", names.Count());
                foreach (var el in names)
                {
                    Console.WriteLine("{0},{1},{2}", el.a, el.b,el.c);
                }
                Console.WriteLine("Koniec names");
                Console.ReadLine();
                /////////////////////////koniec names.////////////////////////

                ////////////////////////////phones/////////////////////////////////////////////
                var phones = from node in htmlDocument.DocumentNode.Descendants("strong")
                             where node.Attributes.Contains("class") &&
                                   node.ParentNode.Name == "a" &&
                                   node.ParentNode.ParentNode.Name == "div"&&
                                   node.ParentNode.ParentNode.ParentNode.Elements("div").Count() < 8//to wyrzuca wszystkie te co nie potrzebne
                             select new
                             {
                                 telefon = node.InnerText,
                                 numer = node.ParentNode.ParentNode.ParentNode.Attributes["cnt"].Value
                             };
                Console.WriteLine("phones ma {0} elementow", phones.Count());
                foreach (var el in phones)
                {
                    Console.WriteLine("{0},{1}", el.telefon, el.numer);
                    //telefony.Add(el.InnerText.ToString());
                }
                Console.WriteLine("Koniec phones");
                ////////////////////////koniec phones//////////////////////////////////////////

                ////////////////////////////wwww/////////////////////////////////////////////
                var wwww = from node in htmlDocument.DocumentNode.Descendants("a")
                           where
                           node.Attributes.Contains("href") &&
                           node.Attributes.Contains("title") &&
                           node.Attributes.Contains("class") &&
                           node.Attributes.Contains("rel") &&
                           node.Attributes.Contains("target") &&
                           node.ParentNode.ParentNode.ParentNode.Elements("div").Count() < 8//to wyrzuca wszystkie te co nie potrzebne
                           select new
                           {
                               link=node.InnerText,
                               numer = node.ParentNode.ParentNode.ParentNode.Attributes["cnt"].Value
                           };
                Console.WriteLine("wwww ma {0} elementow", wwww.Count());
                foreach (var el in wwww)
                {
                    Console.WriteLine("{0},{1}", el.link,el.numer);
                }
                Console.WriteLine("Koniec wwww");
                ////////////////////////koniec wwww//////////////////////////////////////////
                ////////////////////////////adres/////////////////////////////////////////////
                var adresy = from node in htmlDocument.DocumentNode.Descendants("div")
                           where
                           node.Attributes.Contains("class") &&
                           node.Attributes["class"].Value.Contains("contacts") &&
                           node.ParentNode.Name=="li" &&
                           (node.Attributes["class"].Value.Contains("contacts cross"))==false//takie ktore nie mają obszaru działania
                           select new
                           {
                               adres = node.InnerText,
                               numer = node.ParentNode.Attributes["cnt"].Value
                           };
                Console.WriteLine("adresy ma {0} elementow", adresy.Count());
                foreach (var el in adresy)
                {
                    Console.WriteLine("{0},{1}", el.adres,el.numer);
                }
                Console.WriteLine("Koniec adresy");
                ////////////////////////koniec adresy//////////////////////////////////////////
                //*[@id="serpContent"]/article/ul/li[5]/div[4]/text()
                ////cale dane od nowa
                //Console.WriteLine("Poczatek dane");
                //List<HtmlNode> nodes = new List<HtmlNode>();

                //var dane = from node in htmlDocument.DocumentNode.Descendants("li")
                //           where
                //           node.Attributes.Contains("cnt") &&
                //           node.Attributes.Contains("class") &&
                //           node.ParentNode.Name == "ul"
                //           select new
                //           {
                //               a = node.InnerHtml,//outer
                //               b = node.Attributes[0].Value,
                //               c = node.Attributes[1].Value,//to co jest w cnt node.Attributes[1].Value

                //               d = node.ChildNodes["div"].Attributes["class"].Value,
                //               //d = node.ChildNodes["div"].Attributes.Contains("class").ToString(),//true
                //               e = node.SelectSingleNode("//strong[@class='noLP']")

                //           };

               
                //Console.WriteLine("Koniec dane");
                //Console.WriteLine("dane ma {0} elementow", dane.Count());
                //Console.WriteLine("Koniec dane");
                ////Console.WriteLine("{0}", dane.First().c);//c
                //Console.WriteLine("dane ma {0} elementow", dane.Count());
                /////////////////////////////adres///////////////////////////////////////////

                /*
                Console.WriteLine("Noweeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
              
                ////IEnumerable<HtmlNode> lion2 = htmlDocument.DocumentNode.SelectNodes("//div[@class='title']");//tytuł
                ////IEnumerable<HtmlNode> lion2 = htmlDocument.DocumentNode.SelectNodes("//strong[@class='noLP']");//numery telefonow
                ////IEnumerable<HtmlNode> lion2 = htmlDocument.DocumentNode.SelectNodes("//div[@class='contacts']");//adresy
                ////IEnumerable<HtmlNode> lion2 = htmlDocument.DocumentNode.SelectNodes("//div[@class='hidePhone contactsLink']");///www
                //IEnumerable<HtmlNode> lion2 = htmlDocument.DocumentNode.Descendants("div")
                //    .Where(d =>
                //           d.Attributes.Contains("class") &&
                //           d.Attributes["class"].Value.Contains("title")
                //           );
                //Console.WriteLine(name.ToString());

                //var lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]//article//ul//li//div//h2//a");//tytuł
                var lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]/article/ul/li[2]/div[2]/h2/a");//tytuł
                //*[@id="serpContent"]/article/ul/li[1]/div[3]/h2/a

                List<string> linki2 = new List<string>();
                try
                {
                    foreach (var el in lion2)
                    {
                        //Console.WriteLine("{0}", el.InnerText.ToString());
                        Console.WriteLine("{0}", el.InnerText);
                        linki2.Add(el.InnerText.ToString());
                    }
                }
                catch 
                {
                    Console.WriteLine("Błąd z tytułem");
                    //lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]/article/ul/li[2]/div[3]/h2/a[1]");
                    //linki2 = new List<string>();
                    //foreach (var el in lion2)
                    //{
                    //    Console.WriteLine("{0}", el.InnerText);
                    //    linki2.Add(el.InnerText.ToString());
                    //}

                    linki2.Add("Brak tytułu");
                }
                

                //var lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]/article/ul/li[1]/div[2]/div/a[1]");//www
                lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]/article/ul/li[1]/div[2]/div/a[1]");
                try
                {
                    foreach (var el in lion2)
                    {
                        //Console.WriteLine("{0}", el.InnerText.ToString());
                        Console.WriteLine("{0}", el.InnerText);
                        linki2.Add(el.InnerText.ToString());
                    }
                }
                catch
                {
                    Console.WriteLine("Błąd z www");
                    linki2.Add("Brak adresu www");
                }
                //var lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]/article/ul/li[1]/div[4]/a");//numer
                lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]/article/ul/li[1]/div[4]/a");
                try
                {
                    foreach (var el in lion2)
                    {
                        //Console.WriteLine("{0}", el.InnerText.ToString());
                        Console.WriteLine("{0}", el.InnerText);
                        linki2.Add(el.InnerText.ToString());
                    }
                }
                catch
                {
                    Console.WriteLine("Błąd z numerem");
                    linki2.Add("Brak numeru");
                }
                //var lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]/article/ul/li[1]/div[4]/text()");//adres
                //var name = lion2.Select(node => node.InnerText);
                lion2 = htmlDocument.DocumentNode.SelectNodes("//*[@id=\"serpContent\"]/article/ul/li[1]/div[4]/text()");
                try
                {
                    foreach (var el in lion2)
                    {
                        //Console.WriteLine("{0}", el.InnerText.ToString());
                        Console.WriteLine("{0}", el.InnerText);
                        linki2.Add(el.InnerText.ToString());
                    }
                }
                catch
                {
                    Console.WriteLine("Błąd z adresem");
                    linki2.Add("Brak adresu");
                }
                Console.WriteLine(linki2.Count());
                Console.WriteLine("Pobrałem");
                foreach (var el in linki2)
                {
                    Console.WriteLine("{0}", el.ToString());
                }
                //Console.WriteLine(linki2[1]);
                Console.WriteLine("Pobrałem");
                //Zapisz(tytuly, "tytuły-auta.txt");
                //////IEnumerable<HtmlNode> lion3 = htmlDocument.DocumentNode.Descendants("div")
                //////    .Where(d =>
                //////           d.Attributes.Contains("class") &&
                //////           d.Attributes["class"].Value.Contains("title")
                //////           );
                //////List<string> linki3 = new List<string>();
                //////foreach (var el in lion3)
                //////{
                //////    //Console.WriteLine("{0}", el.InnerText.ToString());
                //////    Console.WriteLine("{0}", el.InnerText);
                //////    linki3.Add(el.InnerText);
                //////}
                //////Console.WriteLine(linki3.Count());
                //////Console.WriteLine(linki3[0].Trim().Trim());
                //////Console.WriteLine(linki3[0].Trim().Trim().Split().Count());
                //////char[] znaki = new char[] {' '};
                //////Console.WriteLine(linki3[0].Trim(new Char[] {' '}));
                 * */
            }
            catch
            {
                Console.WriteLine("Brak internetu.Sprawdź połączenie z internetem");
            }
            Console.ReadLine();
        }


    }
}
