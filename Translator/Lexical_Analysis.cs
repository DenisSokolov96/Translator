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
         * 
         * [Программа][имя программы] - для название программы
         * 
         * [id№][тип][имя переменной][значение] - для переменных
         * 
         * [вывод/читать][указать то что выводим/в какую пременную считываем] - для вывода/ввода на экран  
         * 
         * [резервированное слово][условие/действие] - для условий
         * 
         * [функ][Главная ()] - для объявления функции
         * [{]
         * [}]
         * 
         * [цикл][для] - для цикла for
         * [id№][тип][имя переменной][значение] - для переменных
         * [выполнение][имя переменной/id№][<=][значение/переменная]
         * [изменение][счет][1]
         * [{]
         * [}]
         */
        List<string[]> listStr = new List<string[]>();
        //списки для резервированных слов
        List<string> identPeremen = new List<string>() { "цп", "сп", "лп", "дп" };
        List<string> identReadWrite = new List<string>() { "вывод", "читать", "вывод" };
        List<string> identCondition = new List<string>() { "Если", "то", "иначе" };
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
                if (Search_Func_Main(Text) == 1) //поиск - главная функция                
                    if (Read_Str_Text(Text) == 1)                    
                        //запись в токен
                        if (getMasToken()) writeToken();                                                    
                        else Form1.Str_Write += "Ощибка получения токенов.\n";            
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
                listStr.Add(new string[] { str_name });
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

                Regex regex = new Regex(@"(\s*)Главная(\s*)\((\s*)\)(\{*)");
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
            else {
                listStr.Add(new string[] { "функция" , "Главная ()" });
                return 1;
            }
            
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
        private int Read_Str_Text(string[] Text)
        {
            int i = 0;
            foreach (string str in Text)
            {
                //проверка на цикл for
                if (!cycleFor(str, in i, in Text))
                {
                    //проверка на ввод/вывод
                    if (!writeRead(str))
                    {
                        //проверка на инициализацию переменных
                        if (!recognition(str))
                        {
                            //проверка на выражение
                            if (!expression(str))
                            {   
                                switch (str) {
                                    case "\t{": 
                                    case "{":
                                    case "\t}":
                                    case "}": { listStr.Add(new string[] { str }); } break;
                                    case "Главная (){": { listStr.Add(new string[] { "{" }); } break;
                                    default: {
                                            if ((str != "") && (str != "\t") && (str != "\t\t") && !listStr[0].Contains(str) )
                                                Form1.Str_Write += "Ошибка в строке: {" + str + " }\n";
                                        } break;
                                }
                            }
                        }                        
                    }                        

                }                
                i++;
            }

            return 1;
        }

        //поиск обьявления переменных
        private bool recognition(string str)
        {
            string[] mas = new string[] { @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(=(\s)*(-)?([0-9])+(\s)*(;))", @"[-]?([0-9])*",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(=(\s)*[']((\s)*(\w)*)*['](\s)*)", @"(\s)*[']((\s)*(\w)*)*['](\s)*",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*=(\s)*правда|ложь(\s)*;", @"правда|ложь",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*=(\s)*([0-9]+).([0-9]+)(\s)*;", @"([0-9]+).([0-9]+)"};
            string[] masNotChange = new string[] { @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я]||[0-9])*)*(\s)*(;)", @":(\s)*([А-Я]|[а-я])([А-Я]|[а-я]|[0-9])*" };
            for (int i = 0; i < identPeremen.Count; i++)
            {
                try
                {                                       
                    Regex regex = new Regex(identPeremen[i] + mas[i*2]);
                    MatchCollection matches = regex.Matches(str);

                    Regex regex2 = new Regex(identPeremen[i] + masNotChange[0]);
                    MatchCollection matches2 = regex2.Matches(str);
                    if ((matches.Count > 0) || (matches2.Count > 0))
                    {
                        //получаем тип своей переменной
                        regex = new Regex(identPeremen[i]);
                        matches = regex.Matches(str);
                        string ident = matches[0].ToString();

                        //получаем имя своей переменной
                        regex = new Regex(masNotChange[1]);
                        matches = regex.Matches(str);
                        string name = matches[0].ToString();
                        string sname = "";
                        for (int j = 0; j < name.Length; j++)
                            if ((name[j] != '\t') && (name[j] != ' ') && (name[j] != ':')) sname += name[j];
                            else if (sname.Length != 0) break;

                        //получаем значение своей переменной
                        regex = new Regex(mas[i * 2 + 1]);
                        matches = regex.Matches(str);
                        if (matches.Count > 0)
                        {
                            bool f = false;
                            for (int j = 0; j < matches.Count; j++)
                                if (matches[j].ToString() != "")
                                {
                                    listStr.Add(new string[] { "id", ident, sname, matches[j].ToString() });
                                    f = true;
                                    return true;
                                }
                            if (f) break;
                            switchStr(ref ident, ref sname);
                            return true;

                        }
                        else {
                            switchStr(ref ident, ref sname);
                            return true;
                        }                       

                    }
                }
                catch { }
            }

            //не найден
            return false;
        }

        //поиск обьявления операторов ввода/вывода
        private bool writeRead(string str)
        {
            string[] mas = new string[] { @"(\s)*", @"('[\w*\s*(:)*(+)*(-)*(*)*(/)*(?)*]*')*", 
                                          @"(\s)*", @"(\s)*([А-Яа-я]*)+\w*(\s)*",
                                          @"(\s)*", @"(\s)*((([А-Яа-я])+\w*)|([+-]\d*))*(\s)*([+-/*]*)(\s)*((([А-Яа-я])+\w*)|([+-]\d*))*(\s)*"};
            for (int i = 0; i < identReadWrite.Count; i++)
            {
                try
                {                   
                    Regex regex = new Regex(identReadWrite[i] + mas[i * 2]);
                    MatchCollection matches = regex.Matches(str);

                    if (matches.Count > 0)
                    {
                        //получаем тип своей переменной
                        regex = new Regex(identReadWrite[i]);
                        matches = regex.Matches(str);
                        string ident = matches[0].ToString();
                                               
                        //что выводить/читать
                        regex = new Regex(mas[i * 2 + 1]);
                        matches = regex.Matches(str);
                        if (matches.Count > 0)
                        {
                            string maxStr = "";
                            for (int k = 0; k < str.Length; k++)
                            {
                                string newStr = "";
                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                //прокручиваем до переменной
                                while ((str[k] == ' ') || (str[k] == '\t') || (str[k] == '('))
                                {
                                    if (k < str.Length)
                                        k++;
                                    else break;
                                }
                                

                                while ((k<str.Length) && (str[k] != ' ') && (str[k] != '+') && (str[k] != '-') && (str[k] != '*') && (str[k] != '/') && (str[k] != '%') && (str[k] != '\t') && (str[k] != ')') && (str[k] != '('))
                                {
                                    newStr += str[k];
                                    k++;
                                }
                                maxStr += searchInList(newStr);
                                if ((searchInList(newStr) == "")&&(maxStr.Length!=0)) maxStr += newStr;

                                if (maxStr != "")
                                {
                                    //прокручиваем до переменной
                                    while ((str[k] == ' ') || (str[k] == '\t') || (str[k] == '(') || (str[k] == ')'))
                                    {
                                        if (k < str.Length)
                                            k++;
                                        else break;
                                    }
                                    if ((str[k] == '+') || (str[k] == '-') || (str[k] == '*') || (str[k] == '/') || (str[k] == '%'))
                                        maxStr += str[k]; 
                                }

                            }

                            bool f = false;
                            for (int j = 1; j < matches.Count; j++)
                                if (matches[j].ToString() != "")
                                {                                    
                                    listStr.Add(new string[] { ident, maxStr });
                                    f = true;
                                    return true;
                                }
                            if (f) break;                            
                        }
                    }
                }
                catch { }
            }
            //не найден
            return false;
        }

        private bool cycleFor(string str, in int i,in string[] Text)
        {
            //для ( цп: счет = 2; счет <= Н; счет++) 
            Regex regex = new Regex(@"для(\s*)\((\s*)([А-Яа-я]*)(\s*):(\s*)[А-Яа-я]+(\w*)(\s*)=(\s*)[0-9]*(\s*);(\s*)[А-Яа-я]+(\w*)(\s*)(<=|>=|!=|==|<|>)+(\s*)[А-Яа-я]+(\w*)(\s*);((\s*)[А-Яа-я]+(\w*)\+\+|(\s*)[А-Яа-я]+(\w*)(\s*)(\+|\-|\*|/)=(\s*)([0-9]*|[А-Яа-я]+(\w*)))\)(\s*)(\{)?"); 
            MatchCollection matches = regex.Matches(str);
            if (matches.Count>0)
            {
                string cycle = matches[0].ToString();
                listStr.Add(new string[] { "цикл", "для" });
                if (!recognition(cycle)) Form1.Str_Write += "Ошибка в обьявлении переменной {"  + str + " }\n";

                regex = new Regex(@"[А-Яа-я]+(\w*)(\s*)(<=|>=|!=|==|<|>)+(\s*)[А-Яа-я]+(\w*)(\s*);");
                matches = regex.Matches(cycle);
                if (matches.Count > 0)
                    cycle = matches[0].ToString();
                else Form1.Str_Write += "Ошибка в обьявлении переменной {" + str + " }\n";

                string sName = ""; //счет
                string signСondition = ""; //<=
                string sign = ""; //Н
                for (int j = 0; j < cycle.Length; j++)
                {
                    if ((cycle[j] == '<') || (cycle[j] == '>') || (cycle[j] == '!') || (cycle[j] == '='))
                    {                        
                        for (int j2 = j; j2 < cycle.Length; j2++)
                        {
                            if ((cycle[j2] != '<') && (cycle[j2] != '>') && (cycle[j2] != '!') && (cycle[j2] != '='))
                            {
                                for (int j3 = j2; j3 < cycle.Length; j3++)
                                    if (cycle[j3] != ';') { if (cycle[j3] != ' ') sign += cycle[j3]; }
                                    else
                                    {
                                        listStr.Add(new string[] { "выполнение", sName, signСondition, sign });
                                        break;
                                    }
                                break;
                            }
                            signСondition += cycle[j2];
                        }
                        break;
                    }
                    if (cycle[j]!=' ') sName += cycle[j];                    
                }
                regex = new Regex(@"((\s*)[А-Яа-я]+(\w*)\+\+|(\s*)[А-Яа-я]+(\w*)(\s*)(\+|\-|\*|/)=(\s*)([0-9]*|[А-Яа-я]+(\w*)))\)(\s*)(\{)?");
                matches = regex.Matches(str);
                if (matches.Count > 0)
                    cycle = matches[0].ToString();
                else Form1.Str_Write += "Ошибка в обьявлении переменной {" + str + " }\n";

                sName = "";
                for (int j = 0; j < cycle.Length; j++)
                {
                    if ((cycle[j] == '+') || (cycle[j] == '-') || (cycle[j] == '*') || (cycle[j] == '/') ||  (cycle[j] == '='))
                    {
                        signСondition = "";
                        for (int j2 = j; j2 < cycle.Length; j2++)
                        {
                            if ((cycle[j2] != '+') && (cycle[j2] != '-') && (cycle[j2] != '*') && (cycle[j2] != '=') && (cycle[j2] != '/'))
                            {
                                sign = "";
                                for (int j3 = j2; j3 < cycle.Length; j3++)
                                    if (cycle[j3] != ')') { if (cycle[j3] != ' ') sign += cycle[j3]; }
                                    else
                                    {
                                        if (sign == "") listStr.Add(new string[] { "изменение", searchInList(sName), strToInt(in signСondition).ToString() });
                                            else listStr.Add(new string[] { "изменение", sName, sign });
                                        return true;
                                    }
                            }
                            if (cycle[j2] != ' ') signСondition += cycle[j2];
                        }
                    }
                    if (cycle[j] != ' ') sName += cycle[j]; 
                }

            }
            //не найден
            return false;
        }

        //поиск уже имеющихся в списке переменных
        //для преобразования их в токен
        private string searchInList(string str)
        {           
            int k = 0;
            foreach (string[] s in listStr)
            {
                switch (s[0])
                {
                    case "id":
                        {
                            if (s[2] == str) return "(id, "+ k.ToString() +")";   
                        }
                        break;
                }
                k++;
            }
            return "";

        }
        //проверка на выражение
        private bool expression(string str)
        {
            //человек = (человек + К) % счет;
            Regex regex = new Regex(@"(\s*)([А-Яа-я]+(\w*))(\s*)[(+=)(-=)(\*=)(/=)(=)]+(\s*)(\(?(\s*)[А-Яа-я]+(\w*)(\s*)[(+)(-)(*)(/)(%)]*(\s*)([А-Яа-я]+(\w*))*(\s*)\)?)*[(+)(-)(*)(/)(%)]*(\s*)([А-Яа-я]+(\w*))*(\s*);(\s*)");            
            MatchCollection matches = regex.Matches(str);
            if (matches.Count > 0)
            {
                string sName = "";
                int i = 0;
                while (i < str.Length)
                {
                    if ((str[i] != '+') && (str[i] != '-') && (str[i] != '*') && (str[i] != '/') && (str[i] != '='))
                    {
                        if (str[i] != ' ')  sName += str[i];
                    }
                    else break;
                    i++;
                }
                //найден
                return true;
            }
            //не найден
            return false;
        }

        private int strToInt(in string s2)
        {
            switch (s2)
            {
                case "++": return 1;  
                case "--": return -1;
            }
            return 0;
        }

        private void switchStr(ref string ident, ref string sname)
        {
            switch (ident)
            {
                case "цп": { listStr.Add(new string[] { "id", ident, sname, "0" }); } break;
                case "сп": { listStr.Add(new string[] { "id", ident, sname, "''" }); } break;
                case "лп": { listStr.Add(new string[] { "id", ident, sname, "ложь" }); } break;
                case "дп": { listStr.Add(new string[] { "id", ident, sname, "0.0" }); } break;
            }
        }               

        private void writeToken()
        {
            Form1.Str_Write += "\nЛексический анализ (вывод токенов).\n";
            Form1.Str_Write += "/********************************/\n";
            //вывод токенов на экран
            foreach (string[] str in listToken)
            {
                for (int i = 0; i < str.Length; i++)
                    switch (str[i])
                    {
                        case "id":
                            {
                                Form1.Str_Write += "(" + str[i] + ", " + str[i + 1] + ") ";
                                i++;
                            }
                            break;
                        case "читать":
                            {                               
                                Form1.Str_Write += str[i] + " (" + str[i + 1] + ", " + str[i + 2] + ") ";
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

        
        private bool getMasToken()
        {
            bool result = false;
            int i = 0;

            foreach (string[] str in listStr)
            {
                switch (str[0])
                {
                    case "id":
                        {
                            listToken.Add(new string[] { "id", i.ToString(), "=", str[3] });
                        }
                        break;
                    case "вывод":
                        {
                            listToken.Add(new string[] { str[0], str[1] });
                        }
                        break;
                    case "читать":
                        {
                            listToken.Add(new string[] { str[0], "id", changeIdent(str[1]) });
                        }
                        break;
                    default: { if (str.Length==1) listToken.Add(new string[] { str[0] });
                               if (str.Length == 2)  listToken.Add(new string[] { str[0] , str[1] });
                               if (str.Length == 3) listToken.Add(new string[] { str[0], str[1], str[2] });
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
                    if (str[i] == s)
                    {
                        return j.ToString();
                    }
                j++;
            }
            Form1.Str_Write += "Переменная не объявлена!\n";
            return null;
        }
        /******************************************************дальше не нужные функции**********************************************************************/




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
    }
}
