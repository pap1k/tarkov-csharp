using SampSharp.GameMode.Display;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharpGamemode.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SampSharpGameMode.Admins;
using SampSharp.GameMode;

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
        [Command("totpshowex", UsageMessage = "/totpshowex [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totpshowex(BasePlayer sender, BasePlayer t)
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
        [Command("totpshow", UsageMessage = "/totpshowqr [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totpshow(BasePlayer sender, BasePlayer target)
        {
            sender.SendClientMessage(Colors.GREY, "Используйте /totpshowex");
            return;
            string code = target.PVars.Get<string>(temptotp);
            if (code != "no")
            {
                var qr = Generator.Generate("otpauth://totp/_?secret="+ code);
                var tds = TOTPQR.CreateQR(qr, target);
                sender.PVars["LISTQR"] = tds;
                sender.SendClientMessage("OK");
                target.PVars[totpcreate] = true;
                foreach (var adm in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                    adm.SendClientMessage(Colors.RED, $"[A] {sender.Name} продемонстрировал {target.Name} его TOTP токен");
            }
            else
                sender.SendClientMessage("У указанного вами игрока не сгенерирован TOTP токен. Сгенеировать - /totpgen");
        }
        [Command("testtxd", UsageMessage = "/testtxd [x] [y]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_testtxd(BasePlayer sender, int x, int y)
        {
            var td = new PlayerTextDraw(sender, new Vector2(x, y), "_");
            td.Height = 10;
            td.Width = 10;
            td.PreviewZoom = 1;
            td.Font = TextDrawFont.PreviewModel;
            td.PreviewModel = -1;
            td.Outline = 0;
            td.UseBox = true;
            td.BackColor = Colors.GREEN;
            td.Show();
            sender.SendClientMessage("POK");
        }
        [Command("totphideqr", UsageMessage = "/totphideqr [ID или часть ника]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totphideqr(BasePlayer sender)
        {
            foreach (var x in sender.PVars.Get<List<int>>("LISTQR"))
                ;
            sender.SendClientMessage("OK");
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
