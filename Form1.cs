using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace POSK_Projekt5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int[] AX = new int[16];
        int[] BX = new int[16];
        int[] CX = new int[16];
        int[] DX = new int[16];
        int[] arg = new int[16];

        int[] bufor_AX = new int[16];
        int[] bufor_BX = new int[16]; 
        int[] bufor_CX = new int[16]; 
        int[] bufor_DX = new int[16];
        int[] bufor_arg = new int[16];

        int[] reset_list = new int[16];
        int[] argument1 = new int[16];
        int[] argument2 = new int[16];

        string cel;
        int krokowy = 0;

        List<String> lista_rozkazy = new List<String>();

        public void Ustaw_bity(object sender, EventArgs e)  //ustawia na biezaco bity w buforze
        {
            Button b = (Button)sender;
            int nr_bitu = 15 - Convert.ToInt32(b.Name.Substring(1, 2));
            int wartosc;
            if (b.BackColor == Color.LightGray)
            {
                wartosc = 1;
                b.BackColor = Color.Red;
            }
            else
            {
                wartosc = 0;
                b.BackColor = Color.LightGray;
            }
            switch (b.Name[0].ToString())
            {
                case "A":
                    bufor_AX[nr_bitu] = wartosc;
                    break;

                case "B":
                    bufor_BX[nr_bitu] = wartosc;
                    break;

                case "C":
                    bufor_CX[nr_bitu] = wartosc;
                    break;

                case "D":
                    bufor_DX[nr_bitu] = wartosc;
                    break;

                case "n":
                    bufor_arg[nr_bitu] = wartosc;
                    break;
            }
        }

        public void Wpisz_rejestr(object sender, EventArgs e)   //wpisuje bity do rejestru ustawione w buforze
        {
            Button b = (Button)sender;

            if (b.Name[5].ToString() == "A")
            {
                bufor_AX.CopyTo(AX, 0);
                AL.Text = String.Join("", AX).Substring(8, 8);
                AH.Text = String.Join("", AX).Substring(0, 8);
            }
            else if (b.Name[5].ToString() == "B")
            {
                bufor_BX.CopyTo(BX, 0);
                BL.Text = String.Join("", BX).Substring(8, 8);
                BH.Text = String.Join("", BX).Substring(0, 8);
            }
            else if (b.Name[5].ToString() == "C")
            {
                bufor_CX.CopyTo(CX, 0);
                CL.Text = String.Join("", CX).Substring(8, 8);
                CH.Text = String.Join("", CX).Substring(0, 8);
            }
            else if (b.Name[5].ToString() == "D")
            {
                bufor_DX.CopyTo(DX, 0);
                DL.Text = String.Join("", DX).Substring(8, 8);
                DH.Text = String.Join("", DX).Substring(0, 8);
            }
            else if (b.Name[5].ToString() == "T")
            {
                bufor_arg.CopyTo(arg, 0);
                nL.Text = String.Join("", arg).Substring(8, 8);
                nH.Text = String.Join("", arg).Substring(0, 8);
            }
        }

        public void Reset_argumentow()  // resetuje argumenty (bufory dla wykonywanych operacji)
        {
            Array.Clear(argument1, 0, 16);
            Array.Clear(argument2, 0, 16);
            cel = "";
        }

        public void Wpisanie_do_rejestru(int[] arg1, string cel)    //wpisanie do rejestru ustawionych bitow w buforze
        {
            string wpisanie = String.Join("", arg1);
            string H = wpisanie.Substring(0, 8);
            string L = wpisanie.Substring(8, 8);
            if (cel == "AX")
            {
                arg1.CopyTo(AX, 0);
                AL.Text = L;
                AH.Text = H;
            }
            else if (cel == "BX")
            {
                arg1.CopyTo(BX, 0);
                BL.Text = L;
                BH.Text = H;
            }
            else if (cel == "CX")
            {
                arg1.CopyTo(CX, 0);
                CL.Text = L;
                CH.Text = H;
            }
            else if (cel == "DX")
            {
                arg1.CopyTo(DX, 0);
                DL.Text = L;
                DH.Text = H;
            }
            Reset_argumentow();
        }

        public void MOV(int[] arg1, int[] arg2) //przeniesienie/wpisanie
        {
            arg2.CopyTo(arg1, 0);
            Wpisanie_do_rejestru(arg1, cel);
        }

        public void ADD(int[] arg1, int[] arg2) //dodawanie
        {
            int przeniesienie = 0;
            int[] dodawanie = new int[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 15; i >= 0; i--)
            {
                int x = arg1[i] + arg2[i] + przeniesienie;

                if (x == 0 || x == 1)
                {
                    dodawanie[i] = x;
                    przeniesienie = 0;
                }
                else if (x == 2)
                {
                    dodawanie[i] = 0;
                    przeniesienie = 1;
                }
                else if (x == 3)
                {
                    dodawanie[i] = 1;
                    przeniesienie = 1;
                }
            }
            if (przeniesienie == 1)
            {
                label_flaga.Text = "CF = 1. Wynik nieprawidłowy, Konieczny reset rejestru docelowego";
            }
            dodawanie.CopyTo(arg1, 0);
            Wpisanie_do_rejestru(arg1, cel);
        }

        public void SUB(int[] arg1, int[] arg2) //odejmowanie
        {
            int[] odejmowanie = new int[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int borrow = 0;

            for (int i = 15; i >= 0; i--)
            {
                int x = arg1[i] - arg2[i] - borrow;
                if (x == -1)
                {
                    odejmowanie[i] = 1;
                    borrow = 1;
                }
                else if (x == -2)
                {
                    odejmowanie[i] = 0;
                    borrow = 1;
                }
                else
                {
                    odejmowanie[i] = x;
                    borrow = 0;
                }
            }
            if (borrow == 1)
            {
                label_flaga.Text = "CF = 1. Wynik nieprawidłowy, Konieczny reset rejestru docelowego.";
            }
            odejmowanie.CopyTo(arg1, 0);
            Wpisanie_do_rejestru(arg1, cel);
        }

        private void Wykonaj_rozkaz(object sender, EventArgs e) //wykonuje bezposrednio tylko jeden wybrany rozkaz
        {
            switch (argument1_comboBox.SelectedItem)
            {
                case "AX":
                    AX.CopyTo(argument1, 0);
                    cel = "AX";
                    break;

                case "BX":
                    BX.CopyTo(argument1, 0);
                    cel = "BX";
                    break;

                case "CX":
                    CX.CopyTo(argument1, 0);
                    cel = "CX";
                    break;

                case "DX":
                    DX.CopyTo(argument1, 0);
                    cel = "DX";
                    break;
            }

            switch (argument2_comboBox.SelectedItem)
            {
                case "AX":
                    AX.CopyTo(argument2, 0);
                    break;

                case "BX":
                    BX.CopyTo(argument2, 0);
                    break;

                case "CX":
                    CX.CopyTo(argument2, 0);
                    break;

                case "DX":
                    DX.CopyTo(argument2, 0);
                    break;

                case "Tn":
                    arg.CopyTo(argument2, 0);
                    break;
            }

            switch (rozkaz_comboBox.SelectedItem)
            {
                case "MOV":
                    MOV(argument1, argument2);
                    break;

                case "ADD":
                    ADD(argument1, argument2);
                    break;

                case "SUB":
                    SUB(argument1, argument2);
                    break;
            }
        }

        private void Zmien_napis_operacji(object sender, EventArgs e)   //zmienia napis wykonywanej operacji arytmetycznej
        {
            switch (rozkaz_comboBox.SelectedItem)
            {
                case "MOV":
                    label_operacja.Text = "<=";
                    break;

                case "ADD":
                    label_operacja.Text = "+";
                    break;

                case "SUB":
                    label_operacja.Text = "-";
                    break;
            }
        }

        private void Zapisz_rozkaz(object sender, EventArgs e)  //dodaje rozkaz do listy rozkazow
        {
            if (rozkaz_comboBox.SelectedItem != null && argument1_comboBox.SelectedItem != null
                && argument2_comboBox.SelectedItem != null)
            {
                textBox_lista_rozkazow.Clear();
                string rozkaz = rozkaz_comboBox.SelectedItem.ToString()
                    + argument1_comboBox.SelectedItem.ToString() + argument2_comboBox.SelectedItem.ToString();
                lista_rozkazy.Add(rozkaz);
                Zapisz_usun_rozkaz();
            }
        }

        private void Usun_rozkaz(object sender, EventArgs e)    //usuwa rozkaaz z listy rozkazow
        {
            if (lista_rozkazy.Count > 0)
            {
                lista_rozkazy.RemoveAt(lista_rozkazy.Count - 1);
                textBox_lista_rozkazow.Clear();
                Zapisz_usun_rozkaz();
            }
        }

        private void Zapisz_usun_rozkaz()   //wpisuje rozkazy do textBoxa rozkazow
        {
            try
            {
                for (int i = 0; i <= lista_rozkazy.Count - 1; i++)
                {
                    string wpisz = (i + 1).ToString() + "    " + lista_rozkazy[i].Substring(0, 3) + "  "
                    + lista_rozkazy[i].Substring(3, 2) + ", " + lista_rozkazy[i].Substring(5, 2);
                    textBox_lista_rozkazow.AppendText(wpisz);
                    textBox_lista_rozkazow.AppendText(Environment.NewLine);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wprowadzono plik z nieprawidłowymi danymi.", "Błąd", MessageBoxButtons.OK);
            }

        }

        private void Wykonaj_program(object sender, EventArgs e)    //wykonuje wszystkie rozkazy
        {
            foreach (string r in lista_rozkazy)
            {
                Calosciowy_krokowy(r);
            }
        }

        private void Calosciowy_krokowy(string r)   //funkcja wykonujaca jeden rozkaz
        {
            if (r.Substring(3, 2) == "AX")
            {
                AX.CopyTo(argument1, 0);
                cel = "AX";
            }
            else if (r.Substring(3, 2) == "BX")
            {
                BX.CopyTo(argument1, 0);
                cel = "BX";
            }
            else if (r.Substring(3, 2) == "CX")
            {
                CX.CopyTo(argument1, 0);
                cel = "CX";
            }
            else if (r.Substring(3, 2) == "DX")
            {
                DX.CopyTo(argument1, 0);
                cel = "DX";
            }

            if (r.Substring(5, 2) == "AX")
            {
                AX.CopyTo(argument2, 0);
            }
            else if (r.Substring(5, 2) == "BX")
            {
                BX.CopyTo(argument2, 0);
            }
            else if (r.Substring(5, 2) == "CX")
            {
                CX.CopyTo(argument2, 0);
            }
            else if (r.Substring(5, 2) == "DX")
            {
                DX.CopyTo(argument2, 0);
            }
            else if (r.Substring(5, 2) == "Tn")
            {
                arg.CopyTo(argument2, 0);
            }

            switch (r.Substring(0, 3))
            {
                case "MOV":
                    MOV(argument1, argument2);
                    break;
                case "SUB":
                    SUB(argument1, argument2);
                    break;
                case "ADD":
                    ADD(argument1, argument2);
                    break;
            }
        }

        private void Praca_krokowa(object sender, EventArgs e)  //umozliwia prace krokowa
        {
            if (lista_rozkazy.Count > 0 && krokowy <= lista_rozkazy.Count - 1)
            {
                string r = lista_rozkazy[krokowy];
                Calosciowy_krokowy(r);
                krokowy += 1;
            }
            else if (krokowy > lista_rozkazy.Count - 1)
            {
                krokowy = 0;
            }
            label_linia_wykonanego_rozkazu.Text = "Linia wykonanego rozkazu: " + krokowy;
        }

        private void Reset_kodu_programu(object sender, EventArgs e)    //resetuje liste rozkazow
        {
            lista_rozkazy.Clear();
            textBox_lista_rozkazow.Clear();
        }

        private void Zapisz_program(object sender, EventArgs e) //zapisuje rejestry i liste rozkazow do programu
        {
            try
            {
                using StreamWriter plik = File.CreateText("procesor.txt");
                plik.WriteLine(String.Join("", AX));
                plik.WriteLine(String.Join("", BX));
                plik.WriteLine(String.Join("", CX));
                plik.WriteLine(String.Join("", DX));
                plik.WriteLine(String.Join(",", lista_rozkazy));
                MessageBox.Show("Program zapisano pomyślnie.", "Zapis programu", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                MessageBox.Show(message, "Błąd", MessageBoxButtons.OK);
            }

        }

        private void Wczytaj_program(object sender, EventArgs e)    //wczytuje plik tekstowy do programu
        {
            string path;
            string rozszerzenie;
            Regex reg_rejestry = new Regex("^[0-1]+$");
            Regex reg_rozkazy = new Regex("^[ABCDnMOSTUVX,]+$");

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                rozszerzenie = Path.GetExtension(path);

                if (rozszerzenie.CompareTo(".txt") == 0)
                {
                    try
                    {
                        using StreamReader plik = new StreamReader(path);

                        string textAX = plik.ReadLine();
                        string textBX = plik.ReadLine();
                        string textCX = plik.ReadLine();
                        string textDX = plik.ReadLine();
                        string textlista_rozkazu = plik.ReadLine();

                        if (reg_rejestry.IsMatch(textAX) && reg_rejestry.IsMatch(textBX) && reg_rejestry.IsMatch(textCX) &&
                            reg_rejestry.IsMatch(textDX) && reg_rozkazy.IsMatch(textlista_rozkazu))
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                AX[i] = (int)char.GetNumericValue(textAX[i]);
                                BX[i] = (int)char.GetNumericValue(textBX[i]);
                                CX[i] = (int)char.GetNumericValue(textCX[i]);
                                DX[i] = (int)char.GetNumericValue(textDX[i]);
                            }

                            Wpisanie_do_rejestru(AX, "AX");
                            Wpisanie_do_rejestru(BX, "BX");
                            Wpisanie_do_rejestru(CX, "CX");
                            Wpisanie_do_rejestru(DX, "DX");
                            lista_rozkazy = textlista_rozkazu.Split(",").ToList();
                            textBox_lista_rozkazow.Clear();

                            if (lista_rozkazy.First() != "")
                            {
                                Zapisz_usun_rozkaz();
                            }
                            else
                            {
                                lista_rozkazy.Clear();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Plik z nieprawidłowymi danymi.", "Błąd", MessageBoxButtons.OK);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        MessageBox.Show("Plik z nieprawidłowymi danymi rejestrow", "Błąd", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Zły format pliku", "Błąd", MessageBoxButtons.OK);
                }
            }
        }

        private void Przycisk_Reset_Rejestrow(object sender, EventArgs e)    //reset rejestrow AX, BX, CX, DX
        {
            Wpisanie_do_rejestru(reset_list, "AX");
            Wpisanie_do_rejestru(reset_list, "BX");
            Wpisanie_do_rejestru(reset_list, "CX");
            Wpisanie_do_rejestru(reset_list, "DX");
            label_flaga.Text = "CF = 0.";
        }
    }
}

