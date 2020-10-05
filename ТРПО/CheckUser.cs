using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEP
{
    static class CheckUser
    {
        public static string CheckUserLogin(string str)
        {
            if (str == string.Empty)
            {
                return "Вы не ввели логин";
            }
            else
            {
                if (str.Length > 2 && str.Length <= 20)
                {
                    char[] loginArray = str.ToCharArray();
                    for (int i = 0; i < loginArray.Length; i++)
                    {
                        if (!char.IsLetter(loginArray[i]) && !char.IsDigit(loginArray[i]) && loginArray[i] != '_')
                        {
                            return "Вы указали в логине недопустимые символы.";
                        }
                    }
                }
                else
                {
                    return "Допустимая длина логина 3-20 символов.";
                }
            }
            return str;
        }

        public static string CheckUserPassword(string str)
        {
            if (str == string.Empty)
            {
                return "Вы не ввели пароль";
            }
            else
            {
                if (str.Length > 2 && str.ToString().Length <= 20)
                {
                    char[] passwordArray = str.ToCharArray();
                    for (int i = 0; i < passwordArray.Length; i++)
                    {
                        if (!char.IsLetter(passwordArray[i]) && !char.IsDigit(passwordArray[i]) && passwordArray[i] != '_' && passwordArray[i] != '*')
                        {
                            return "Вы указали в пароле недопустимые символы.";
                        }
                    }
                }
                else
                {
                    return "Допустимая длина пароля 3-20 символов.";
                }
            }
            return str;
        }

        public static string CheckUserName(string str)
        {
            if (str == string.Empty)
            {
                return "Имя не может быть пустым.";
            }
            else
            {
                if (str.Length > 2 && str.Length <= 20)
                {
                    char[] nameArray = str.ToCharArray();
                    for (int i = 0; i < nameArray.Length; i++)
                    {
                        if (!char.IsLetter(nameArray[i]))
                        {
                            return "Вы указали в имени недопустимые символы.";
                        }
                    }
                }
                else
                {
                    return "Допустимая длина имени 3-20 символов.";
                }
            }
            return str;
        }

        public static string CheckUserSurname(string str)
        {
            if (str == string.Empty)
            {
                return "Фамилия не может быть пустой.";
            }
            else
            {
                if (str.Length > 2 && str.Length <= 20)
                {
                    char[] surnameArray = str.ToCharArray();
                    for (int i = 0; i < surnameArray.Length; i++)
                    {
                        if (!char.IsLetter(surnameArray[i]))
                        {
                            return "Вы указали в фамилии недопустимые символы.";
                        }
                    }
                }
                else
                {
                    return "Допустимая длина фамилии 3-20 символов.";
                }
            }
            return str;
        }

        public static string CheckUserStatus(string str)
        {
            if (str == string.Empty)
            {
                return "Статус не может быть пустым.";
            }
            else
            {
                if (str.Length > 3 && str.Length <= 5)
                {
                    if (str != "true" && str != "false")
                    {
                        return "Статус может быть только 'true' или 'false'.";
                    }
                }
                else
                {
                    return "Вы указали неверный статус";
                }
            }
            return str;
        }
    }
}
