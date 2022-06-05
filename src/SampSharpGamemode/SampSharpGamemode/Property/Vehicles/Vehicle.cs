using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using SampSharpGamemode.Parkings;
using SampSharpGamemode.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampSharpGamemode.Vehicles
{
    public enum e_DBVehicle
    {
        UID = 0,
        TYPE,
        COLORS,
        ALARM,
        OWNER,
        PARKING
    }
    public class MyVehicle : BaseVehicle
    {
        public override void OnStreamIn(PlayerEventArgs e)
        {
            var veh = GameMode.ServerVehicles.Find(x => x.RealVehicle == this);
            SetParametersForPlayer(e.Player, false, veh.isOpen);
            
            base.OnStreamIn(e);
        }
        public override void OnSpawn(EventArgs e)
        {
            var veh = GameMode.ServerVehicles.Find(x => x.RealVehicle == this);
            veh.isSpawned = true;
            Console.WriteLine(veh.isSpawned);
            base.OnSpawn(e);
        }
        public override void OnDeath(PlayerEventArgs e)
        {
            var veh = GameMode.ServerVehicles.Find(x => x.RealVehicle == this);
            veh.isSpawned = false;
            base.OnDeath(e);
        }
        public override void OnPlayerEnter(EnterVehicleEventArgs e)
        {
            base.OnPlayerEnter(e);
            var veh = GameMode.ServerVehicles.Find(x => x.RealVehicle == e.Vehicle);
            veh.LastDriverUID = e.Player.PVars.Get<int>(PvarsInfo.uid);
            veh.ldnick = e.Player.Name;
        }
    }
    public class Vehicle
    {
        public BaseVehicle RealVehicle;
        public bool isOpen = false;
        public bool isTemp = false;
        public bool AddAlarm = false;
        public int UID, Color1, Color2;
        public bool Loaded = false;
        public VehicleModelType Type;
        public int Owner, ParkingID;
        public string LastDriverNick { get { return GetLastDriverName(); } set { } }
        public string ldnick;
        public int LastDriverUID;
        public bool isNew = false;
        public bool isSpawned;
        public Vehicle(int UID, VehicleModelType vehicletype, Parking parking, int owneruid, int color1 = 204, int color2 = 204, int respawnDelay = -1, bool addAlarm = false)
        {
            Type = vehicletype;
            this.UID = UID;
            Owner = owneruid;
            Color1 = color1;
            Color2 = color2;
            if(parking != null)
            {
                ParkingID = parking.UID;
                Loaded = true;
            }
            else
            {
                ParkingID = -1;
                Loaded = false;
            }
            isTemp = false;
            isOpen = true;
            AddAlarm = addAlarm;
        }
        public Vehicle(VehicleModelType vehicletype, Parking parking, int color1 = 204, int color2 = 204, int respawnDelay = -1, bool addAlarm = false)
        {
            Type = vehicletype;
            Owner = parking.Owner;
            Color1 = color1;
            Color2 = color2;
            ParkingID = parking.UID;
            isTemp = false;
            isOpen = true;
            AddAlarm = addAlarm;
            isNew = true;
        }
        public Vehicle(VehicleModelType vehicletype, BasePlayer owner, int color1 = 204, int color2 = 204, int respawnDelay = -1, bool addAlarm = false)
        {
            Owner = owner.PVars.Get<int>(PvarsInfo.uid);
            Type = vehicletype;
            ParkingID = -1;
            isTemp = true;
            isOpen = true;
        }
        public void Spawn()
        {
            var owner = BasePlayer.All.ToList().Find(x => x.PVars.Get<int>(PvarsInfo.uid) == Owner);
            var parking = GameMode.ServerParkings.Find(x => x.UID == ParkingID);
            if (isTemp)
            {
                var pos = new Vector3(owner.Position.X + 2, owner.Position.Y + 2, owner.Position.Z);
                RealVehicle = BaseVehicle.Create(Type, pos, owner.Angle, Color1, Color2, -1, AddAlarm);
            }
            else
                RealVehicle = BaseVehicle.Create(Type, parking.Position, parking.Rotation, Color1, Color2, -1, AddAlarm);
            isSpawned = true;
        }
        public void Spawn(Vector3 pos)
        {
            if(RealVehicle != null)
            {
                if (isSpawned)
                    RealVehicle.Repair();
                else
                    RealVehicle.Respawn();
                RealVehicle.Position = pos;
            }
            else
            {
                RealVehicle = BaseVehicle.Create(Type, pos, 0.0f, Color1, Color2, -1, AddAlarm);
            }
            isSpawned = true;
        }
        public int SaveNew()
        {
            if (isNew)
            {
                GameMode.db.InsertVehicle((int)Type, $"{Color1},{Color2}", Convert.ToInt32(AddAlarm), Owner, ParkingID);
                UID = int.Parse(GameMode.db.LAST_INSERT_ID().data[0][0]);
            }
            return UID;
        }
        private string GetLastDriverName()
        {
            var p = BasePlayer.All.ToList().Find(x => x.PVars.Get<int>(PvarsInfo.uid) == LastDriverUID);
            return p != null ? p.Name : "[OFF] " + ldnick;
        }
        public void UpdateParking(int uid)
        {
            UID = uid;
            GameMode.db.UpdateVehicle_parkid(UID, uid);
        }
        public static Vehicle GetClosest(Vector3 to)
        {
            var close = GameMode.ServerVehicles.Where(x => x.isSpawned && to.DistanceTo(x.RealVehicle.Position) <= 10).ToList();
            if (close.Count == 0)
                return null;
            else if (close.Count == 1)
                return close.First();
            else
            {
                close.Sort((Vehicle x, Vehicle y) => to.DistanceTo(x.RealVehicle.Position).CompareTo(to.DistanceTo(y.RealVehicle.Position)));
                return close.First();
            }
        }
    }
}
