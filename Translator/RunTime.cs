using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    class RunTime
    {
        Error_class Error = new Error_class();
        Form1 Head = new Form1();
        string Str_Write = "";

        public string Start(List<Variable> listStr, string Str_Write_local, Queue<int> Queue_vl)
        {
            Str_Write = Str_Write_local;
            int i = 0;
            while (i < listStr.Count)
            {
                switch (listStr[i].iD)
                {
                    case "вывод":
                        {
                            
                            int j = listStr.FindIndex(x => ((x.name == listStr[i].name) && (x.name != null)));
                            if ((j < i) && (j != -1))
                            {
                                if (listStr[j].type == "сп")
                                {
                                    string[] s = listStr[j].value.Split('"');
                                    string st = "";
                                    for (int t = 0; t < s.Length; t++)
                                        if (s[t] != "\"") st += s[t];
                                    Form1.Str_Write_Programm += st + "\n";
                                }
                                else Form1.Str_Write_Programm += listStr[j].value + "\n";
                            }
                            else if (listStr[i].value != null)
                            {
                                Form1.Str_Write_Programm += listStr[i].value + "\n";
                            }
                            else Str_Write += Error.Sintax_Error_Var_notfound(listStr[i].name + " " + listStr[i].value);

                           
                        }
                        break;
                    case "читать":
                        {
                           
                            try
                            {
                                int j = listStr.FindIndex(x => x.name == listStr[i].name );
                                if ( (j < i)&& (j!=-1) )
                                {
                                    listStr[j].value = Form1.listRead[Form1.Count_str_Read];
                                    Form1.Count_str_Read++;
                                }
                                else Str_Write += Error.Sintax_Error_Var_notfound(listStr[i].name + " " + listStr[i].value);
                            }
                            catch { Str_Write += Error.Sintax_Error_RW(listStr[i].iD + " "+ listStr[i].name); }
                        }
                        break;
                    case "для":
                        {
                            if (listStr[i].type != null)
                            {
                                int temp = 0;
                                i++;
                                for (int jj = i; jj < listStr.Count(); jj++)                                
                                    if (listStr[jj].iD == "конец для")
                                    {
                                        temp = jj;
                                        break;
                                    }

                                string[] mas_symbol = listStr[i].value.Split(' ');
                                int j = listStr.FindIndex(x => x.name == mas_symbol[0]);
                                comparison(ref listStr, ref mas_symbol, ref i, ref j, ref temp, ref Queue_vl);                             
                            }
                        }
                        break;
                    case "выражение":
                        {
                            string str = listStr[i].value.Trim(' ', '\t');
                            string[] mas_symbol = str.Split(' ');
                            int k = 0;
                            for (int t = 0; t < mas_symbol.Length; t++)
                            {
                                mas_symbol[t] = mas_symbol[t].Trim(' ');
                                if (mas_symbol[t] == ";") k++;
                            }
                           // if (k!=1) Str_Write += Error.Sintax_Error_TZ(listStr[i].iD + " " + listStr[i].name);



                            int j = listStr.FindIndex(x => x.name == mas_symbol[0]);
                            check_on_equality(ref j,ref listStr,ref mas_symbol, ref str,ref i); 
                        }
                        break;
                    case "конец для":
                        {
                            try
                            {
                                int temp = i;
                                i = Queue_vl.Peek();
                                if (i == -1) i = temp;
                                else
                                {
                                    //увеличть значение в цикле
                                    string str = listStr[i].value.Trim(' ', '\t');
                                    string[] mas_symbol = str.Split(' ');
                                    for (int t = 0; t < mas_symbol.Length; t++)
                                        mas_symbol[t] = mas_symbol[t].Trim(' ');

                                    int j = listStr.FindIndex(x => x.name == mas_symbol[0]);
                                    check_on_equality(ref j, ref listStr, ref mas_symbol, ref str, ref i);
                                    //сравнение
                                    i--;
                                    mas_symbol = listStr[i].value.Split(' ');
                                    j = listStr.FindIndex(x => x.name == mas_symbol[0]);
                                    comparison(ref listStr, ref mas_symbol, ref i, ref j, ref temp, ref Queue_vl);
                                }
                            }
                            catch { Str_Write += Error.Sintax_Error_Not_Cycle(listStr[i].iD); }
                        }
                        break;
                    case "если":
                        {
                            check_if(ref listStr,ref i);                            
                        }
                        break;
                    case "конец если":
                        {
                            if (listStr[i + 1].iD == "иначе")
                            {
                                for (int j = i + 2; j < listStr.Count; j++)
                                    if (listStr[j].iD == "конец иначе") { i = j; break; }
                            } 
                        }
                        break;
                    case "иначе": { i++; } break;
                    case "конец иначе":{}break;
                    default: { } break;
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
            }
        }

        private string[] get_data(List<Variable> listStr, string symbol, int i)
        {
            string[] str = { "", "" };

            int k = listStr.FindIndex(x => x.name == symbol);

            if (k != -1)
                if (k < i)
                    try
                    {
                        switch (listStr[k].type)
                        {
                            case "цп": str[0] = Convert.ToInt32(listStr[k].value).ToString(); str[1] = "цп"; break;
                            case "дп": str[0] = Convert.ToDouble(listStr[k].value).ToString(); str[1] = "дп"; break;
                            case "сп": str[0] = listStr[k].value; str[1] = "сп"; break;
                            case "лп": str[0] = listStr[k].value; str[1] = "лп"; break;
                            default: str[0] = "0"; str[1] = "цп"; break;
                        }
                    }
                    catch { Str_Write += Error.Sintax_Error_expression(listStr[k].type); }
                else Str_Write += Error.Sintax_Error_Var_notfound(listStr[i].value);
            else
            {
                int t = 0;
                try { str[0] = Convert.ToInt32(symbol).ToString(); str[1] = "цп"; return str; }
                catch { t++; }


                try { str[0] = Convert.ToDouble(symbol).ToString(); str[1] = "дп"; return str; }
                catch { t++; }

                try { if ((symbol[0] != '"') && (symbol[symbol.Length - 1] != '"')) { t++; } else { str[0] = symbol; str[1] = "сп"; return str; } }
                catch { t++; }

                if (t == 3) Str_Write += Error.Sintax_Error_expression(listStr[i].value);
            }
            return str;
        }

        private void Act(ref string[] var1, ref string[] var2,string mas_symbol, ref List<Variable> listStr, ref int j)
        {
            if (var1[1] == var2[1])
            {
                switch (mas_symbol)
                {
                    case "+":
                        {
                            if (var1[1] == "цп") listStr[j].value = (Convert.ToInt32(var1[0]) + Convert.ToInt32(var2[0])).ToString();
                            if (var1[1] == "дп") listStr[j].value = (Convert.ToDouble(var1[0]) + Convert.ToDouble(var2[0])).ToString();
                            if (var1[1] == "сп") listStr[j].value = var1[0] + var2[0];
                            if (var1[1] == "лп") Str_Write += Error.Sintax_Error_Type();
                        }
                        break;
                    case "-":
                        {
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
                        }
                        break;
                    case "/":
                        {
                            if (var1[1] == "цп") listStr[j].value = (Convert.ToInt32(var1[0]) / Convert.ToInt32(var2[0])).ToString();
                            if (var1[1] == "дп") listStr[j].value = (Convert.ToDouble(var1[0]) / Convert.ToDouble(var2[0])).ToString();
                            if (var1[1] == "сп") Str_Write += Error.Sintax_Error_Type();
                            if (var1[1] == "лп") Str_Write += Error.Sintax_Error_Type();
                        }
                        break;
                    case "%":
                        {
                            if (var1[1] == "цп") listStr[j].value = (Convert.ToInt32(var1[0]) % Convert.ToInt32(var2[0])).ToString();
                            if (var1[1] == "дп") listStr[j].value = (Convert.ToDouble(var1[0]) % Convert.ToDouble(var2[0])).ToString();
                            if (var1[1] == "сп") Str_Write += Error.Sintax_Error_Type();
                            if (var1[1] == "лп") Str_Write += Error.Sintax_Error_Type();
                        }
                        break;
                }
            }
            else Error.Sintax_Error_Type();
        }

         private void check_on_equality(ref int j,ref List<Variable> listStr,ref string[] mas_symbol, ref string str, ref int i)
         {
            if ((j < i) && (j != -1))
            {
                if (mas_symbol[1] == "++" || mas_symbol[1] == "--") select_zn(str, mas_symbol[1], mas_symbol.Length, j, ref listStr, i);
                else if (mas_symbol[1] == "=")
                {
                    string[] var1 = get_data(listStr, mas_symbol[2], i);
                    string[] var2 = get_data(listStr, mas_symbol[4], i);
                    Act(ref var1, ref var2, mas_symbol[3], ref listStr, ref j);
                }
                else Str_Write += Error.Sintax_Error_expression(str);
            }
            else Str_Write += Error.Sintax_Error_Var_notfound(mas_symbol[0]);
         }

        private void comparison(ref List<Variable> listStr, ref string[] mas_symbol, ref int i, ref int j, ref int temp, ref Queue<int> Queue_lv)
        {
            if ((j < i) && (j != -1))
            {
                string[] var1 = get_data(listStr, mas_symbol[0], i);
                string[] var2 = get_data(listStr, mas_symbol[2], i);               
                try
                {
                    switch (mas_symbol[1])
                    {
                        case "<":
                            {
                                if (Convert.ToInt32(var1[0]) < Convert.ToInt32(var2[0])) i++;
                                else { i = temp; Queue_lv.Dequeue(); }
                            }
                            break;
                        case "<=":
                            {
                                if (Convert.ToInt32(var1[0]) <= Convert.ToInt32(var2[0])) i++;
                                else { i = temp; Queue_lv.Dequeue(); }
                            }
                            break;
                        case ">":
                            {
                                if (Convert.ToInt32(var1[0]) > Convert.ToInt32(var2[0])) i++;
                                else { i = temp; Queue_lv.Dequeue(); }
                            }
                            break;
                        case ">=":
                            {
                                if (Convert.ToInt32(var1[0]) >= Convert.ToInt32(var2[0])) i++;
                                else { i = temp; Queue_lv.Dequeue(); }
                            }
                            break;
                        case "!=":
                            {
                                if (Convert.ToInt32(var1[0]) != Convert.ToInt32(var2[0])) i++;
                                else { i = temp; Queue_lv.Dequeue(); }
                            }
                            break;
                        case "==":
                            {
                                if (Convert.ToInt32(var1[0]) == Convert.ToInt32(var2[0])) i++;
                                else { i = temp; Queue_lv.Dequeue(); }
                            }
                            break;
                    }
                }
                catch { Str_Write += Error.Sintax_Error_expression(listStr[temp].value); }

            }
        }

        private void check_if(ref List<Variable> listStr, ref int i)
        {
            string[] mas_symbol = listStr[i].value.Split(' ');
            string[] var1 = get_data(listStr, mas_symbol[0], i);
            string[] var2 = get_data(listStr, mas_symbol[2], i);
            switch (mas_symbol[1])
            {
                case ">":
                    {
                        if ((var1[1] == var2[1]) && (var1[1] == "цп" || var1[1] == "лп"))
                        {
                            //если не правильно
                            if (Convert.ToInt32(var1[0]) <= Convert.ToInt32(var2[0]))
                                for (int j = i + 1; j < listStr.Count; j++)
                                    if (listStr[j].iD == "конец если")
                                        if (listStr[j + 1].iD == "иначе") { i = j + 1; break; }
                        }
                        else Error.Sintax_Error_Type();
                    }
                    break;
                case ">=":
                    {
                        if ((var1[1] == var2[1]) && (var1[1] == "цп" || var1[1] == "лп"))
                        {
                            //если не правильно
                            if (Convert.ToInt32(var1[0]) < Convert.ToInt32(var2[0]))
                                for (int j = i + 1; j < listStr.Count; j++)
                                    if (listStr[j].iD == "конец если")
                                        if (listStr[j + 1].iD == "иначе") { i = j + 1; break; }
                        }
                        else Error.Sintax_Error_Type();
                    }
                    break;
                case "<":
                    {
                        if ((var1[1] == var2[1]) && (var1[1] == "цп" || var1[1] == "лп"))
                        {
                            //если не правильно
                            if (Convert.ToInt32(var1[0]) >= Convert.ToInt32(var2[0]))
                                for (int j = i + 1; j < listStr.Count; j++)
                                    if (listStr[j].iD == "конец если")
                                        if (listStr[j + 1].iD == "иначе") { i = j + 1; break; }
                        }
                        else Error.Sintax_Error_Type();
                    }
                    break;
                case "<=":
                    {
                        if ((var1[1] == var2[1]) && (var1[1] == "цп" || var1[1] == "лп"))
                        {
                            //если не правильно
                            if (Convert.ToInt32(var1[0]) > Convert.ToInt32(var2[0]))
                                for (int j = i + 1; j < listStr.Count; j++)
                                    if (listStr[j].iD == "конец если")
                                        if (listStr[j + 1].iD == "иначе") { i = j + 1; break; }
                        }
                        else Error.Sintax_Error_Type();
                    }
                    break;
                case "==":
                    {
                        if ((var1[1] == var2[1]) && (var1[1] == "цп" || var1[1] == "лп"))
                        {
                            //если не правильно
                            if (Convert.ToInt32(var1[0]) != Convert.ToInt32(var2[0]))
                                for (int j = i + 1; j < listStr.Count; j++)
                                    if (listStr[j].iD == "конец если")
                                        if (listStr[j + 1].iD == "иначе") { i = j + 1; break; }
                        }
                        else Error.Sintax_Error_Type();
                    }
                    break;
                case "!=":
                    {
                        if ((var1[1] == var2[1]) && (var1[1] == "цп" || var1[1] == "лп"))
                        {
                            //если не правильно
                            if (Convert.ToInt32(var1[0]) == Convert.ToInt32(var2[0]))
                                for (int j = i + 1; j < listStr.Count; j++)
                                    if (listStr[j].iD == "конец если")
                                        if (listStr[j + 1].iD == "иначе") { i = j + 1; break; }
                        }
                        else Error.Sintax_Error_Type();
                    }
                    break;
            }
        }

    }
}