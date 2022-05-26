using SampSharp.GameMode.Display;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharpGamemode.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampSharpGamemode.Admins
{
    class TotpSystem
    {
        static string temptotp = "TEMPTOTP",
                      totpcreate = "totpcreate";
        [Command("totptest", UsageMessage = "/totptest [код]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totptest(BasePlayer sender, string text)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            sender.SendClientMessage(Security.TOTP.Get(text));
        }
        [Command("totpshow", UsageMessage = "/totpshow [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totpshow(BasePlayer sender, BasePlayer t)
        {
            string code = t.PVars.Get<string>(temptotp);
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else if (t.Id == sender.Id && sender.PVars.Get<int>(PvarsInfo.adminlevel) < (int)e_AdminLevels.A_FOUNDER)
                sender.SendClientMessage(Colors.GREY, "Вы не можете продемонстрировать TOTP токен самому себе. Обратитесь к старшему администратору.");
            else if (code != "no")
            {
                var dlg = new MessageDialog("{24e302}Введите код в приложение", "{ffffff}Все символы заглавные и в анлийской раскладке:{fbec5d} " + code, "X");
                dlg.Show(t);
                t.PVars[totpcreate] = true;
                foreach (var adm in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                    adm.SendClientMessage(Colors.RED, $"Администратор: {sender.Name} продемонстрировал {t.Name} его временный админ TOTP токен.");
                {
                    if (!t.PVars.Get<bool>(PvarsInfo.admin))
                        t.SendClientMessage(Colors.RED, $"Администратор: {sender.Name} продемонстрировал вам ваш временный TOTP токен.");
                }
            }
            else
                sender.SendClientMessage(Colors.GREY, $"Этот игрок не является администратором и не проходит настройку админ токена.");
        }
        [Command("totpgen", UsageMessage = "/totpgen [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totpgen(BasePlayer sender, BasePlayer t)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            string code = Security.TOTP.GenerateKey();
            t.PVars[temptotp] = code;
            sender.SendClientMessage($"Вы сгенерировали админ TOTP токен для аккаунта {{abcdef}}{t.Name}{{ffffff}}.");
            t.SendClientMessage($"Администратор {{abcdef}}{sender.Name} {{ffffff}}сгенерировал админ TOTP токен для вашего аккаунта.");
        }
        [Command("atotp")]
        private static void CMD_atotp(BasePlayer sender)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            if (!sender.PVars.Get<bool>(totpcreate))
            {
                sender.SendClientMessage("Вы не проходите процеруду подтверждения админ ТОТP токена.");
                return;
            }
            var dlg = new InputDialog("{ffffff}Проверка правильности TOTP токена", "{ffffff}Введите 6 значный код из приложения", false, "Ввод", "Отмена");
            dlg.Response += (_, e) =>
            {
                if(e.DialogButton == DialogButton.Left)
                {
                    bool o = int.TryParse(e.InputText, out int _);
                    if (o)
                    {
                        if (Security.TOTP.Get(sender.PVars.Get<string>(temptotp)) == e.InputText)
                        {
                            sender.SetChatBubble("Ввел корректный TOTP код!", Colors.GREEN, 10f, 10 * 1000);
                            var dlg_truetotp = new MessageDialog(" ", "{24e302}Вы ввели верный TOTP код! Сообщите об этом администратору.", "X");
                            dlg_truetotp.Show(sender);
                            sender.PVars[PvarsInfo.totpkey] = sender.PVars.Get<string>(temptotp);
                            GameMode.db.UpdatePlayerTotp(sender);
                            sender.PVars.Delete(totpcreate);
                        }
                        else
                        {
                            sender.SetChatBubble("Введен неправильный TOTP код.", Colors.RED, 10f, 10 * 1000);
                            var dlg_falsetotp = new MessageDialog(" ", "{ff6347}Введен неправильный TOTP код. Пожалуйста, повторите попытку.", "X");
                            dlg_falsetotp.Show(sender);
                        }
                    }
                }
                else
                    sender.SetChatBubble("Закрыл окно проверки TOTP токена", Colors.GREY, 10f, 10 * 1000);
            };
            sender.SetChatBubble("Открыл окно проверки TOTP токена", -1, 10f, 10 * 1000);
            dlg.Show(sender);
        }
        [Command("fucklast", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_fucklast(BasePlayer sender)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            GameMode.db.UpdatePlayerLastIP(sender, true);
            sender.SendClientMessage("OK");
        }
        [Command("totpflush", UsageMessage = "/totpflush [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totpflush(BasePlayer sender, BasePlayer p)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            p.PVars[PvarsInfo.totpkey] = "no";
            GameMode.db.UpdatePlayerTotp(p);
            sender.SendClientMessage($"Вы сбросили TOTP токен игроку {{abcdef}}{p.Name}{{ffffff}}.");
            p.SendClientMessage($"Администратор {{abcdef}}{sender.Name} {{ffffff}}сбросил ваш TOTP токен.");
        }
        [Command("forcetotp", UsageMessage = "/forcetotp [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_forcetotp(BasePlayer sender, Player target)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            if ((e_AuthState)target.PVars.Get<int>(PvarsInfo.authstate) == e_AuthState.TOTP)
            {
                foreach (var p in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                    p.SendClientMessage(Colors.RED, $"Администратор: {sender.Name} позволил {target.Name} войти в аккаунт без TOTP аутентификации.");
                target.SendClientMessage(Colors.RED, $"Администратор: {sender.Name} позволил вам войти в аккаунт без TOTP аутентификации.");
                Dialog.Hide(target);
                target.LoadInfo();
            }
            else
                sender.SendClientMessage(Colors.GREY, "Этот игрок не находится на этапе TOTP аутентификации.");
        }
    }
}
