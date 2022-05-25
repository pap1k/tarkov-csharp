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
            sender.SendClientMessage(Security.TOTP.Get(text));
        }
        [Command("totpshow", UsageMessage = "/totpshow [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totpshow(BasePlayer sender, BasePlayer t)
        {
            string code = t.PVars.Get<string>(temptotp);
            if (code != "no")
            {
                var dlg = new MessageDialog("Введите код в приложение", "Все символы заглавные и в анлийской раскладке\n"+code, "X");
                dlg.Show(t);
                t.PVars[totpcreate] = true;
                foreach (var adm in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                    adm.SendClientMessage(Colors.RED, $"[A] {sender.Name} продемонстрировал {t.Name} его TOTP токен");
            }
            else
                sender.SendClientMessage("У указанного вами игрока не сгенерирован TOTP токен. Сгенеировать - /totpgen");
        }
        [Command("totpgen", UsageMessage = "/totpgen [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totpgen(BasePlayer sender, BasePlayer t)
        {
            string code = Security.TOTP.GenerateKey();
            t.PVars[temptotp] = code;
            sender.SendClientMessage($"Вы сгенерировали TOTP токен для {t.Name}");
        }
        [Command("atotp", UsageMessage = "/atotp")]
        private static void CMD_atotp(BasePlayer sender)
        {
            if (!sender.PVars.Get<bool>(totpcreate))
            {
                sender.SendClientMessage("Вы не проходите процеруду подтверждения ТОТP токена");
                return;
            }
            var dlg = new InputDialog("Введите 6-значный код из приложения", " ", false, "OK");
            dlg.Response += (_, e) =>
            {
                if(e.DialogButton == DialogButton.Left)
                {
                    bool o = int.TryParse(e.InputText, out int _);
                    if (o)
                    {
                        if (Security.TOTP.Get(sender.PVars.Get<string>(temptotp)) == e.InputText)
                        {
                            sender.SetChatBubble("КОД ВВЕДЕН ВЕРНО", Colors.GREEN, 10f, 10 * 1000);
                            sender.PVars[PvarsInfo.totpkey] = sender.PVars.Get<string>(temptotp);
                            GameMode.db.UpdatePlayerTotp(sender);
                            sender.PVars.Delete(totpcreate);
                        }
                        else
                            sender.SetChatBubble("КОД ВВЕДЕН НЕВЕРНО", Colors.RED, 10f, 10 * 1000);
                    }
                }
                else
                    sender.SetChatBubble("Закрыл окно ввода кода", Colors.RED, 10f, 10 * 1000);
            };
            sender.SetChatBubble("Открыл окно ввода кода", Colors.GREEN, 10f, 10 * 1000);
            dlg.Show(sender);
        }
        [Command("fucklast", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_fucklast(BasePlayer sender)
        {
            GameMode.db.UpdatePlayerLastIP(sender, true);
            sender.SendClientMessage("OK");
        }
        [Command("totpflush", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totpflush(BasePlayer sender, BasePlayer p)
        {
            p.PVars[PvarsInfo.totpkey] = "no";
            GameMode.db.UpdatePlayerTotp(p);
            sender.SendClientMessage($"Вы сбросили TOTP токен игроку {p.Name}");
        }
        [Command("forcetotp", UsageMessage = "/forcetotp [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_forcetotp(BasePlayer sender, Player target)
        {
            if((e_AuthState)target.PVars.Get<int>(PvarsInfo.authstate) == e_AuthState.TOTP)
            {
                foreach (var p in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                    p.SendClientMessage(Colors.RED, $"[A]: Администратор {sender.Name} позволил {target.Name} войти в аккаунт без TOTP аутентификации.");
                target.SendClientMessage(Colors.RED, $"[A]: Администратор {sender.Name} позволил вам войти в аккаунт без TOTP аутентификации.");
                Dialog.Hide(target);
                target.LoadInfo();
            }
            else
                sender.SendClientMessage(Colors.GREY, "Игрок не находится на стадии ввода кода приложения");
        }
    }
}
