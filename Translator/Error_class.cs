using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Translator
{
    class Error_class
    {
        public string Sintax_Error_Iden(string str)
        {
            return "Синтаксическая ошибка в объявлении переменной: " + str + "\n";
        }
        public string Sintax_Error_RW(string str)
        {
            return "Синтаксическая ошибка в операторе ввода/вывода: " + str + "\n";
        }
        public string Sintax_Error_Type()
        {
            return "Ошибка - Несовпадение типов!\n";
        }
        public string Sintax_Error_For(string str)
        {
            return "Ошибка в описании цикла: " + str + "\n";
        }
        public string Sintax_Error_For_Body(string str)
        {
            return "Ошибка в теле цикла: " + str + "\n";
        }
        public string Sintax_Error_Var_notfound(string str)
        {
            return "Переменная не найдена: " + str + "\n";
        }
        public string Sintax_Error_expression(string str)
        {
            return "Ошибка в строке: " + str + "\n";
        }
        public string Sintax_Error_if_else(string str)
        {
            return "Ошибка в условии: " + str + "\n";
        }
        public string Sintax_Error_Not_Cycle(string str)
        {
            return "Цикл не найден: " + str + "\n";
        }
        public string Sintax_Error_Text(string str)
        {
            return "Найден не распознанный текст: " + str + "\n";
        }
        public string Sintax_Error_TZ(string str)
        {
            return "Ошибка с запятой:"+str+"\n";
        }
    }
}