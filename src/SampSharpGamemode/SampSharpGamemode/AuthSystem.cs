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
            var AUTH_DLG = new InputDialog("Авторизация", "\t\t{ffffff}Аккаунт с таким ником зарегистрирован :(\n\tЕсли Вы являетесь владельцем - введите пароль в поле ниже для входа.\nЕсли Вы только хотите начать игру, то, к сожалению, придется придумать другой ник.", true, "Войти", "Отмена");
            var ERROR_DLG = new MessageDialog("Авторизация", "Ошибка", "Выйти");
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
                    player.SendClientMessage("Выйти из игры можно командой /q");
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
            var reg = new Regex("^[-A-Za-z0-9!@#$^&*()_+[\\];\\\\<>,.\\/?~]{3,33}$");
            return reg.IsMatch(check);
        }
    }
}

