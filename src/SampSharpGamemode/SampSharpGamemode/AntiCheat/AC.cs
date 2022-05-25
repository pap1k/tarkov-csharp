using SampSharp.GameMode.Events;
using SampSharp.GameMode.Definitions;
using SampSharpGamemode.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharpGamemode.AntiCheat
{
    class ACPlayer : Player
    {
        public void AC_Kick(Codes.e_codes code)
        {
            //TODO: add record to /acs logger
            kick(Codes.GetString(code));
        }
        public override void OnUpdate(PlayerUpdateEventArgs e)
        {
            if (PVars.Get<bool>(PvarsInfo.admin)) base.OnUpdate(e);
            else
            {
                if (Money != PVars.Get<int>(PvarsInfo.money))
                    AC_Kick(Codes.e_codes.MONEY);
                if (SpecialAction == SpecialAction.Usejetpack)
                    AC_Kick(Codes.e_codes.JETPACK);
                //..........
            }

        }
    }
}
