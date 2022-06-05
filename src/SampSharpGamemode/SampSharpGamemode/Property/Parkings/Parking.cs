using SampSharp.GameMode;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharpGamemode.Parkings
{
    public enum e_DBParking
    {
        UID = 0,
        POSX,
        POSY,
        POSZ,
        ROTATION,
        OWNER,
        ATTACHEDHOUSE,
        CARID,
        AUTOSPAWN
    }
    public class Parking
    {
        public int UID { get; set; }
        public int Owner { get; set; }
        public int HouseID { get; set; }
        public int CarID { get; set; }
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }
        private bool _new = false;
        public bool AutoRespawn;
        public Parking(int uid, float x, float y, float z, float rotation, int owner, int houseid, int carid, bool autorespawn = false)
        {
            UID = uid;
            HouseID = houseid;
            Position = new Vector3(x, y, z);
            Rotation = rotation;
            Owner = owner;
            CarID = carid;
            AutoRespawn = autorespawn;
        }
        public Parking(BasePlayer player)
        {
            _new = true;
            HouseID = -1;
            Position = player.Position;
            Rotation = player.Angle;
            Owner = -1;
            CarID = -1;
        }
        public void Delete()
        {
            if(CarID != -1)
            {
                var veh = GameMode.ServerVehicles.Find(x => x.UID == CarID);
                if (veh != null)
                {
                    veh.Loaded = false;
                    veh.UpdateParking(-1);
                    veh.RealVehicle.Dispose();
                }
            }
            GameMode.db.DeleteParking(UID);
        }
        public void SetOwner(int owner)
        {
            Owner = owner;
            GameMode.db.UpdateParking_owner(UID, owner);
        }
        public void SetHouseId(int houseid)
        {
            HouseID = houseid;
            GameMode.db.UpdateParking_houseid(UID, houseid);
        }
        public void SetCar(int id)
        {
            CarID = id;
            GameMode.db.UpdateParking_carid(UID, id);
        }
        public void UpdatePos(Vector3 pos, float angle)
        {
            Position = pos;
            Rotation = angle;
            GameMode.db.UpdateParking_pos(UID, pos.X, pos.Y, pos.Z, angle);
        }
        public void InsertMe()
        {
            if (_new)
                GameMode.db.InsertParking(Position.X, Position.Y, Position.Z, Rotation, Owner, HouseID, CarID);
            _new = false;
            UID = int.Parse(GameMode.db.LAST_INSERT_ID().data[0][0]);
        }
    }
}
