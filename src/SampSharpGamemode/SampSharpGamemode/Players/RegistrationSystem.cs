using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode.Events;
using System.Text.RegularExpressions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Definitions;

namespace SampSharpGamemode.Players
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
            var RegErrDialog = new MessageDialog("{f90023}Ошибка регистрации", "{f90023}В пароле использованы недопустимые символы.\n{ffffff}Допускается использование только латинских букв и цифр.", "X");
            var RegSuccess = new MessageDialog("{76ee2b}Успешная регистрация", "{ffffff}Поздравляем! Вы успешно зарегистрировали аккаунт!\nЖелаем приятной игры на нашем сервере!", "X");
            var Confirm = new InputDialog("{76ee2b}Регистрация {FFFFFF}| {76ee2b}Повтор пароля", "{ffffff}Введите пароль еще раз в поле ниже", false, "Ввод");
            var ErrConfirm = new MessageDialog("{f90023}Ошибка повтора пароля", "{ffffff}Введеный вами пароль не совпадает с предыдущим.\nПожалуйста, повторите попытку.", "X");
            var PromoInput = new InputDialog("{76ee2b}Регистрация {ffffff}| {f90023}Промокод", "{FFFFFF}Если у вас есть бонусный код, введите его в поле ниже.\nУказав его, вы получите вознаграждение.", false, "Ввод", "Пропустить");
            var ErrPromo = new MessageDialog("{f90023}Ошибка {ffffff}|{f90023} Промокод", "{FFFFFF}Указанного вами промокода не существует.\nПерепроверьте правильность написания или пропустите ввод.", "X");
            var PromoSucces = new MessageDialog("{76ee2b}Регистрация {ffffff}| {76ee2b}Промокод", "", "X");
            RegPassDialog.Response += (sender, e) =>
            {
                if (e.DialogButton == DialogButton.Left)
                {
                    if (IsPasswordCorrect(e.InputText))
                    {
                        player.PVars[PvarsInfo.pass] = e.InputText;
                        Confirm.Show(player);
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
            Confirm.Response += (sender, s) =>
            {
                if (player.PVars.Get<string>(PvarsInfo.pass) != s.InputText)
                {
                    ErrConfirm.Show(player);
                    player.PVars[PvarsInfo.pass] = "";
                    ErrConfirm.Response += (sender, s) =>
                    {
                        RegPassDialog.Show(player);
                    };
                }
                else
                {
                    player.PVars[PvarsInfo.pass] = s.InputText;
                    player.PVars[PvarsInfo.password] = GameMode.getHash(s.InputText);
                    PromoInput.Show(player);
                }
            };
            RegErrDialog.Response += (sender, e) =>
            {
                RegPassDialog.Show(player);
            };
            RegPassDialog.Show(player);
            PromoInput.Response += (sender, e) =>            
            {
                if (e.DialogButton == DialogButton.Left)
                {
                    player.PVars[PvarsInfo.promocode] = e.InputText;
                    var dbpromo = GameMode.db.SelectPromoByName(e.InputText).data;
                    if (dbpromo.Count > 0)
                    {
                        PromoSucces.Message = $"{{FFFFFF}}Вы указали промокод {{76ee2b}}{dbpromo[0][1]}{{ffffff}}.\nКак только вы отыграете 24 часа, вам будет выдан бонус {{34c924}}{dbpromo[0][2]}${{ffffff}}!";
                        GameMode.db.SetPlayerPromo(player);
                        PromoSucces.Show(player);
                    }
                    else
                    {
                        ErrPromo.Show(player);
                    }
                }
                else
                {
                    RegSuccess.Show(player);
                    GameMode.db.InsertPlayer(player);
                    int uid = int.Parse(GameMode.db.LAST_INSERT_ID().data[0][0]);
                    GameMode.db.UpdateSessions_uid(player.PVars.Get<int>(PvarsInfo.sessionid), uid);
                    player.SendClientMessage(Colors.SUCCESS, "Вы успешно зарегистрировались. Надеемся, вы хорошо проведете время у нас. Приятной игры :)");
                    player.LoadInfo();
                }

            };
            ErrPromo.Response += (sender, e) =>
            {
                PromoInput.Show(player);
            };
            PromoSucces.Response += (sender, e) =>
            {
                RegSuccess.Show(player);
                GameMode.db.InsertPlayer(player);
                int uid = int.Parse(GameMode.db.LAST_INSERT_ID().data[0][0]);
                GameMode.db.UpdateSessions_uid(player.PVars.Get<int>(PvarsInfo.sessionid), uid);
                player.SendClientMessage(Colors.SUCCESS, "Вы успешно зарегистрировались. Надеемся, вы хорошо проведете время у нас. Приятной игры :)");
                player.LoadInfo();
            };
        }
    }
}