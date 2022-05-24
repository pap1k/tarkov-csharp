using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode.Events;
using System.Text.RegularExpressions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Definitions;

namespace SampSharpGamemode
{
    public class AuthSystem
    {
        public static void Start(Player player)
        {
            DBType ret = new DBType();
            var AUTH_DLG = new InputDialog("{76ee2b}Авторизация", "{FFFFFF}Приветствуем вас на нашем сервере. Аккаунт с никнеймом " + player.Name + " {f90023}зарегистрирован{FFFFFF}.\nДля авторизации вам необходимо ввести свой пароль в поле ниже.\nЕсли вы {76ee2b}не являетесь {FFFFFF}владельцем аккаунта, то покиньте сервер, нажав на кнопку {fa8500}Отмена {FFFFFF}или введя {fa8500}/q {FFFFFF}в чат.\nЕсли вы {f90023}забыли пароль{FFFFFF}, то введите {fa8500}RECOVERY{FFFFFF} в строку ввода пароля.", true, "Ввод", "Отмена");
            var ERROR_DLG = new MessageDialog("{f90023}Ошибка авторизации", "\t\t\t\t\t\t{f90023}Вы ввели неверный пароль.\n{FFFFFF}Пожалуйста, проверьте регистр или раскладку.\nЕсли вы забыли пароль, то при наличии привязок, вы можете его восстановить, введя {fa8500}RECOVERY {FFFFFF}в строку ввода пароля.", "X");
            AUTH_DLG.Response += (sender, e) =>
            {
                if (e.DialogButton == DialogButton.Left)
                {
                    if (IsPasswordCorrect(e.InputText))
                    {
                        var dbresult = GameMode.db.CheckAuth(player.Name, e.InputText);
                        if (dbresult.data.Count > 0)
                        {
                            player.SendClientMessage("Вы успешно авторизовались!");
                            player.LoadInfo();
                        }
                        else
                            ERROR_DLG.Show(player);

                    }
                    else
                        ERROR_DLG.Show(player);
                }
                else
                {
                    player.SendClientMessage(0xfa8500FF, "{fa8500}Не удалось войти в аккаунт. Введите /q в чат для выхода из игры.");
                    player.kick("nologin");
                }
            };
            ERROR_DLG.Response += (sender, e) =>
            {
                AUTH_DLG.Show(player);
            };
            AUTH_DLG.Show(player);
        }
        private static bool IsPasswordCorrect(string check)
        {
            var reg = new Regex("^[-A-Za-z0-9!@#$^&*()_+[\\];\\\\<>,.\\/?~]{4,20}$");
            return reg.IsMatch(check);
        }
    }
}

