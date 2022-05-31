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
    class testcmds
    {
        [Command("xui228", UsageMessage = "/xui228 [Уровень администрирования]")]
        private static void CMD_xui(BasePlayer sender, int lvl)
        {
            sender.PVars[PvarsInfo.admin] = true;
            sender.PVars[PvarsInfo.adminlevel] = lvl;
            sender.SendClientMessage($"Вам выдана админка {{fbec5d}}{lvl} {{ffffff}}уровня. Обратите внимание, что изменения не были внесены в базу.");
        }
        [Command("totptest", UsageMessage = "/totptest [код]", PermissionChecker = typeof(ViceAdminPermChecker))]
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
    }
}
