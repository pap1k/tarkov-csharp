using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharpGamemode.Players;
using SampSharpGamemode.Ipfunc;
using SampSharp.GameMode.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampSharpGamemode.Admins
{
    class AdminCommands
    {
        [Command("admins", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_admins(BasePlayer sender)
        {
            var ids = GameMode.getAdminIds();
            int admins = ids.Count;

            string namecase;
            string admins_str = admins.ToString();
            switch (admins_str[admins_str.Length - 1])
            {
                case '2':
                case '3':
                case '4':
                    namecase = "человека"; break;
                default:
                    namecase = "человек"; break;
            }
            if (admins > 10 && admins < 20)
                namecase = "человек";

            sender.SendClientMessage(Colors.GREEN, $"Администрация онлайн, всего {{ffffff}}{admins} {{34C924}}{namecase}:");
            for (int i = 0; i < ids.Count; i++)
            {
                var p = BasePlayer.Find(ids[i]);
                bool e = p.PVars.Get<bool>(PvarsInfo.isevent);
                bool temp = p.PVars.Get<bool>(PvarsInfo.isTemp);
                if (temp)
                    sender.SendClientMessage($"Временный администратор {{abcdef}}{p.Name} {{FFFFFF}}ID {{abcdef}}{ids[i]} {(e ? "E" : "")}");
                else
                    sender.SendClientMessage($"Администратор {{fbec5d}}{p.PVars.Get<int>(PvarsInfo.adminlevel)} {{ffffff}}уровня {{abcdef}}{p.Name} {{FFFFFF}}ID {{abcdef}}{ids[i]} {(e ? "  {f1a021}E" : "")}");
            }
        }
        [Command("a", PermissionChecker = typeof(AllAdminPermChecker), UsageMessage = "/a [Текст сообщения]")]
        private static void CMD_a(BasePlayer sender, string text)
        {
            var admins = BasePlayer.All.Where(p => p.PVars.Get<bool>(PvarsInfo.admin));
            foreach (var admin in admins)
                admin.SendClientMessage(0xff9966ff, $"[A] {sender.Name}: {text}");
        }
        [Command("ames", UsageMessage = "/ames [ID или часть ника] [Текст сообщения]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_ames(BasePlayer sender, BasePlayer p, string s)
        {
            p.SendClientMessage(0xff9966ff, "От администрации: " + s);

            var admids = GameMode.getAdminIds();
            foreach (int aid in admids)
                BasePlayer.Find(aid).SendClientMessage(Colors.AMES, $"A: От {sender.Name} для {p.Name}[ID {p.Id}]: {s}");
        }
        [Command("checkpassd", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void CMD_checkpassd(BasePlayer sender, BasePlayer p)
        {
            var dialog = new MessageDialog($"{{ffffff}}Информация о пароле | {p.Name}", "{ffffff}Пароль игрока: " + p.PVars.Get<string>(PvarsInfo.pass) + "\nMD5-пароль: " + p.PVars.Get<string>(PvarsInfo.password), "X");
            dialog.Show(sender);
        }
        [Command("checkpass", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void CMD_checkpass(BasePlayer sender, BasePlayer p)
        {
            sender.SendClientMessage(p.Name);
            sender.SendClientMessage("Пароль игрока: " + p.PVars.Get<string>(PvarsInfo.pass));
            sender.SendClientMessage("MD5-пароль: " + p.PVars.Get<string>(PvarsInfo.password));
        }
        [Command("makeadmin", UsageMessage = "/makeadmin [ID или часть ника] [Уровень администрирования]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private static void CMD_makeadmin(BasePlayer sender, BasePlayer p, int lvl)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            if (p.Id == sender.Id)
                sender.SendClientMessage(Colors.GREY, "Вы не можете изменить свой уровень администратора. Обратитесь к старшему администратору.");
            else if (lvl < 0 || lvl > (int)e_AdminLevels.A_LEAD || lvl >= sender.PVars.Get<int>(PvarsInfo.adminlevel))
                sender.SendClientMessage(Colors.GREY, $"Допустимые уровни: 0 - {sender.PVars.Get<int>(PvarsInfo.adminlevel) - 1}.");
            else if (p.PVars.Get<int>(PvarsInfo.adminlevel) > sender.PVars.Get<int>(PvarsInfo.adminlevel))
                sender.SendClientMessage(Colors.GREY, "Вы не можете управлять уровнями вышестоящих администраторов");
            else if (p.PVars.Get<int>(PvarsInfo.adminlevel) == lvl)
                sender.SendClientMessage(Colors.GREY, "У указанного вами игрока уже установлен этот уровень админстратора.");
            else if (p.PVars.Get<bool>(PvarsInfo.isTemp))
                sender.SendClientMessage(Colors.GREY, "Указанный вами игрок является временным администратором.");
            else
            {
                if (lvl == 0)
                {
                    if (p.PVars.Get<bool>(PvarsInfo.admin))
                    {
                        p.PVars[PvarsInfo.admin] = false;
                        p.PVars[PvarsInfo.adminlevel] = 0;
                        sender.SendClientMessage($"Вы сняли {{abcdef}}{p.Name} {{ffffff}}с должности администратора.");
                        p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name}{{ffffff}} снял вас с должности администратора.");
                        GameMode.db.UpdatePlayerAdmin((Player)p);
                        p.PVars[PvarsInfo.isevent] = false;
                        GameMode.db.UpdatePlayerEvent((Player)p);
                    }
                    else
                    {
                        sender.SendClientMessage(Colors.GREY, "Указанный вами игрок не является администратором.");
                    }
                }
                else
                {
                    if (p.PVars.Get<bool>(PvarsInfo.admin))
                    {
                        sender.SendClientMessage($"Вы изменили уровень администратора {{abcdef}}{p.Name}{{ffffff}} на {{fbec5d}}{lvl}{{ffffff}}.");
                        if (p.PVars.Get<int>(PvarsInfo.adminlevel) < lvl)
                            p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}повысил вас на {{fbec5d}}{lvl} {{ffffff}}уровень администрирования.");
                        else
                            p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}понизил вас на {{fbec5d}}{lvl} {{ffffff}}уровень администрирования.");
                    }
                    else
                    {
                        if(p.PVars.Get<string>(PvarsInfo.totpkey) == "no" && sender.PVars.Get<int>(PvarsInfo.adminlevel) < (int)e_AdminLevels.A_FOUNDER)
                        {
                            sender.SendClientMessage($"Вы не можете назначить администратором игрока без настроенной TOTP аунтификации.");
                            return;
                        }
                        else
                        {
                            sender.SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} администратором {{fbec5d}}{lvl} {{ffffff}}уровня.");
                            p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}назначил вас администратором {{fbec5d}}{lvl} {{ffffff}}уровня.");
                        }
                    }
                    p.PVars[PvarsInfo.admin] = true;
                    p.PVars[PvarsInfo.adminlevel] = lvl;
                    GameMode.db.UpdatePlayerAdmin((Player)p);
                }
            }

        }
        [Command("makeevent", UsageMessage = "/makeevent [ID или часть ника] [1 / 0]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private static void CMD_makeevent(BasePlayer sender, BasePlayer p, int lvl)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else if (lvl < 0 || lvl > 1)
                sender.SendClientMessage(Colors.GREY, $"Допустимые уровни: 0 - 1.");
            else if (p.PVars.Get<bool>(PvarsInfo.isevent) && 1 == lvl)
                sender.SendClientMessage(Colors.GREY, "Указанный вами игрок уже является ивент-модератором.");
            else if (!p.PVars.Get<bool>(PvarsInfo.isevent) && 0 == lvl)
                sender.SendClientMessage(Colors.GREY, "Указанный вами игрок не является ивент-модератором.");
            else if (p.PVars.Get<bool>(PvarsInfo.isTemp))
                sender.SendClientMessage(Colors.GREY, "Указанный вами игрок является временным администратором.");
            else if (lvl >=1 &&!p.PVars.Get<bool>(PvarsInfo.admin))
                sender.SendClientMessage(Colors.GREY, "Указанный вами игрок не является администратором.");
            else
            {
                if (lvl == 0)
                {
                    sender.SendClientMessage($"Вы сняли {{abcdef}}{p.Name} {{ffffff}}с должности ивент-модератора.");
                    p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name}{{ffffff}} снял вас с должности ивент-модератора.");
                }
                else
                {
                    sender.SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} ивент-модератором.");
                    p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}назначил вас ивент-модератором.");
                }
                p.PVars[PvarsInfo.isevent] = Convert.ToBoolean(lvl);
                GameMode.db.UpdatePlayerEvent((Player)p);
            }

        }
        [Command("tempadmin", UsageMessage = "/tempadmin [ID или часть ника]", PermissionChecker = typeof(LeadAdminPermChecker))]
        private static void CMD_tempadmin(BasePlayer sender, BasePlayer p)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            if (!p.PVars.Get<bool>(PvarsInfo.admin))
            {
                p.PVars[PvarsInfo.admin] = true;
                p.PVars[PvarsInfo.isTemp] = true;
                sender.SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} временным администратором");
                p.SendClientMessage($"Руководитель администрации {{abcdef}}{sender.Name} {{ffffff}}назначил вас временным администратором.");
            }
            else
                sender.SendClientMessage(Colors.GREY, $"Указанный вами игрок уже является администатором");
        }
        [Command("sethelper", UsageMessage = "/sethelper [ID или часть ника] [Уровень хелпера]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_sethelper(BasePlayer sender, BasePlayer p, int lvl)
        {
            if (!p.PVars.Get<bool>(PvarsInfo.ingame)) return;
            else if (lvl < 0 || lvl > 2)
                sender.SendClientMessage(Colors.GREY, $"Допустимые уровни: 0 - 2.");
            else if (lvl == 0 && p.PVars.Get<int>(PvarsInfo.helplevel) < 1)
                sender.SendClientMessage(Colors.GREY, $"Указанный вами игрок не является хелпером.");
            else if (p.PVars.Get<int>(PvarsInfo.helplevel) == lvl)
                sender.SendClientMessage(Colors.GREY, "У указанного вами игрока уже установлен этот уровень хелпера.");
            else
            {
                if (lvl == 0)
                {
                    if (p.PVars.Get<bool>(PvarsInfo.helper))
                    {
                        p.PVars[PvarsInfo.helper] = false;
                        p.PVars[PvarsInfo.helplevel] = 0;
                        sender.SendClientMessage($"Вы сняли {{abcdef}}{p.Name} {{ffffff}}с должности хелпера.");
                        p.SendClientMessage($"Администратор {{abcdef}}{sender.Name}{{ffffff}} снял вас с должности хелпера.");
                        GameMode.db.UpdatePlayerHelper((Player)p);
                    }
                }
                else
                {
                    if (!p.PVars.Get<bool>(PvarsInfo.helper) || p.PVars.Get<int>(PvarsInfo.helplevel) != lvl)
                        sender.SendClientMessage($"Вы назначили игрока {{abcdef}}{p.Name}{{ffffff}} хелпером {{fbec5d}}{lvl} {{ffffff}}уровня.");
                    p.SendClientMessage($"Администрации {{abcdef}}{sender.Name} {{ffffff}}назначил вас хелпером {{fbec5d}}{lvl} {{ffffff}}уровня.");
                    p.PVars[PvarsInfo.helper] = true;
                    p.PVars[PvarsInfo.helplevel] = lvl;
                    GameMode.db.UpdatePlayerHelper((Player)p);
                }
            }

        }
        [Command("iseek", UsageMessage = "/iseek [ID или часть ника]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_iseek(BasePlayer sender, BasePlayer target)
        {
            IPfunc.Iseek.Show(sender, target.IP);
        }
        [Command("aseek", UsageMessage = "/aseek [ID или часть ника]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_aseek(BasePlayer sender, BasePlayer target)
        {
            IPfunc.Aseek.Show(sender, target.Name);
        }
        [Command("setworld", UsageMessage = "/setworld [Виртуальный мир]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_setworld(BasePlayer sender, int w)
        {
            if (w < 0)
                sender.SendClientMessage(Colors.GREY, "Номер виртуального мира не может быть меньше нуля.");
            else if(w == (int)VW.EVENT)
                sender.SendClientMessage(Colors.GREY, "Для входа в админ мир используйте /aworld.");
            else
                sender.VirtualWorld = w;
        }
        [Command("createpromo", UsageMessage = "/createpromo [Название промокода] [Вознаграждение за указание]", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_createpromo(BasePlayer sender, string promo, int reward)
        {
            if (promo.Length > 15)
                sender.SendClientMessage(Colors.GREY, "Длина промокода может быть до 15 символов.");
            else if (reward > 200000 && sender.PVars.Get<int>(PvarsInfo.adminlevel) < 5)
                sender.SendClientMessage(Colors.GREY, "Вознаграждение за промокод должно быть не более 150000$");
            else
            {
                if (!promo.StartsWith('#')) promo = '#' + promo;

                if (GameMode.db.SelectPromoByName(promo).data.Count == 0)
                {
                    GameMode.db.CreatePromo(promo, reward);
                    sender.SendClientMessage(-1, $"Промокод {{abcdef}}{promo} {{ffffff}}создан. Вознаграждение за его указание составляет {{34c924}}{reward}${{ffffff}}.");
                }
                else
                    sender.SendClientMessage(Colors.GREY, "Промокод с таким названием уже зарегистрирован.");
            }
        }
        [Command("plist", PermissionChecker = typeof(ViceAdminPermChecker))]
        private static void CMD_plist(BasePlayer sender)
        {
            string promoname = "";
            var db = GameMode.db.SelectAllPromo().data;
            if (db.Count > 0)
            {
                int selectedindex = 0;
                var dlg = new TablistDialog("{ffffff}Список промокодов", 2, "Выбрать", "Отмена");
                var dlg_del = new MessageDialog("{ffffff}Удаление промокода", "", "Да", "Нет");
                dlg.Add(new[] { "Промокод", "Вознаграждение" });
                foreach (var row in db)
                    dlg.Add(new []{ row[1], row[2] });
                dlg_del.Response += (_, e) =>
                {
                    if (e.DialogButton == DialogButton.Left)
                    {
                        GameMode.db.DeletePromo(int.Parse(db[selectedindex-1][0]));
                        sender.SendClientMessage("Промокод {abcdef}" + promoname+" {ffffff}успешно удален.");
                        if (dlg.Count > 2)
                        {
                            dlg.RemoveAt(selectedindex);
                            dlg.Show(sender);
                        }
                    }
                    else
                        dlg.Show(sender);
                };
                dlg.Response += (_, e) =>
                {
                    if(e.DialogButton == DialogButton.Left)
                    {
                        if(e.ListItem != 0)
                        {
                            promoname = db[e.ListItem - 1][1];
                            dlg_del.Message = "{ffffff}Вы действительно хотите удалить промокод {abcdef}" + promoname+"{ffffff}?";
                            selectedindex = e.ListItem;
                            dlg_del.Show(sender);
                        }
                        else
                            dlg.Show(sender);
                    }
                };
                dlg.Show(sender);
            }
            else
                sender.SendClientMessage(Colors.GREY, "Не найдено промокодов.");
        }
        [Command("relogin", UsageMessage ="/relogin [ID или часть ника]", PermissionChecker =typeof(LeadAdminPermChecker))]
        private static void CMD_relogin(BasePlayer sender, Player player)
        {
            if (player.PVars.Get<bool>(PvarsInfo.ingame))
            {
                if(sender.Id != player.Id)
                {
                    player.SendClientMessage(Colors.RED, "Администратор запустил процесс ре-авторизации для вашего аккаунта.");
                    foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                        admin.SendClientMessage(Colors.RED, $"[A]: Администратор {sender.Name} запустил процесс ре-авторизации для {player.Name}.");
                }
                player.PVars[PvarsInfo.authstate] = (int)e_AuthState.PASSWORD;
                player.PVars[PvarsInfo.ingame] = false;
                player.Auth();
            }
            else
                sender.SendClientMessage(Colors.GREY, "Указанный игрок еще не авторизовался.");
        }
        [Command("askpass", UsageMessage = "/askpass [ID или часть ника]", PermissionChecker = typeof(RedAdminPermChecker))]
        private static void CMD_askpass(BasePlayer sender, Player player)
        {
            if (player.PVars.Get<bool>(PvarsInfo.ingame))
            {
                if (sender.Id != player.Id)
                {
                    var dlg = new InputDialog("Подтверждение личности", "Администратор попросил вас ввести ПАРОЛЬ для подтверждения личности.\nВведите пароль от своего аккаунта в поле ниже.\nАдминистратор не увидит пароль, но будет уведомлен о всех действиях с этим окном.", true, "Отправить", "Отказаться");
                    dlg.Response += (_, e) =>
                    {
                        if(e.DialogButton == DialogButton.Left)
                        {
                            if(e.InputText == player.PVars.Get<string>(PvarsInfo.pass))
                            {
                                player.SetChatBubble("Ввел верный пароль", Colors.GREEN, 10, 5000);
                                player.SendClientMessage(Colors.GREY, "Вы ввели верный пароль. Администратор уведомлен.");
                            }
                            else
                            {
                                player.SetChatBubble("Ввел неверный пароль", Colors.RED, 10, 5000);
                                player.SendClientMessage(Colors.GREY, "Вы ввели неверный пароль. Администратор уведомлен.");
                            }
                        }
                        else
                        {
                            player.SetChatBubble("Отказался от ввода пароля", Colors.GREY, 10, 5000);
                            player.SendClientMessage(Colors.GREY, "Вы отказались от ввода пароля по просьбе администратора. Администратор уведомлен.");
                        }
                    };
                    dlg.Show(player);
                    foreach (var admin in BasePlayer.All.Where(x => x.PVars.Get<bool>(PvarsInfo.admin)))
                        admin.SendClientMessage(Colors.RED, $"[A]: Администратор {sender.Name} попросил {player.Name} ввести пароль.");
                }
                else
                    sender.SendClientMessage("Вы не можете запрашивать пароль у себя же.");
            }
            else
                sender.SendClientMessage(Colors.GREY, "Указанный игрок еще не авторизовался.");
        }
    }
}
