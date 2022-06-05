using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.World;
using SampSharpGamemode.Vehicles;
using SampSharpGamemode.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampSharpGamemode.Parkings;

namespace SampSharpGamemode.Property.Vehicles
{
    class VehcielCommands
    {
        [Command("tempmodel", UsageMessage ="/tempmodel [ID или часть ника] [ID модели транспорта]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_spawnmodel(BasePlayer caller, BasePlayer target,  int type)
        {
            if (Enum.IsDefined(typeof(VehicleModelType), type))
            {
                var v = new Vehicle((VehicleModelType)type, caller);
                v.Spawn();
                target.PutInVehicle(v.RealVehicle);
                target.PVars[PvarsInfo.tempadmincar] = v.RealVehicle.Id;
                if(caller != target)
                {
                    target.SendClientMessage(Colors.GREY, $"Администратор {caller.Name} посадил вас во временный админ транспорт.");
                    caller.SendClientMessage(Colors.GREY, $"Вы посадили {target.Name} во временный админ транспорт.");
                }
            }
            else
                caller.SendClientMessage(Colors.GREY, "Указанной вами модели не существует");
        }
        [Command("tempmodel", UsageMessage = "/tempmodel [ID модели транспорта]", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void CMD_spawnmodel(BasePlayer caller, int type)
        {
            if (Enum.IsDefined(typeof(VehicleModelType), type))
            {
                var v = new Vehicle((VehicleModelType)type, caller);
                v.Spawn();
                caller.PutInVehicle(v.RealVehicle);
                caller.PVars[PvarsInfo.tempadmincar] = v.RealVehicle.Id;
            }
            else
                caller.SendClientMessage(Colors.GREY, "Указанной вами модели не существует");
        }
        [Command("createvehicle", UsageMessage = "/createvehicle [ID паркинга] [ID модели транспорта]", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void CMD_createvehicle(BasePlayer caller, int parkingid, int type)
        {
            if (Enum.IsDefined(typeof(VehicleModelType), type))
            {
                Parking park = GameMode.ServerParkings.Find(x => x.UID == parkingid);
                if(park != null)
                {
                    if(park.CarID == -1)
                    {
                        if(park.Owner != -1)
                        {
                            var veh = new Vehicle((VehicleModelType)type, park);
                            veh.Spawn();
                            int uid = veh.SaveNew();
                            park.SetCar(uid);
                            GameMode.ServerVehicles.Add(veh);
                            caller.SendClientMessage(Colors.GREY, "Успешно создан транспорт с UID = " + uid);
                        }
                        else
                            caller.SendClientMessage(Colors.GREY, "Указанная парковка не находится в собственности игрока. Чтобы создать транспорт используйте /createstaticvehicle");
                    }
                    else
                    {
                        var veh = GameMode.ServerVehicles.Find(x => x.UID == park.CarID);
                        if(veh != null)
                        {
                            caller.SendClientMessage(Colors.GREY, "На паркинге заспавнен транспорт с ID " + veh.RealVehicle.Id);
                            caller.SendClientMessage(Colors.GREY, "Используйте /unloadvehicle или /deletevehicle чтобы освободить парковку.");
                        }
                        else
                            caller.SendClientMessage(Colors.GREY, "К паркингку привязан транспорт, но его не удалось найти в пуле загруженных. UID: "+park.CarID);
                    }
                }
                else
                    caller.SendClientMessage(Colors.GREY, "Не найден паркинг по указанному ID.");
            }
            else
                caller.SendClientMessage(Colors.GREY, "Указанной вами модели не существует");
        }
        [Command("createstaticvehicle", UsageMessage = "/createstaticvehicle [ID паркинга] [ID модели транспорта]", PermissionChecker = typeof(FounderAdminPermChecker))]
        private static void CMD_ssdpfksdl(BasePlayer caller, int parkingid, int type)
        {
            if (Enum.IsDefined(typeof(VehicleModelType), type))
            {
                Parking park = GameMode.ServerParkings.Find(x => x.UID == parkingid);
                if (park != null)
                {
                    if (park.CarID == -1)
                    {
                        var veh = new Vehicle((VehicleModelType)type, park);
                        veh.Spawn();
                        int uid = veh.SaveNew();
                        park.SetCar(uid);
                        GameMode.ServerVehicles.Add(veh);
                        caller.SendClientMessage(Colors.GREY, "Успешно создан статичный транспорт с UID = " + uid);
                    }
                    else
                    {
                        var veh = GameMode.ServerVehicles.Find(x => x.UID == park.CarID);
                        if (veh != null)
                        {
                            caller.SendClientMessage(Colors.GREY, "На паркинге заспавнен транспорт с ID " + veh.RealVehicle.Id);
                            caller.SendClientMessage(Colors.GREY, "Используйте /unloadvehicle или /deletevehicle чтобы освободить парковку.");
                        }
                        else
                            caller.SendClientMessage(Colors.GREY, "К паркингку привязан транспорт, но его не удалось найти в пуле загруженных. UID: " + park.CarID);
                    }
                }
                else
                    caller.SendClientMessage(Colors.GREY, "Не найден паркинг по указанному ID.");
            }
            else
                caller.SendClientMessage(Colors.GREY, "Указанной вами модели не существует");
        }
        [Command("vinfo", PermissionChecker = typeof(AllAdminPermChecker))]
        private static void cmd_vinfo(BasePlayer sender, int id = -1)
        {
            if(id != -1)
            {
                var veh = GameMode.ServerVehicles.Find(x => x.RealVehicle.Id == id);
                if (veh == null)
                    sender.SendClientMessage(Colors.GREY, "Не найдено транспорта с указанным ID");
                else
                {
                    sender.SendClientMessage($"Транспорт ID: {veh.RealVehicle.Id}, UID: {veh.UID}. LD: {veh.LastDriverNick}, ParkingID: {veh.ParkingID}, O(uid): {veh.Owner}");
                    sender.SendClientMessage($"PosX: {veh.RealVehicle.Position.X}, PosY: {veh.RealVehicle.Position.Y}, Posz: {veh.RealVehicle.Position.Z}");
                }
            }
            else
            {
                var veh = Vehicle.GetClosest(sender.Position);
                if (veh == null)
                    sender.SendClientMessage(Colors.GREY, "Рядом с вами не найдено ТС");
                else
                {
                    sender.SendClientMessage($"Транспорт ID: {veh.RealVehicle.Id}, UID: {veh.UID}. LD: {veh.LastDriverNick}, ParkingID: {veh.ParkingID}, O(uid): {veh.Owner}");
                    sender.SendClientMessage($"PosX: {veh.RealVehicle.Position.X}, PosY: {veh.RealVehicle.Position.Y}, Posz: {veh.RealVehicle.Position.Z}");
                }
            }
        }
    }
}
