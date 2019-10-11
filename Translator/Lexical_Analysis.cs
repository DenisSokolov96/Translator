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
         * ; важные только при объявлении переменных, а остальные нет
         */      
        //списки для резервированных слов
        List<string> identPeremen = new List<string>() { "цп", "сп", "лп", "дп" };
        List<string> identReadWrite = new List<string>() { "вывод", "читать", "вывод" };
        List<string> identCondition = new List<string>() { "Если", "то", "иначе" };
        Error_class Error = new Error_class();
        Form1 Head = new Form1();
        Queue<int> Queue_vl = new Queue<int>();
        List<Variable> listStr = new List<Variable>();
        /*---------------------------------------------------*/


        public string Start_Analysis(string[] Text)
        {
            //Обнуление строки информации
            Head.Str_Write = "";

            if (Search_Programm(Text) == 1) //поиск - программа            
                if (Search_Func_Main(Text) == 1) //поиск - главная функция                
                    if (Read_Str_Text(Text) == 1)
                    {
                        //writeToken();
                        RunTime runtime = new RunTime();
                        return runtime.Start(listStr, Head.Str_Write, Queue_vl);                        
                    }
            return Head.Str_Write;
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
                Head.Str_Write += "Неправильная конструкция: Программа <Имя>;\n**********************\n";
                return 0;
            }
            else
            {
                //listStr.Add(new Variable { value = str_name });
                return 1;
            };
        }

        private int Search_Func_Main(string[] Text)
        {
            int k = 0;
            int rang2 = 0;

            foreach (string str in Text)
            {

                Regex regex = new Regex(@"(\s*)Главная(\s*)\((\s*)\)(\{*)");
                MatchCollection matches = regex.Matches(str);
                k += matches.Count;
                if (k > 0)
                {
                    rang2 = Check_Scob2(str, rang2);
                }
            }

            if (rang2 != 0)
            {
                Head.Str_Write += "Скобочная структура ( ) не верна.\n**********************\n";
                return 0;
            }

            if (k != 1)
            {
                Head.Str_Write += "Не найдена главная функция.\n**********************\n";
                return 0;
            }
            else return 1;
            
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
                                if (!if_else(str))
                                {
                                    string s = str.Trim(' ', '\t');
                                    switch (s)
                                    {
                                        case "Главная ()": { } break;
                                        case "конец главная":
                                        case "конец для":
                                        case "конец если":
                                        case "конец иначе":
                                            {
                                                string[] mas_symbol = s.Split(' ');
                                                listStr.Add(new Variable { iD = s });
                                            }
                                            break;
                                        case "\t":
                                        case " ":
                                        case "":
                                        case "\n": break;
                                        default: {
                                                string[] s2 = s.Split(' ');
                                                if (s2.Length > 0 && s2[0] == "Программа") { }
                                                else Head.Str_Write += Error.Sintax_Error_Text(s); }break;
                                    }
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
            string[] mas = new string[] { @"(\s)*:(\s)*([А-Я]||[а-я])(([А-Я]||[а-я])*)*(\s)*(=(\s)*(-)?([0-9])+(\s)*(;))", @"[-]?([0-9])*",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я])*)*(\s)*(=(\s)*[""]((\s)*(\w)*)*[""](\s)*)", @"(\s)*[""]((\s)*(\w)*)*[""](\s)*",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я])*)*(\s)*=(\s)*правда|ложь(\s)*;", @"правда|ложь",
                                          @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я])*)*(\s)*=(\s)*([0-9]+).([0-9]+)(\s)*;", @"([0-9]+).([0-9]+)"};
            string[] masNotChange = new string[] { @"(\s*):(\s*)([А-Я]||[а-я])(([А-Я]||[а-я])*)*(\s)*(;)", @":(\s)*([А-Я]|[а-я])([А-Я]|[а-я])*" };
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
                        try
                        {
                            string[] mas_symbol = str.Split(':', ';', ' ', '\t');
                            if (mas_symbol.Length > 6)
                            {
                                listStr.Add(new Variable {iD = "id", type = mas_symbol[1], name = mas_symbol[3], value = mas_symbol[5] });
                                return true;
                            }
                            else if (mas_symbol.Length == 5)
                                 {
                                    if (mas_symbol[1]!= "сп") listStr.Add(new Variable { iD = "id", type = mas_symbol[1], name = mas_symbol[3], value = "0" });
                                    else listStr.Add(new Variable { iD = "id", type = mas_symbol[1], name = mas_symbol[3], value = "" });
                                    return true;
                                 }
                            else Head.Str_Write += Error.Sintax_Error_Iden(str);
                            
                        }
                        catch { Head.Str_Write += Error.Sintax_Error_Iden(str); }
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
                        try
                        {
                            str = str.Trim(' ', '\t');
                            string[] mas_symbol = str.Split('(', ')');
                            for (int t = 0; t < mas_symbol.Length; t++)
                                mas_symbol[t] = mas_symbol[t].Trim(' ');

                            if (mas_symbol.Length == 3)
                            {
                                if (mas_symbol[0] == "читать") listStr.Add(new Variable { iD = mas_symbol[0], name = mas_symbol[1] });
                                else
                                {//вывод
                                    string[] s = mas_symbol[1].Split('"');
                                    if (s.Length==1) listStr.Add(new Variable { iD = mas_symbol[0], name = mas_symbol[1] });
                                    else 
                                        if (s.Length == 3) listStr.Add(new Variable { iD = mas_symbol[0], value = s[1] });
                                           else  Head.Str_Write += Error.Sintax_Error_RW(str);
                                    
                                }
                                return true;
                            }
                            else Head.Str_Write += Error.Sintax_Error_RW(str);
                        }
                        catch { Head.Str_Write += Error.Sintax_Error_RW(str); }                       
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
            //[для][цп: счет = 2][счет <= Н][счет ++][ ]
            str = str.Trim(' ', '\t');
            string[] mas_symbol = str.Split('(', ')',';');
            for (int t = 0; t < mas_symbol.Length; t++)
                mas_symbol[t] = mas_symbol[t].Trim(' ');
            if (mas_symbol[0]=="для")
            {
                //[цп][счет][2]
                string[] mas_id = mas_symbol[1].Split(':', '=');
                if (mas_id.Length != 3) Head.Str_Write += Error.Sintax_Error_Iden(mas_symbol[1]);
                for (int t = 0; t < mas_id.Length; t++)
                    mas_id[t] = mas_id[t].Trim(' ');

                //[счет][<=][Н]
                string[] mas_do = mas_symbol[2].Split(' ');
                if (mas_do.Length != 3) Head.Str_Write += Error.Sintax_Error_For(mas_symbol[2]);
                for (int t = 0; t < mas_do.Length; t++)
                    mas_do[t] = mas_do[t].Trim(' ');

                //[счет][++] //[счет][=][счет][+][5/счет]
                string[] mas_change = mas_symbol[3].Split(' ');
                if ((mas_change.Length != 2)&&(mas_change.Length != 5)) Head.Str_Write += Error.Sintax_Error_Iden(mas_symbol[3]);
                for (int t = 0; t < mas_change.Length; t++)
                    mas_change[t] = mas_change[t].Trim(' ');
                try
                {
                    if (mas_change.Length == 2)
                    {
                        listStr.Add(new Variable { iD = "для", type = mas_id[0], name = mas_id[1], value = mas_id[2] });
                        listStr.Add(new Variable { iD = "для", value = mas_do[0] + " " + mas_do[1] + " " + mas_do[2] });
                        listStr.Add(new Variable { iD = "для", value = mas_change[0] + " " + mas_change[1] });
                    }
                    else if (mas_change.Length == 5)
                    {
                        listStr.Add(new Variable { iD = "для", type = mas_id[0], name = mas_id[1], value = mas_id[2] });
                        listStr.Add(new Variable { iD = "для", value = mas_do[0] + " " + mas_do[1] + " " + mas_do[2] });
                        listStr.Add(new Variable { iD = "для", value = mas_change[0] + " " + mas_change[1] + " " + mas_change[2] + " " + mas_change[3] + " " + mas_change[4] });
                    }
                    else Head.Str_Write += Error.Sintax_Error_For(mas_symbol[3]);
                }
                catch { Head.Str_Write += Error.Sintax_Error_Not_Cycle(str); }
                Queue_vl.Enqueue(listStr.Count-1);
                return true;
            }
            //не найден
            return false;
        }
      
        //проверка на выражение
        private bool expression(string str)
        {
            //человек = (человек + К) % счет;
            str = str.Trim(' ', '\t', ';');
            string[] mas_symbol = str.Split(' ');
            for (int t = 0; t < mas_symbol.Length; t++)
                mas_symbol[t] = mas_symbol[t].Trim(' ');
            if (mas_symbol[0] == "Б") { }
            if (  ((mas_symbol.Length>1)&&( (mas_symbol[1] == "++") || (mas_symbol[1] == "--")))||(mas_symbol.Length > 2)   )
            {
                //поиск объявляемых идентификаторов
                for (int t = 0; t < identPeremen.Count; t++)
                {
                    if (mas_symbol[0] == identPeremen[t])
                    {
                        recognition(str);
                        return true;
                    }
                }

                //выражение [человек][=][(][человек][+][К][)][%][счет][;]
                if ( (mas_symbol[1]=="=")||(mas_symbol[1] == "++")||(mas_symbol[1] == "--") )
                {
                    listStr.Add(new Variable { iD = "выражение", value = str });
                    return true;
                }
                //все лишние символы не рассматриваем
            }
            //не найден
            return false;
        }         

        //рроверка на условие
        private bool if_else(string str)
        {
            //если ( П < Н )
            //  П++
            //конец если
            //иначе 
            // 
            //конец иначе
            str = str.Trim(' ','\t');
            string[] mas_symbol = str.Split();
            if (mas_symbol[0] == "если")
            {
                if (mas_symbol[1] == "(" && mas_symbol[5] == ")")
                {

                    listStr.Add(new Variable { iD = mas_symbol[0], value = mas_symbol[2] + " " + mas_symbol[3] + " " + mas_symbol[4] });
                    return true;
                }
                else Head.Str_Write += Error.Sintax_Error_if_else(str);
            }

            if (mas_symbol[0] == "иначе")
            {
                if (mas_symbol.Length == 1)
                {
                    listStr.Add(new Variable { iD = mas_symbol[0] });
                    return true;
                }
                else Head.Str_Write += Error.Sintax_Error_if_else(str);
            }
            //не найден
            return false;
        }

        private void writeToken()
        {
            Head.Str_Write += "\nВывод внутренних данных.\n";
            Head.Str_Write += "/********************************/\n";
            //вывод токенов на экран
            foreach (Variable aVar in listStr)
            {
                Head.Str_Write += aVar.iD + " "+ aVar.type +" " + aVar.name + " " + aVar.value;
                Head.Str_Write += "\n";                
            }
            Head.Str_Write += "/********************************/\n";
        }        
    }
}