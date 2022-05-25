using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharpGamemode.Vehicles
{
    class Vehicle : BaseVehicle
    {
        public bool isOpen = false;
        public override void OnStreamIn(PlayerEventArgs e)
        {
            SetParametersForPlayer(e.Player, false, isOpen);
            base.OnStreamIn(e);
        }
    }
}
