using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEP
{
    static class CheckCurrency
    {
        public static string CheckCurrencyName (string str)
        {
            if (str == string.Empty)
            {
                return "Название валюты не может быть пустым.";
            }
            else
            {
                if (str.Length > 2 && str.Length <= 30)
                {
                    char[] sparePartArray = str.ToCharArray();
                    for (int i = 0; i < sparePartArray.Length; i++)
                    {
                        if (!char.IsLetter(sparePartArray[i]) && sparePartArray[i] != '(' && sparePartArray[i] != ')' && sparePartArray[i] != ' ')
                        {
                            return "Вы указали в названии валюты недопустимые символы.";
                        }
                    }
                }
                else
                {
                    return "Допустимая длина названия валюты 3-30 символов.";
                }
            }
            return str;
        }

        public static string CheckCurrencyCourse (string str)
        {
            if (str == string.Empty)
            {
                return "Курс не может быть пустым.";
            }
            else
            {
                char[] costArray = str.ToCharArray();
                for (int i = 0; i < costArray.Length; i++)
                {
                    if ((!char.IsDigit(costArray[i]) && costArray[i] != ','))
                    {
                        return "Вы указали в курсе недопустимые символы.";
                    }
                }
                if (Convert.ToDouble(str) < 0 || Convert.ToDouble(str) > 10)
                {
                    return "Введен неверный курс. Нужно ввести от 0 до 10.";
                }
            }
            return str;
        }
    }
}