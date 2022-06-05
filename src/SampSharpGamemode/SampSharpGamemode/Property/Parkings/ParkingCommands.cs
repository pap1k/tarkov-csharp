using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharpGamemode.Parkings;
using SampSharpGamemode.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampSharpGamemode.Property.Parkings
{
    class ParkingCommands
    {
        [Command("createparking", PermissionChecker =typeof(FounderAdminPermChecker))]
        private static void CMD_createparking(BasePlayer caller)
        {
            var p = new Parking(caller);
            p.InsertMe();
            caller.SendClientMessage(Colors.GREY, $"Успешно создан паркинг с UID = {p.UID}");
            GameMode.ServerParkings.Add(p);
        }

        [Command("pinfo", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void CMD_pinfo(BasePlayer player, int uid = 0)
        {
            if(uid == 0)
            {
                var close = GameMode.ServerParkings.Where(x => player.GetDistanceFromPoint(x.Position) < 10).ToList();
                if (close.Count > 0)
                {
                    close.Sort((Parking a, Parking b) =>
                    {
                        return player.GetDistanceFromPoint(a.Position).CompareTo(player.GetDistanceFromPoint(b.Position));
                    });
                    var closest = close.First();
                    var dlg = new MessageDialog($"Информация о паркинге #{closest.UID}", $"Rotation: {closest.Rotation}\nOwner: {closest.Owner}\nHouseID: {closest.HouseID}\nCarID: {closest.CarID}", "X");
                    dlg.Show(player);
                }
                else
                    player.SendClientMessage(Colors.GREY, "Рядом с вами не найдено паркингов");
            }
            else
            {
                var closest = GameMode.ServerParkings.Find(x => x.UID == uid);
                if(closest != null)
                {
                    var dlg = new MessageDialog($"Информация о паркинге #{closest.UID}", $"XYZ: {closest.Position.X}, {closest.Position.Y}, {closest.Position.Z}\nRotation: {closest.Rotation}\nOwner: {closest.Owner}\nHouseID: {closest.HouseID}\nCarID: {closest.CarID}", "X");
                    dlg.Show(player);
                }
                else
                {
                    player.SendClientMessage(Colors.GREY, "На сервере не найден паркинг с таким UID.");
                }
            }
        }
        [Command("p3d", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void CMD_p3d(BasePlayer player)
        {
            string pvar = "pISUSED3DPARKVIEW";
            string pvarid = "pPARKING3DVIEWIDS";
            if (player.PVars.Get<bool>(pvar))
            {
                foreach (string id in player.PVars.Get<string>(pvarid).Split(','))
                    PlayerTextLabel.Find(player, int.Parse(id)).Dispose();
                player.PVars[pvar] = false;
                player.SendClientMessage(Colors.GREY, "Режим отображения парковок 3D выключен");
            }
            else
            {
                int[] ids = new int[GameMode.ServerParkings.Count];
                int i = 0;
                foreach (var park in GameMode.ServerParkings)
                {
                    var p = new PlayerTextLabel(player, $"Паркинг #{park.UID}", -1, park.Position, 30);
                    ids[i++] = p.Id;
                }
                player.PVars[pvar] = true;
                string sids = "";
                foreach (int id in ids)
                    sids += id + ",";
                sids = sids.Trim(',');
                player.PVars[pvarid] = sids;
                player.SendClientMessage(Colors.GREY, "Режим отображения парковок 3D включен");
            }
        }
        [Command("deleteparking", UsageMessage ="/deleteparking [UID]", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void cmd_delete(BasePlayer sender, int uid)
        {
            var park = GameMode.ServerParkings.Find(x => x.UID == uid);
            if (park != null)
            {
                GameMode.ServerParkings.Remove(park);
                park.Delete();
                if(park.CarID != -1)
                {
                    //TODO: move Vehicle to Storage
                    sender.SendClientMessage(Colors.GREY, $"Паркинг с UID {park.UID} удален. Транспорт, стоящий на этой парковке, отправлен в хранилище владельца.");
                }
                else
                    sender.SendClientMessage(Colors.GREY, $"Паркинг с UID {park.UID} удален.");
            }
            else
                sender.SendClientMessage(Colors.GREY, "На сервере не найден паркинг с таким UID.");
        }
        [Command("fixparking", UsageMessage = "/fixparking [UID]", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void cmd_fixparking(BasePlayer sender, int uid)
        {
            var park = GameMode.ServerParkings.Find(x => x.UID == uid);
            if (park != null)
            {
                if(sender.InAnyVehicle)
                    park.UpdatePos(sender.Vehicle.Position, sender.Vehicle.Angle);
                else
                    park.UpdatePos(sender.Position, sender.Angle);
                sender.SendClientMessage(Colors.GREY, $"Паркинг #{uid} обновлен");
            }
            else
                sender.SendClientMessage(Colors.GREY, "На сервере не найден паркинг с таким UID.");
        }
    }
}
