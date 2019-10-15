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
        /* Информация для хранения разобраных лексем
         * [Программа][имя программы] - для название программы
         * [id№][тип][имя переменной][значение] - для переменных
         * [вывод/читать][указать то что выводим/в какую пременную считываем] - для вывода/ввода на экран  
         * [резервированное слово][условие/действие] - для условий
         */
        List<string[]> listStr = new List<string[]>();
        //списки для резервированных слов
        List<string> identPeremen = new List<string>() { "цп", "сп", "лп", "дп" };
        List<string> identReadWrite = new List<string>() { "вывод", "читать" };
        List<string> identCondition = new List<string>() { "Если", "то", "иначе" };
        List<string> identFor = new List<string>() { "для", "до", "{" ,"}"};
        List<string> identWhile = new List<string>() { "пока", "(", ")", "{", "}" };
        //список для хранения токенов
        List<string[]> listToken = new List<string[]>();
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
                        //запись в токен
                        if (getMasToken())
                        {
                            Form1.Str_Write += "\nЛексический анализ (вывод токенов).\n";
                            Form1.Str_Write += "/********************************/\n";
                            //вывод токенов на экран
                            foreach (string[] str in listToken)
                            {
                                for (int i = 0; i < str.Length; i++)
                                    switch(str[i])
                                    {
                                        case "id":{
                                                Form1.Str_Write += "(" + str[i] + ", " + str[i + 1] + ") ";
                                                i++; 
                                              }break;
                                        case "читать":
                                            {
                                                Form1.Str_Write +=str[i] + " (" + str[i + 1] + ", " + str[i + 2] + ") ";
                                                i = str.Length;
                                            }
                                            break;
                                        case "вывод":
                                            {
                                                Form1.Str_Write += str[i] + " ( " + str[i + 1] + " ) ";
                                                i = str.Length;
                                            }
                                            break;
                                        default: { Form1.Str_Write += str[i] + " "; } break;
                                    }
                                   
                                Form1.Str_Write += "\n";
                            }
                            Form1.Str_Write += "/********************************/\n";
                            Form1.Str_Write += "Лексический анализ выполнен.\n\n";
                        }
                        else Form1.Str_Write += "Ощибка получения токенов.\n";

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
                listStr.Add(new string[] { "Программа", str });
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
                recognition(str);                

                //if (!recognition1(str)) Form1.Str_Write += "Ошибка в строке: { " + str +" } \n";
                //if (!recognition2(str)) Form1.Str_Write += "Ошибка в строке: { " + str + " } \n";
                //if (!recognition3(str)) Form1.Str_Write += "Ошибка в строке: { " + str + " } \n";
                //if (!recognition4(str)) Form1.Str_Write += "Ошибка в строке: { " + str + " } \n";                
            }

            return 1;
        }

        private void recognition(string str)
        {
            string[] mas = new string[] { @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(=(\s)*(-)?([0-9])+(\s)*(;))", @"[-]?([0-9])*",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(=(\s)*[']((\s)*(\w)*)*['](\s)*)", @"(\s)*[']((\s)*(\w)*)*['](\s)*",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*=(\s)*правда|ложь(\s)*;", @"правда|ложь",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*=(\s)*([0-9]+).([0-9]+)(\s)*;", @"([0-9]+).([0-9]+)"};
            for (int i = 0; i < identPeremen.Count; i++)
            {
                try
                {
                    
                    Regex regex = new Regex(identPeremen[i] + mas[i*2]);
                    MatchCollection matches = regex.Matches(str);
                    Regex regex2 = new Regex(identPeremen[i] + @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(;)");
                    MatchCollection matches2 = regex2.Matches(str);
                    if ((matches.Count > 0) || (matches2.Count > 0))
                    {
                        //получаем тип своей переменной
                        regex = new Regex(identPeremen[i]);
                        matches = regex.Matches(str);
                        string ident = matches[0].ToString();

                        //получаем имя своей переменной
                        regex = new Regex(@":(\s)*([А-Я]|[а-я])([А-Я]|[а-я]|[0-9])*");
                        matches = regex.Matches(str);
                        string name = matches[0].ToString();
                        string sname = "";
                        for (int j = 0; j < name.Length; j++)
                            if ((name[j] != '\t') && (name[j] != ' ') && (name[j] != ':')) sname += name[j];
                            else if (sname.Length != 0) break;

                        //получаем значение своей переменной
                        regex = new Regex(mas[i*2+1]);
                        matches = regex.Matches(str);
                        if (matches.Count > 0)
                        {
                            for (int j = 0; j < matches.Count; j++)
                                if (matches[j].ToString() != "")
                                {
                                    listStr.Add(new string[] { "id", ident, sname, matches[j].ToString() });                                    
                                }
                            switch(ident)
                            {
                                case "цп": { listStr.Add(new string[] { "id", ident, sname, "0" }); } break;
                                case "сп": { listStr.Add(new string[] { "id", ident, sname, "''" }); } break;
                                case "лп": { listStr.Add(new string[] { "id", ident, sname, "ложь" }); } break;
                                case "дп": { listStr.Add(new string[] { "id", ident, sname, "0.0" }); } break;
                            }
                            
                        }
                        else
                        {
                             switch(ident)
                            {
                                //case "цп": { listStr.Add(new string[] { "id", ident, sname, "0" }); } break;
                                case "сп": { listStr.Add(new string[] { "id", ident, sname, "''" }); } break;
                                case "лп": { listStr.Add(new string[] { "id", ident, sname, "ложь" }); } break;
                                case "дп": { listStr.Add(new string[] { "id", ident, sname, "0.0" }); } break;
                            }

                        }

                    }
                }
                catch { }

            }
        }

       /* private bool recognition1(string str)
        {
            //for (int i = 0; i < identPeremen.Count; i++)
            {
                try
                {
                    Regex regex = new Regex(identPeremen[0] + @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(=(\s)*(-)?([0-9])+(\s)*(;))");
                    MatchCollection matches = regex.Matches(str);
                    Regex regex2 = new Regex(identPeremen[0] + @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(;)");
                    MatchCollection matches2 = regex2.Matches(str);
                    if ((matches.Count > 0)||(matches2.Count > 0))
                    {
                        //получаем тип своей переменной
                        regex = new Regex(identPeremen[0]);
                        matches = regex.Matches(str);
                        string ident = matches[0].ToString();

                        //получаем имя своей переменной
                        regex = new Regex(@":(\s)*([А-Я]|[а-я])([А-Я]|[а-я]|[0-9])*");
                        matches = regex.Matches(str);
                        string name = matches[0].ToString();
                        string sname = "";
                        for (int j = 0; j < name.Length; j++)
                            if ((name[j] != '\t') && (name[j] != ' ') && (name[j] != ':')) sname += name[j];
                            else if (sname.Length != 0) break;

                        //получаем значение своей переменной
                        regex = new Regex(@"[-]?([0-9])*");                        
                        matches = regex.Matches(str);
                        if (matches.Count > 0)
                        {
                            for (int j = 0; j < matches.Count; j++)
                                if (matches[j].ToString() != "")
                                {
                                    listStr.Add(new string[] { "id", ident, sname, matches[j].ToString() });
                                    return true;
                                }
                            listStr.Add(new string[] { "id", ident, sname, "0" });
                            return true;
                        }                      

                    }                   
                }
                catch { }
                
            }

            return true;
        }*/
        /*private bool recognition2(string str)
        {
           
            {
                try
                {
                    Regex regex = new Regex(identPeremen[1] + @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(=(\s)*[']((\s)*(\w)*)*['](\s)*)");
                    MatchCollection matches = regex.Matches(str);
                    Regex regex2 = new Regex(identPeremen[1] + @"(\s*):(\s*)([А-Я]||[а-я])([А-Я]||[а-я]||[0-9])*(\s)*(;)");
                    MatchCollection matches2 = regex2.Matches(str);
                    if ((matches.Count > 0) || (matches2.Count > 0))
                    {
                        //получаем тип своей переменной
                        regex = new Regex(identPeremen[1]);
                        matches = regex.Matches(str);
                        string ident = matches[0].ToString();

                        //получаем имя своей переменной
                        regex = new Regex(@":(\s)*([А-Я]|[а-я])([А-Я]|[а-я]|[0-9])*");
                        matches = regex.Matches(str);
                        string name = matches[0].ToString();
                        string sname = "";
                        for (int j = 0; j < name.Length; j++)
                            if ((name[j] != '\t') && (name[j] != ' ') && (name[j] != ':')) sname += name[j];
                            else if (sname.Length != 0) break;
                       
                        //получаем значение своей переменной                        
                        regex = new Regex(@"(\s)*[']((\s)*(\w)*)*['](\s)*");
                        matches = regex.Matches(str);
                        if (matches.Count > 0)
                        {
                            for (int j = 0; j < matches.Count; j++)
                                if (matches[j].ToString() != "")
                                {
                                    listStr.Add(new string[] { "id", ident, sname, matches[j].ToString() });
                                    return true;
                                }
                            listStr.Add(new string[] { "id", ident, sname, "''" });
                            return true;
                        }
                        else
                        {
                            listStr.Add(new string[] { "id", ident, sname, "''" });
                            return true;
                        }

                    }
                }
                catch { }

            }

            return true;
        }*/


       /* private bool recognition3(string str)
        {

            {
                try
                {
                    Regex regex = new Regex(identPeremen[2] + @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*=(\s)*правда|ложь(\s)*;");
                    MatchCollection matches = regex.Matches(str);
                    Regex regex2 = new Regex(identPeremen[2] + @"(\s*):(\s*)([А-Я]||[а-я])([А-Я]||[а-я]||[0-9])*(\s)*(;)");
                    MatchCollection matches2 = regex2.Matches(str);
                    if ((matches.Count > 0) || (matches2.Count > 0))
                    {
                        //получаем тип своей переменной
                        regex = new Regex(identPeremen[2]);
                        matches = regex.Matches(str);
                        string ident = matches[0].ToString();

                        //получаем имя своей переменной
                        regex = new Regex(@":(\s)*([А-Я]|[а-я])([А-Я]|[а-я]|[0-9])*");
                        matches = regex.Matches(str);
                        string name = matches[0].ToString();
                        string sname = "";
                        for (int j = 0; j < name.Length; j++)
                            if ((name[j] != '\t') && (name[j] != ' ') && (name[j] != ':')) sname += name[j];
                            else if (sname.Length != 0) break;

                        //получаем значение своей переменной                        
                        regex = new Regex(@"правда|ложь");
                        matches = regex.Matches(str);
                        if (matches.Count > 0)
                        {
                            for (int j = 0; j < matches.Count; j++)
                                if (matches[j].ToString() != "")
                                {
                                    listStr.Add(new string[] { "id", ident, sname, matches[j].ToString() });
                                    return true;
                                }
                            listStr.Add(new string[] { "id", ident, sname, "ложь" });
                            return true;
                        }
                        else
                        {
                            listStr.Add(new string[] { "id", ident, sname, "ложь" });
                            return true;
                        }

                    }
                }
                catch { }

            }

            return true;
        }*/

        /*private bool recognition4(string str)
        {

            {
                try
                {
                    Regex regex = new Regex(identPeremen[3] + @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*=(\s)*([0-9]+).([0-9]+)(\s)*;");
                    MatchCollection matches = regex.Matches(str);
                    Regex regex2 = new Regex(identPeremen[3] + @"(\s*):(\s*)([А-Я]||[а-я])([А-Я]||[а-я]||[0-9])*(\s)*(;)");
                    MatchCollection matches2 = regex2.Matches(str);
                    if ((matches.Count > 0) || (matches2.Count > 0))
                    {
                        //получаем тип своей переменной
                        regex = new Regex(identPeremen[3]);
                        matches = regex.Matches(str);
                        string ident = matches[0].ToString();

                        //получаем имя своей переменной
                        regex = new Regex(@":(\s)*([А-Я]|[а-я])([А-Я]|[а-я]|[0-9])*");
                        matches = regex.Matches(str);
                        string name = matches[0].ToString();
                        string sname = "";
                        for (int j = 0; j < name.Length; j++)
                            if ((name[j] != '\t') && (name[j] != ' ') && (name[j] != ':')) sname += name[j];
                            else if (sname.Length != 0) break;

                        //получаем значение своей переменной                        
                        regex = new Regex(@"([0-9]+).([0-9]+)");
                        matches = regex.Matches(str);
                        if (matches.Count > 0)
                        {
                            for (int j = 0; j < matches.Count; j++)
                                if (matches[j].ToString() != "")
                                {
                                    listStr.Add(new string[] { "id", ident, sname, matches[j].ToString() });
                                    return true;
                                }
                            listStr.Add(new string[] { "id", ident, sname, "0.0" });
                            return true;
                        }
                        else
                        {
                            listStr.Add(new string[] { "id", ident, sname, "0.0" });
                            return true;
                        }

                    }
                }
                catch { }

            }

            return true;
        }*/

        //дочитать до конца строку идентификатора
        private string idReadToEnd(in int i, string str)
        {
            string perem = "";
            bool flag = false;
            for (int j = i; j < str.Length; j++)
            {
                if ((str[j] != '\t') && (str[j] != ' ') && (str[j] != ';') && (str[j] != ':')) { perem += str[j]; flag = true; }
                else if (flag)
                {
                    //надо дочитать до конца и выдать ошибку если найден любой символ кроме пробела
                    flag = false;                                     
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
            return perem;
        }

        //дочитать до конца строку ввода/вывода
        private string rwReadToEnd(ref int i, string str )
        {
            string perem = "";
            bool flag = false;
            int rez = i;

            int t = Check_Scob2(str,0);
            if (t != 0)
            {
                Form1.Str_Write += "Нарушение скобочной структуры!\n";
                return null;
            }

            while ((i + 1 < str.Length) && (str[i] != '"')) i++;
            if (i + 1 < str.Length) i++;//чтобы взять следующий символ
            else {
                i = rez;
                while ((i + 1 < str.Length) && (str[i] != '(')) i++;
                if (i + 1 < str.Length) i++;//чтобы взять следующий символ
            }

            for (int j = i; j < str.Length; j++)
            {        
                if ((str[j] != '"') && (str[j] != '\n') && (str[j] != ';')) { perem += str[j]; flag = true; }
                else if (flag)
                {
                    //надо дочитать до конца и выдать ошибку если найден ');'
                    flag = false;
                    j++;
                    while (j < str.Length)
                    {
                        if (str[j] != ')')
                        {
                            while (j < str.Length)
                            {
                                if (str[j] != ';') j = str.Length;
                                j++;
                            }
                        }
                        j++;
                    }
                }
            }
            str = "";
            for (int j = perem.Length - 1; j >= 0; j--)
                if (perem[j] != ')') str += perem[j];
                else { j = -1; str += ')'; }
            if ((perem.Length - str.Length) != 0)
                return perem.Substring(0, perem.Length - str.Length);
            else return perem;

        }
        //дочитать до конца строку условие
        //номер для дальнейшего считывания, вся строка и начало строки
        private string conReadToEnd(int i, string str, ref string ident)
        {
            string perem = "";
            if (ident == "Если")
            {
                for (; i < str.Length && str[i] == ' '; i++) { }                                              

                for (int j = i; j < str.Length && str[j]!='\n' ; j++)
                {
                    perem += str[j];
                }
                return perem;
            }
            else {
                for (; i < str.Length && str[i] == ' '; i++) { }

                for (int j = i; j < str.Length && str[j] != '\n'; j++)
                {
                    perem += str[j];
                }
                return perem;
            }               
        }

        private bool getMasToken()
        {
            bool result = false;
            int i = 0;

            foreach (string[] str in listStr)
            {
                switch(str[0])
                {
                    case "id": {                          
                            listToken.Add(new string[] { "id" , i.ToString(), "=", str[3] });
                        } break;
                    case "вывод":{                            
                            listToken.Add(new string[] { str[0], str[1]});
                        } break;
                    case "читать":{                            
                            listToken.Add(new string[] { str[0], "id", changeIdent(str[1]) });
                        } break;
                }
                i++;
            }
            result = true;              
            

            return result;
        }

        //метод для замены идентификаторов на обозначение (возможно это лишнее)
        private string changeIdent(string s)
        {
            int j = 0;
            foreach (string[] str in listStr)
            {
                for (int i = 0; i < str.Length; i++)
                    if (str[i] == s) {
                        return j.ToString();
                    }
                j++;
            }
            Form1.Str_Write += "Переменная не объявлена!\n";
            return null;
        }
    }
}
