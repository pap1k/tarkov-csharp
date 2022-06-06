using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharpGamemode;
using SampSharpGamemode.Players;
using SampSharpGamemode.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharpGameMode.Admins
{
    [CommandGroup("test")]
    class testcmds
    {
        [Command("mypos", UsageMessage = "/test xui228 [Уровень администрирования]")]
        private static void CMD_xui(BasePlayer sender)
        {
            sender.SendClientMessage($"Ваша позиция: X: {sender.Position.X}, Y: {sender.Position.Y}, Z: {sender.Position.Z}");
        }
        [Command("xui228", UsageMessage = "/test xui228 [Уровень администрирования]")]
        private static void CMD_xui(BasePlayer sender, int lvl)
        {
            sender.PVars[PvarsInfo.admin] = true;
            sender.PVars[PvarsInfo.adminlevel] = lvl;
            sender.SendClientMessage($"Вам выдана админка {{fbec5d}}{lvl} {{ffffff}}уровня. Обратите внимание, что изменения не были внесены в базу.");
        }
        [Command("totptest", UsageMessage = "/test totptest [код]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_totptest(BasePlayer sender, string text)
        {
            if (!sender.PVars.Get<bool>(PvarsInfo.ingame)) return;
            sender.SendClientMessage(TOTP.Get(text));
        }
        [Command("testtry", UsageMessage = "/xui228 [Уровень администрирования]")]
        private static void CMD_asdasd(BasePlayer sender)
        {
            int t = 0, f = 0;
            for(int i = 0; i < 100; i++)
            {
                Random r = new Random();
                if (r.Next(0, 11) > 5)
                    t++;
                else
                    f++;
            }
            sender.SendClientMessage($"{t}/{f} ({t/f})");
        }
        [Command("cs", UsageMessage = "цель, arg. арг 1294 это столб")]
        private static void CMD_aasdsadasd(BasePlayer sender, BasePlayer target, int arg)
        {
            GameMode.Native.SendClientCheck(target.Id, 71, arg, 0, 48);
            sender.SendClientMessage($"Запрос отправлен");
        }
    }
}
