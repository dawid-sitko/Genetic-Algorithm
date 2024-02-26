using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Algorytm
{
    public class AlgorytmGenetyczny
    {
        private static Random rand = new Random();

        public void PrzeprowadzObliczenia(int licznikPopulacji, int liczebnosc, double krzyzowanie, double mutacja, int a, int b, int c, int iloscWywolan)
        {
            string[] wyniki = new string[iloscWywolan];
            for(int i=0;i<iloscWywolan;i++)
            {
                var wynik = Max(licznikPopulacji, liczebnosc, krzyzowanie, mutacja, a, b, c);
                string liniaDoZapisu = $"{wynik} {FunkcjaKwadratowa(a, b, c, wynik, 0.0)}";
                wyniki[i] = liniaDoZapisu;
            }
            File.WriteAllLines("Wyniki.txt", wyniki);
        }
        
        public int Max(int licznikPopulacji, int liczebnosc, double krzyzowanie, double mutacja, int a, int b, int c)
        {
            SprawdzParametry(licznikPopulacji, liczebnosc, krzyzowanie, mutacja, a, b, c);
            var minimum = 0.0;

            if (a < 0 || (a>0 && (b<0 || c<0 )))
            {
                var poczatek = FunkcjaKwadratowa(a, b, c, 0, 0.0);
                var koniec = FunkcjaKwadratowa(a, b, c, 255, 0.0);
                if (poczatek < 0)
                {
                    minimum = poczatek;
                }
                if (koniec < 0 && koniec < minimum)
                {
                    minimum = koniec;
                }
            }

            List<DNA> populacja = new List<DNA>();

            for (int i = 0; i < liczebnosc; i++)
            {
                int[] kod = StworzKodDNA();

                populacja.Add(new DNA(kod));
            }

            for (int i = 0; i < licznikPopulacji; i++)
            {
                populacja = PomieszajPopulacje(populacja);
                populacja = Krzyzowanie(populacja, krzyzowanie);
                populacja = Mutacja(populacja, mutacja);

                var sumaFunkcji = 0.0;
                var rozkladPrawdopodobienstwa = new List<double>();
                List<DNA> nowaPopulacja = new List<DNA>();

                for (int j = 0; j < populacja.Count(); j++)
                {
                    sumaFunkcji += FunkcjaKwadratowa(a, b, c, populacja[j].WartoscDziesietna(), minimum);
                }

                for (int j = 0; j < populacja.Count(); j++)
                {
                    rozkladPrawdopodobienstwa.Add(FunkcjaKwadratowa(a, b, c, populacja[j].WartoscDziesietna(), minimum) / sumaFunkcji);
                }

                for (int j = 0; j < liczebnosc; j++)
                {
                    var zmiennaLosowa = rand.NextDouble();
                    var sumaPrawdopodobienstwa = 0.0;

                    for (int k = 0; k < rozkladPrawdopodobienstwa.Count(); k++)
                    {
                        sumaPrawdopodobienstwa += rozkladPrawdopodobienstwa[k];
                        if (sumaPrawdopodobienstwa >= zmiennaLosowa)
                        {
                            nowaPopulacja.Add(populacja[k]);
                            break;
                        }
                    }
                }
                populacja.Clear();
                populacja.AddRange(nowaPopulacja);
            }

            for (int j = 0; j < populacja.Count(); j++)
            {
                populacja[j].wartosFunkcji = FunkcjaKwadratowa(a, b, c, populacja[j].WartoscDziesietna(), minimum);
            }

            populacja = populacja.OrderBy(x => x.wartosFunkcji).ToList();

            return populacja.Last().WartoscDziesietna(); 
        }
        private void SprawdzParametry(int populacje, int liczebnosc, double krzyzowanie, double mutacja, int a, int b, int c)
        {
            if(populacje*liczebnosc>150)
            {
                throw new Exception("Za duza liczba osobnikow do przetworzenia");
            }
            if(krzyzowanie <= 0 || krzyzowanie >1 || mutacja <= 0 || mutacja > 1)
            {
                throw new Exception("Wskazniki algorytmu poza zakresem");
            }
      
        }
        private double FunkcjaKwadratowa(int a, int b, int c, int x, double minimum)
        {
            if (minimum != 0.0)
            {
                return a * x * x + b * x + c + minimum;
            }
            return a * x * x + b * x + c;
        }
        private List<DNA> Mutacja(List<DNA> populacja, double mutacja)
        {
            for (int i = 0; i < populacja.Count; i++)
            {
                for (int j = 0; j < populacja[i].kod.Count(); j++)
                {
                    if (rand.NextDouble() <= mutacja)
                    {
                        if(populacja[i].kod[j] == 0)
                        {
                            populacja[i].kod[j] = 1;
                        }
                        else
                        {
                            populacja[i].kod[j] = 0;
                        }
                    }
                }
            }
            return populacja;
        }
        private List<DNA> Krzyzowanie(List<DNA> pop, double krzyzowanie)
        {
            List<DNA> potomkowie = new List<DNA>();
            for (int i = 0; i + 1 < pop.Count; i += 2)
            {
                var r1 = pop[i];
                var r2 = pop[i + 1];

                if (rand.NextDouble() <= krzyzowanie)
                {
                    int pozycja = (int)(1 + (rand.NextDouble() * (7 - 1))); 
                    var p1 = new DNA(StworzKodDNAPotomka(r1.kod, r2.kod, true, pozycja));
                    var p2 = new DNA(StworzKodDNAPotomka(r1.kod, r2.kod, true, pozycja));
                    potomkowie.Add(p1);
                    potomkowie.Add(p2);
                }
            }
            pop.AddRange(potomkowie);
            return pop;
        }
        private List<DNA> PomieszajPopulacje(List<DNA> populacja)
        {
            int n = populacja.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                DNA dna = populacja[k];
                populacja[k] = populacja[n];
                populacja[n] = dna;
            }
            return populacja;
        }
        private int[] StworzKodDNA()
        {
            int[] kod = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (rand.NextDouble() >= 0.5)
                {
                    kod[i] = 1;
                }
                else
                {
                    kod[i] = 0;
                }
            }
            return kod;
        }
        private int[] StworzKodDNAPotomka(int[] r1, int[] r2, bool pierwszyRodzic, int pozycja)
        {
            int[] kod = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (pierwszyRodzic)
                {
                    if (i < pozycja)
                    {
                        kod[i] = r1[i];
                    }
                    else
                    {
                        kod[i] = r2[i];
                    }
                }
                else
                {
                    if (i < pozycja)
                    {
                        kod[i] = r2[i];
                    }
                    else
                    {
                        kod[i] = r1[i];
                    }
                }
            }
            return kod;
        }
        private class DNA
        {
            public int[] kod;

            public double wartosFunkcji;
            public DNA(int[] kod)
            {
                this.kod = kod;
            }

            public int WartoscDziesietna()
            {
                var wartoscDziesietna = 0;
                for (int i = (kod.Count() - 1); i >= 0; i--)
                {
                    if (kod[i] == 1)
                    {
                        var to = (int)Math.Pow(2, kod.Count() - (i + 1));
                        wartoscDziesietna += to;
                    }

                }
                return wartoscDziesietna;
            }
        }
    }
}
