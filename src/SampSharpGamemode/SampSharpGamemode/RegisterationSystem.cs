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
            var reg = new Regex("^[-A-Za-z0-9!@#$^&*()_+[\\];\\\\<>,.\\/?~]{3,33}$");
            return reg.IsMatch(check);
        }
        public static void Start(Player player)
        {
            Console.WriteLine($"Started reg system for {player.Name}");
            var RegHiDialog = new MessageDialog("Регистрация", "\t\tПривет!\n\tДля продолжения надо зарегистрироваться.\n\t\tПродолжить?:", "Продолжить", "Отмена");
            var RegPassDialog = new InputDialog("Регистрация", "Для регистрации необходимо придумать уникальный пароль,\nсодержащий латинские символы любого регистар и цифры.\nПароль должен быть диной от 3 - х до 30 - и символов", false, "Продолжить", "Отмена");
            var RegErrDialog = new MessageDialog("Ошибка", "Пароль может содежрать символы любого регистра и цифры.", "OK");
            var RegSuccess = new MessageDialog("Регистрация", "Вы успешно зарегистрировали аккаунт!", "Войти в игру");

            RegHiDialog.Response += (sender, e) =>
            {
                if(e.DialogButton == DialogButton.Left)
                {
                    RegPassDialog.Show(player);
                }
                else
                    RegHiDialog.Show(player);
            };
            RegPassDialog.Response += (sender, e) =>
            {
                if (e.DialogButton == DialogButton.Left)
                {
                    if (IsPasswordCorrect(e.InputText))
                    {
                        player.password = e.InputText;
                        GameMode.db.InsertPlayer(player);
                        RegSuccess.Show(player);
                    }
                    else
                        RegErrDialog.Show(player);
                }
                else
                    player.SendClientMessage("ну тогда ливни");
            };
            RegErrDialog.Response += (sender, e) =>
            {
                RegPassDialog.Show(player);
            };
            RegSuccess.Response += (sender, e) =>
            {
                player.ingame = true;
                player.SendClientMessage("Успешная авторизация! Приятной игры!");
            };

            RegHiDialog.Show(player);
        }
        
    }
}
