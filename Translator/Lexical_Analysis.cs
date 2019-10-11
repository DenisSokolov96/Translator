using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Translator
{
    class Lexical_Analysis
    {
        /*---------------------------------------------------*/
        //возможно надо завести еще одну для типа данных!!!!!!!!!!!!!!!!!!!!! начать с этого
        Dictionary<string, string> tableId = new Dictionary<string, string>();
        Dictionary<string, string> lexems = new Dictionary<string, string>();
        //private int Number_func = -1;
        private int countId = 0;
        //списки для резервированных слов
        List<string> identPeremen = new List<string>() { "цп", "сп", "бп", "дп" };
        List<string> identReadWrite = new List<string>() { "вывод", "читать" };
        List<string> identCondition = new List<string>() { "Если", "то", "иначе" };
        string[,] matTokens = new string[50, 10];
        /*---------------------------------------------------*/


        public void Start_Analysis(string[] Text)
        {
            //Обнуление строки информации
            Form1.Str_Write = "";
            //разрешение на перекомпиляцию
            Form1.Launcher_Prog = true;
            if (Search_Programm(Text) == 1) //поиск - программа
            {
                if (Search_Func_Main(Text) == 1) //поиск - главная функция
                {
                    if (Read_Str_Func(Text) == 1)
                    {
                        Form1.Str_Write += "Главная функция прочитана успешно.\n";                       
                    }

                }
            }

        }

        private int Search_Programm(string[] Text)
        {
            int k = 0;
            string str_name = "";
            foreach (string str in Text)
            {
                Regex regex = new Regex(@"Программа(\s)(\s)*[А-Я]([А-Я,а-я,0-9,_]*)*;");
                MatchCollection matches = regex.Matches(str);
                k += matches.Count;
                if (matches.Count == 1) str_name = matches[0].ToString();
            }

            if (k != 1)
            {
                Form1.Str_Write += "Неправильная конструкция: Программа <Имя>;\n**********************\n";
                return 0;
            }
            else
            {
                string str = "";
                for (int i = 9; i < str_name.Length - 1; i++)
                    if (str_name[i].ToString() != " ") str += str_name[i];
                tableId.Add("Программа", str);
                lexems.Add("id" + countId.ToString(), "Программа");
                countId++;
                return 1;
            };
        }

        private int Search_Func_Main(string[] Text)
        {
            int k = 0;
            int rang = 0;
            int rang2 = 0;

            foreach (string str in Text)
            {

                Regex regex = new Regex(@"Главная(\s*)");
                MatchCollection matches = regex.Matches(str);
                k += matches.Count;
                if (k > 0)
                {
                    rang = Check_Scob1(str, rang);
                    rang2 = Check_Scob2(str, rang2);

                }
            }

            if (rang != 0)
            {
                Form1.Str_Write += "Скобочная структура { } не верна.\n**********************\n";
                return 0;
            }

            if (rang2 != 0)
            {
                Form1.Str_Write += "Скобочная структура ( ) не верна.\n**********************\n";
                return 0;
            }

            if (k != 1)
            {
                Form1.Str_Write += "Не найдена главная функция.\n**********************\n";
                return 0;
            }
            else return 1;
        }

        //функция проверки (подсчет) скобочной структуры {}
        private int Check_Scob1(string str, int rang)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '{') rang++;
                if (str[i] == '}') rang--;
                if (rang < 0) break;
            }
            return rang;
        }

        //функция проверки (подсчет) скобочной структуры ()
        private int Check_Scob2(string str, int rang2)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(') rang2++;
                if (str[i] == ')') rang2--;
                if (rang2 < 0) break;
            }
            return rang2;
        }

        //Чтение и разбор строк в функции 
        private int Read_Str_Func(string[] Text)
        {
            foreach (string str in Text)
            {
                string ident = "";
                bool flag = false;
                for (int i = 0; i < str.Length; i++)
                {
                    if ((str[i] != '\t') && (str[i] != ' ') && (str[i] != ':') && (str[i] != '(') && (str[i] != ')') && (str[i] != '{') && (str[i] != '}')) { ident += str[i]; flag = true; }
                    else
                    {
                        if (flag)
                        {
                            flag = false;
                            Form1.Str_Write += ident + "\n";
                            //определяем что считали, если это идентификатор переменной, то ее надо создать
                            //или проверить что она уже создана
                            switch (checkRezer(ident))
                            {
                                case "id": { readToEnd(i, str); i = str.Length; } break;
                                case "rw": { } break;
                                case "con": { } break;
                                case "null": { } break;
                            }
                            ident = "";
                        }
                    }
                }
                ident = "";


            }

            return 1;
        }

        //проверка типа резервированного слова
        private string checkRezer(string str)
        {
            if (identPeremen.Contains(str)) return "id";
            if (identReadWrite.Contains(str)) return "rw";
            if (identCondition.Contains(str)) return "con";

            return "null";
        }

        //дочитать до конца строку
        private void readToEnd(int i, string str)
        {
            string perem = "";
            bool flag = false;
            for (int j = i; j < str.Length; j++)
            {
                if ((str[j] != ' ') && (str[j] != ';') && (str[j] != ':')) { perem += str[j]; flag = true; }
                else if (flag)
                {
                    //надо дочитать до конца и выдать ошибку если найден любой символ кроме пробела
                    flag = false;
                    tableId.Add(perem, "0");
                    lexems.Add("id" + countId.ToString(), perem);
                    countId++;
                    j++;
                    while (j < str.Length)
                    {
                        if (str[j] != ' ')
                        {
                            j = str.Length;
                            //передача ошибки
                            Form1.Str_Write += "Ошибка в строке, обнаружены лишние символы!\n";
                        }
                        j++;
                    }
                }
            }

        }
    }
}
