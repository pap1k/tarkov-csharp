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
    
    class RegisterationSystem
    {
        //public void RegisterDialogs_Eesponse() DLG_REG_PASSWORD
        private static bool IsPasswordCorrect(string check)
        {
            var reg = new Regex("^[-A-Za-z0-9!@#$^&*()_+[\\];\\\\<>,.\\/?~]{4,20}$");
            return reg.IsMatch(check);
        }
        public static void Start(Player player)
        {
            Console.WriteLine($"Started reg system for {player.Name}");
            var RegPassDialog = new InputDialog("{76ee2b}Регистрация", "{ffffff}Приветствуем вас на нашем сервере. Аккаунт с таким никнеймом {76ee2b}не зарегистрирован{ffffff}.\nДля регистрации вам необходимо указать пароль в поле ниже.\n{f90023}Обращаем ваше внимание на то, что: {ffffff}\n{f90023}•{ffffff} Пароль чувствителен к регистру.\n{f90023}•{ffffff} Длина пароля может быть от 4 до 20 символов.\n{f90023}•{ffffff} Пароль может состоять из латинских букв и цифр.\n", false, "Ввод", "Отмена");
            var RegErrDialog = new MessageDialog("{76ee2b}Ошибка регистрации", "{f90023}В пароле использованы недопустимые символы.\n{ffffff}Допускается использование только латинских букв и цифр.", "X");
            var RegSuccess = new MessageDialog("{76ee2b}Успешная регистрация", "{ffffff}Поздравляем! Вы успешно зарегистрировали аккаунт!\nЖелаем приятной игры на нашем сервере!", "X");

            RegPassDialog.Response += (sender, e) =>
            {
                if (e.DialogButton == DialogButton.Left)
                {
                    if (IsPasswordCorrect(e.InputText))
                    {
                        player.PVars[PvarsInfo.password] = e.InputText;
                        GameMode.db.InsertPlayer(player);
                        RegSuccess.Show(player);
                        player.LoadInfo();
                    }
                    else
                        RegErrDialog.Show(player);
                }
                else
                {
                    player.SendClientMessage(0xfa8500FF, "{fa8500}Аккаунт не зарегистрирован. Введите /q в чат для выхода из игры.");
                    player.kick("noreg");
                }
            };
            RegErrDialog.Response += (sender, e) =>
            {
                RegPassDialog.Show(player);
            };
            RegPassDialog.Show(player);
        }
        
    }
}
