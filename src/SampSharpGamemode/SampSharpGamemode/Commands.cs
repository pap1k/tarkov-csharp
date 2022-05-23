using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.ParameterTypes;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SampSharpGamemode
{
    public class AllAdminPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return "У Вас нет прав на использование данной команды"; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<bool>(PvarsInfo.admin);
        }
    }
    public class DefaultPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return String.Empty; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<bool>(PvarsInfo.ingame);
        }
    }
    public class MyCommandManager : CommandsManager
    {
        public MyCommandManager(BaseMode gameMode) : base(gameMode)
        {
        }

        protected override ICommand CreateCommand(CommandPath[] commandPaths, string displayName, bool ignoreCase,
            IPermissionChecker[] permissionCheckers, MethodInfo method, string usageMessage)
        {
            // Create an instance of your own command type.
            return new MyCommand(commandPaths, displayName, ignoreCase, permissionCheckers, method, usageMessage);
        }
    }

    public class MyCommand : DefaultCommand
    {
        public MyCommand(CommandPath[] names, string displayName, bool ignoreCase,
            IPermissionChecker[] permissionCheckers, MethodInfo method, string usageMessage)
            : base(names, displayName, ignoreCase, permissionCheckers, method, usageMessage)
        {
        }
        public override bool Invoke(BasePlayer player, string commandText)
        {
            player.SendClientMessage(player.PVars.Get<bool>(PvarsInfo.ingame).ToString());
            return player.PVars.Get<bool>(PvarsInfo.ingame) ? base.Invoke(player, commandText) : false;
        }
        protected override ICommandParameterType GetParameterType(ParameterInfo parameter, int index, int count)
        {
            // Override GetParameterType to use your own automatical detection of parameter types.
            // This way, you can avoid having to attach `ParameterType` attributes to all parameters of a custom type.

            // use default parameter type detection.
            var type = base.GetParameterType(parameter, index, count);

            if (type != null)
                return type;

            // if no parameter type was found check if it's of any type we recognize.
            if (parameter.ParameterType == typeof(bool))
            {
                // TODO: detected this type to be of type `bool`. 
                // TODO: Return an implementation of ICommandParameterType which processes booleans.
            }

            // Unrecognized type. Return null.
            return null;
        }

        protected override bool SendPermissionDeniedMessage(IPermissionChecker permissionChecker, BasePlayer player)
        {
            // Override SendPermissionDeniedMessage to send permission denied messages in the way you prefer.
            if (permissionChecker == null) throw new ArgumentNullException(nameof(permissionChecker));
            if (player == null) throw new ArgumentNullException(nameof(player));

            if (permissionChecker.Message == null)
                return false;

            // Send permission denied message in red instead of white.
            player.SendClientMessage(Color.Red, permissionChecker.Message);
            return true;
        }
    }

    [Controller]
    public class MyCommandController : CommandController
    {
        public override void RegisterServices(BaseMode gameMode, GameModeServiceContainer serviceContainer)
        {
            // Register our own commands manager service instead of the default.
            CommandsManager = new MyCommandManager(gameMode);
            serviceContainer.AddService(CommandsManager);

            // Register commands in game mode.
            CommandsManager.RegisterCommands(gameMode.GetType());
        }
    }
}
