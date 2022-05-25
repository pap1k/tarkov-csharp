using System.Linq;
using System.Collections.Generic;
using System.Text;
using SampSharpGamemode.Players;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;

namespace SampSharpGamemode.Admins
{
    class Punishments
    {
        private static void banfunc(BasePlayer caller, Player target, string caterogy, string reason, bool isAban)
        {
            string[] allow = { "c", "r", "hr", "hc" };
            if (allow.Contains(caterogy))
            {
                if (!target.PVars.Get<bool>(PvarsInfo.isleaving))
                {
                    string admmess = "Постоянная блокировка аккаунта";
                    if (caterogy[0] != 'h') admmess = "Заблокирован на Х дней";
                    foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                        admin.SendClientMessage(Colors.RED, $"[A]: {caller.Name} забанил {target.Name}: [{caterogy.ToUpper()}] {reason}");
                    target.ban(admmess, reason, caterogy[0] == 'h');
                    target.kick();
                }
                else
                    caller.SendClientMessage(Colors.GREY, "Игрок уже покидает сервер");
            }
            else
                caller.SendClientMessage(Colors.GREY, "Категория указана неверно");
        }
        [Command("ban", UsageMessage = "/ban [ID или чать ника] [R | HR | C | HC (/bannahehelp)] [Причина]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_ban(BasePlayer caller, Player target, string caterogy, string reason)
        {
            banfunc(caller, target, caterogy, reason, false);
        }
        [Command("aban", UsageMessage = "/aban [ID или чать ника] [R | HR | C | HC (/bannahehelp)] [Причина]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_aban(BasePlayer caller, Player target, string caterogy, string reason)
        {
            banfunc(caller, target, caterogy, reason, true);
        }
        [Command("sban", UsageMessage = "/aban [ID или чать ника] [R | HR | C | HC (/bannahehelp)] [Причина]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private static void CMD_sban(BasePlayer caller, Player target, string caterogy)
        {
            string admmess = "Скрытая блокировка";
            target.ban(admmess, "", true);
        }
    }
}
