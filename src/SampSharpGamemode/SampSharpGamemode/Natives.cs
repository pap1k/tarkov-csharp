using SampSharp.Core.Natives.NativeObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharpGamemode
{
    public class MyCustomNatives
    {
        [NativeMethod]
        public virtual int SendClientCheck(int playerid, int type, int arg, int offset, int size)
        {
            throw new NativeNotImplementedException();
        }
    }
}
