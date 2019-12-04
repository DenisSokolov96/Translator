using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    class RunTime
    {
        Error_class Error = new Error_class();
        Form1 Head = new Form1();
        string Str_Write = "";
        public string Start(List<Variable> listStr, string Str_Write_local, Stack<int> Stack_vl)
        {
            Str_Write = Str_Write_local;
            int i = 0;
            while (i<listStr.Count)
            {
                switch (listStr[i].iD)
                {                    
                    case "вывод":
                        {
                            int j = listStr.FindIndex(x => ( (x.name == listStr[i].name)&&(x.name!=null) ));
                            if ((j < i)&&(j!=-1)) Form1.Str_Write_Programm += listStr[j].value + "\n";                            
                            else if (listStr[i].value!="") Form1.Str_Write_Programm += listStr[i].value + "\n";
                                 else Str_Write += Error.Sintax_Error_Var_notfound(listStr[i].name +" "+ listStr[i].value);                            
                        }
                        break;
                    case "читать":
                        {
                            int j = listStr.FindIndex(x => x.name == listStr[i].name);
                            if (j < i)
                            {
                                listStr[j].value = Form1.listRead[Form1.Count_str_Read];
                                Form1.Count_str_Read++;                                
                            }
                            else Str_Write += Error.Sintax_Error_Var_notfound(listStr[i].name+ " " + listStr[i].value);                            
                        }
                        break;
                    case "для":
                        {
                            //i = Run_Cycle(str_new, Stack_vl, listStr, i);
                        }
                        break;
                    case "выражение"://нет проверки типов и скобочной структуры, а так же выражеия только с одним знаком
                        {                            
                            string str = listStr[i].value.Trim(' ', '\t');
                            string[] mas_symbol = str.Split(' ');
                            for (int t = 0; t < mas_symbol.Length; t++)
                                mas_symbol[t] = mas_symbol[t].Trim(' ');

                            int j = listStr.FindIndex(x => x.name == mas_symbol[0]);
                            if ((j < i) && (j != -1))
                            {
                                select_zn(str, mas_symbol[1], mas_symbol.Length, j, ref listStr, i);
                                if (mas_symbol[1] == "=")
                                {
                                    string[] var1 = get_data(listStr, mas_symbol[2], i);
                                    string[] var2 = get_data(listStr, mas_symbol[4], i);


                                    if (var1[1] == var2[1])
                                    {
                                        switch (mas_symbol[3])
                                        {
                                            case "+":
                                                {
                                                   if (var1[1]=="цп")  listStr[j].value = (Convert.ToInt32(var1[0]) + Convert.ToInt32(var2[0])).ToString();
                                                   if (var1[1] == "дп") listStr[j].value = (Convert.ToDouble(var1[0]) + Convert.ToDouble(var2[0])).ToString();
                                                   if (var1[1] == "сп") listStr[j].value = var1[0] + var2[0];
                                                   if (var1[1] == "лп") Str_Write += Error.Sintax_Error_Type();
                                                }
                                                break;
                                                 case "-": {
                                                    if (var1[1] == "цп") listStr[j].value = (Convert.ToInt32(var1[0]) - Convert.ToInt32(var2[0])).ToString();
                                                    if (var1[1] == "дп") listStr[j].value = (Convert.ToDouble(var1[0]) - Convert.ToDouble(var2[0])).ToString();
                                                    if (var1[1] == "сп") Str_Write += Error.Sintax_Error_Type();
                                                    if (var1[1] == "лп") Str_Write += Error.Sintax_Error_Type();
                                                }
                                                break;
                                                 case "*":
                                                {
                                                    if (var1[1] == "цп") listStr[j].value = (Convert.ToInt32(var1[0]) * Convert.ToInt32(var2[0])).ToString();
                                                    if (var1[1] == "дп") listStr[j].value = (Convert.ToDouble(var1[0]) * Convert.ToDouble(var2[0])).ToString();
                                                    if (var1[1] == "сп") Str_Write += Error.Sintax_Error_Type();
                                                    if (var1[1] == "лп") Str_Write += Error.Sintax_Error_Type();
                                                } break;
                                                 case "/":
                                                {
                                                    if (var1[1] == "цп") listStr[j].value = (Convert.ToInt32(var1[0]) / Convert.ToInt32(var2[0])).ToString();
                                                    if (var1[1] == "дп") listStr[j].value = (Convert.ToDouble(var1[0]) / Convert.ToDouble(var2[0])).ToString();
                                                    if (var1[1] == "сп") Str_Write += Error.Sintax_Error_Type();
                                                    if (var1[1] == "лп") Str_Write += Error.Sintax_Error_Type();
                                                } break;
                                                 case "%":
                                                {
                                                    if (var1[1] == "цп") listStr[j].value = (Convert.ToInt32(var1[0]) % Convert.ToInt32(var2[0])).ToString();
                                                    if (var1[1] == "дп") listStr[j].value = (Convert.ToDouble(var1[0]) % Convert.ToDouble(var2[0])).ToString();
                                                    if (var1[1] == "сп") Str_Write += Error.Sintax_Error_Type();
                                                    if (var1[1] == "лп") Str_Write += Error.Sintax_Error_Type();
                                                } break;
                                        }
                                    }
                                    else Error.Sintax_Error_Type();



                                }
                            }
                            else Str_Write += Error.Sintax_Error_Var_notfound(mas_symbol[0]);                            
                            
                        }
                        break;                        
                    default:{} break;      
                }
                i++;
            }
            return Str_Write;
        }

        private void select_zn(string head_str, string mas_symbol, int n, int j, ref List<Variable> listStr, int i)
        {
            switch (mas_symbol)
            {
                case "++": {
                        if (n > 2) Str_Write += Error.Sintax_Error_expression(head_str);
                        else if (listStr[j].type == "цп") listStr[j].value = (Convert.ToInt32(listStr[j].value) + 1).ToString();
                        else Str_Write += Error.Sintax_Error_Type();
                    } break;
                case "--":
                    {
                        if (n > 2) Str_Write += Error.Sintax_Error_expression(head_str);
                        else if (listStr[j].type == "цп") listStr[j].value = (Convert.ToInt32(listStr[j].value) - 1).ToString();
                        else Str_Write += Error.Sintax_Error_Type();
                    }
                    break;
                case "=":{}break;
                default: { Str_Write += Error.Sintax_Error_expression(head_str); } break;
            }
        }

        private string[] get_data(List<Variable> listStr, string symbol, int i)
        {
            string[] str = { "", ""};

            int k = listStr.FindIndex(x => x.name == symbol);  

            if (k != -1)
            {
                if (k < i)
                {
                    switch (listStr[k].type)
                    {
                        case "цп": str[0] = Convert.ToInt32(listStr[k].value).ToString(); str[1] = "цп"; break;
                        case "дп": str[0] = Convert.ToDouble(listStr[k].value).ToString(); str[1] = "дп"; break;
                        case "сп": str[0] = listStr[k].value; str[1] = "сп"; break;
                        case "лп": str[0] = listStr[k].value; str[1] = "лп"; break;
                    }
                    
                }
                else Str_Write += Error.Sintax_Error_Var_notfound(listStr[i].value);
            }
            else
            {
                int t = 0;
                try { str[0] = Convert.ToInt32(symbol).ToString(); str[1] = "цп"; return str; }
                catch { t++;  }


                try { str[0] = Convert.ToDouble(symbol).ToString(); str[1] = "дп"; return str; }
                catch { t++; }

                try { if ((symbol[0] != '"') && (symbol[symbol.Length - 1] != '"')) { t++; } else { str[0] = symbol; str[1] = "сп"; return str; } }
                catch { t++; }
                
                if (t == 3 ) Str_Write += Error.Sintax_Error_expression(listStr[i].value);
            }           

            return str;
        }

        private int Run_Cycle(string[] str_new, Stack<int> Stack_vl, List<string[]> listStr, int i)
        {
            i++;
            //заголовок цикла
            string[] str_head;
            str_head = str_new;

            str_new = listStr[i];
            str_new[0].Trim(' ');
            if (str_new[0] != "{")
                Str_Write += Error.Sintax_Error_For_Body(str_new[0]);
            else
            {
                i++;
                str_new = listStr[i];
                str_new[0].Trim(' ');
            }


            return i;
        }

        private void ChangeID(string[] str_new, int element)
        {
            //id тип имя_переменная значение - str_old
            //id тип имя_переменная значение - str_new
            string[] str_old = Form1.listRunTime[element];

            //проверка на типы
            if (str_old[1] == str_new[1])
            {
                //проверка на имена
                if (str_old[2] == str_new[2]) {
                    switch (str_old[0])
                    {
                        case "цп": {
                                str_old[3] = (Convert.ToInt32(str_old[3]) + Convert.ToInt32(str_new[3])).ToString();                                
                                break; }
                        case "сп":
                            {
                                str_old[3] += str_new[3];
                                break;
                            }
                        case "лп":
                            {
                                if (str_old[3] == "истина") str_old[3] = "1";
                                else str_old[3] = "0";

                                if (str_new[3] == "истина") str_new[3] = "1";
                                else str_new[3] = "0";
                                str_old[3] = ( (Convert.ToInt32(str_old[3]) + Convert.ToInt32(str_new[3]))%2 ).ToString();
                                break;
                            }
                        case "дп":
                            {
                                str_old[3] = (Convert.ToDouble(str_old[3]) + Convert.ToDouble(str_new[3])).ToString();
                                break;
                            }
                    }
                }
            }
            else Str_Write += Error.Sintax_Error_Type();

            //обновляем 
            Form1.listRunTime[element] = str_old;
        }
    }
}