using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;


namespace Ogloszenia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Task ProcessData(List<string> list, IProgress<ProgressReport> progress)
        //{
        //    int index = 1;
        //    int totalProcess = list.Count();
        //    var progressReport = new ProgressReport();
        //    return Task.Run(() =>
        //        {
        //            for (int i = 0; i < totalProcess; i++)
        //            {
        //                progressReport.PercenntComplete = index++ * 100 / totalProcess;
        //                progress.Report(progressReport);
        //                Thread.Sleep(10);
        //            }
        //        });
        //}
        public static void Zapisz(string element, string nazwapliku)
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
                        StreamWriter sw = File.CreateText(sciezka);
                        //Console.WriteLine("Utworzyłem ścieżkę");
                        sw.WriteLine(element);
                        sw.Close();
                        //Console.WriteLine("Plik zapisany!!!!");
                    }
                }
                else
                {
                    /////zastępowanie////////
                    string sciezka = "PlikiTekstowe";
                    sciezka += "\\";
                    sciezka += nazwapliku;
                    StreamWriter sw = File.CreateText(sciezka);
                    sw.WriteLine(element);
                    sw.Close();
                    //Console.WriteLine("Plik zmieniony i zapisany!");
                }
            }
            catch
            {
                //Console.WriteLine("Bład z plikiem");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Przycisk.Content = "Pobieram...Proszę czekać";
            Console.WriteLine("Pobieram...");
            Przycisk.IsEnabled = false;
            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                htmlWeb.OverrideEncoding = Encoding.GetEncoding("ISO-8859-2");//polskie znaki
                //var progress = new Progress<ProgressReport>();
                //progress.ProgressChanged += (o, report) =>
                //{
                //    LabelWynik.Content = string.Format("Proces trwa...{0}", report.PercenntComplete);
                //    progressBar.Value = report.PercenntComplete;
                //    progressBar.UpdateLayout();
                //};

                List<string> strony = new List<string>();
                for (int i = 1; i < 200; i++)
                {
                    string tytulstrony = "http://www.oglaszamy24.pl/ogloszenia/?std=1&keyword=szybka+po%BFyczka+w+domu+klienta&results=" + i.ToString();
                    strony.Add(tytulstrony);
                }
                int zmienna = 1;
                //
                foreach (string strona in strony)
                {
                    //await ProcessData(strony, progress);
                    HtmlDocument htmlDocument = htmlWeb.Load(strona);
                    ////////////////////////////linki/////////////////////////////////////////////
                    var links = from node in htmlDocument.DocumentNode.Descendants("a")
                                where node.Attributes.Contains("href") &&
                                      node.Attributes.Contains("class") &&
                                      node.Attributes.Contains("name") &&
                                      node.Attributes["name"].Value == "alnk" &&
                                      node.Attributes["class"].Value == "o_title" &&
                                      node.ParentNode.Name == "div"
                                select new
                                {
                                    a = node.Attributes["href"].Value,
                                };
                    Console.WriteLine("links ma {0} elementow", links.Count());
                    //LabelWynik.Content = links.Count().ToString();//
                    if (links.Count() == 0)
                    {
                        break;
                    }
                    foreach (var el in links)
                    {
                        //Console.WriteLine("{0}", el.a);
                        string nowylink = el.a.ToString();
                        HtmlDocument htmlDocument2 = htmlWeb.Load(nowylink);
                        ////////////////////////////tresc/////////////////////////////////////////////
                        var source = from node in htmlDocument2.DocumentNode.Descendants("div")
                                     where node.Attributes.Contains("id") &&
                                           node.Attributes.Contains("style") &&
                                           node.Attributes["id"].Value == "adv_desc" &&
                                           node.ParentNode.Name == "div"
                                     select new
                                     {
                                         a = node.InnerHtml,
                                     };
                        //Console.WriteLine("source ma {0} elementow", source.Count());
                        foreach (var element in source)
                        {
                            string nazwa = zmienna.ToString() + ".html";
                            Zapisz(element.a, nazwa);
                            zmienna++;
                        }
                    }
                    LabelWynik.Content = "Pobrano i zapisano pliki \nw  folderze PlikiTekstowe";
                }
            }
            catch
            {
                LabelWynik.Content = "Brak połączenia z internetem.";
            }
            Przycisk.Content = "Pobierz nowe dane";
            Przycisk.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PrzyciskGumtree.Content = "Pobieram...Proszę czekać";
            Console.WriteLine("Pobieram...");
            PrzyciskGumtree.IsEnabled = false;
            HtmlWeb htmlWeb = new HtmlWeb();
            string mojplik = Resource.lista_ogloszenia;
            string[] gumtreelink = mojplik.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine(gumtreelink.Count());
            int numerek = 1;
            try
            {
                for (int i = 0; i < gumtreelink.Count(); i++)
                //for (int i = 0; i < 10; i++)
                {
                    HtmlDocument htmlDocument3 = htmlWeb.Load("http://www." + gumtreelink[i]);
                    ////////////////////////////tresc/////////////////////////////////////////////
                    var sourcegumtree = from node in htmlDocument3.DocumentNode.Descendants("span")
                                        where node.Attributes.Contains("class") &&
                                        node.Attributes["class"].Value == "pre"
                                        select new
                                        {
                                            a = node.InnerHtml,
                                        };
                    if (sourcegumtree.Count() > 0)
                    {
                        foreach (var element in sourcegumtree)
                        {
                            //Console.WriteLine("{0}", element.a);

                            Zapisz(element.a, "gumtree-" + numerek + ".html");
                            //Console.WriteLine(numerek);
                            numerek++;
                        }
                        //Console.WriteLine(sourcegumtree.Count());
                        // Console.ReadLine();
                    }
                    else
                    {
                        numerek++;
                    }

                }
                Label2Wynik.Content = "Pobrano dane. Zapisano w\nfolderze PlikiTekstowe";
                PrzyciskGumtree.Content = "Pobierz nowe dane";
                PrzyciskGumtree.IsEnabled = true;
            }
            catch
            {
                Label2Wynik.Content = "Brak połączenia z internetem.";
                PrzyciskGumtree.Content = "Pobierz nowe dane";
                PrzyciskGumtree.IsEnabled = true;

            }
            //Label2Wynik.Content = "Pobrano dane. Zapisano w\nfolderze PlikiTekstowe";
            //PrzyciskGumtree.Content = "Pobierz nowe dane";
            //PrzyciskGumtree.IsEnabled = true;

        }

        private void Przycisk_MouseMove(object sender, MouseEventArgs e)
        {
                LabelInfo1.Content = "Aby pobrać dane  ze strony \nogloszenia24.pl kliknij w przycisk";
        }

        private void Przycisk_MouseLeave(object sender, MouseEventArgs e)
        {
            LabelInfo1.Content = "";
        }
    }
}
