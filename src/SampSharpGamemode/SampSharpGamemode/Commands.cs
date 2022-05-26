using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.ParameterTypes;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using SampSharpGamemode.Players;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SampSharpGamemode
{
    public class MyPlayerType : ICommandParameterType
    {
        public bool Parse(ref string commandText, out object output, bool isNullable = false)
        {
            var text = commandText.TrimStart();
            output = null;

            if (string.IsNullOrEmpty(text))
                return false;

            var word = text.Split(' ')
                .First();

            // find a player with a matching id.
            if (int.TryParse(word, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                var player = BasePlayer.Find(id);
                if (player != null)
                {
                    output = player;
                    commandText = commandText.Substring(word.Length)
                        .TrimStart(' ');
                    return true;
                }
            }

            var lowerWord = word.ToLower();

            // find all candidates containing the input word, case insensitive.
            var candidates = BasePlayer.All.Where(p => p.Name.ToLower()
                    .Contains(lowerWord))
                .ToList();

            // in case of ambiguities find all candidates containing the input word, case sensitive.
            if (candidates.Count > 1)
                candidates = candidates.Where(p => p.Name.Contains(word))
                    .ToList();

            // in case of ambiguities find all candidates matching exactly the input word, case insensitive.
            if (candidates.Count > 1)
                candidates = candidates.Where(p => p.Name.ToLower() == lowerWord)
                    .ToList();

            // in case of ambiguities find all candidates matching exactly the input word, case sensitive.
            if (candidates.Count > 1)
                candidates = candidates.Where(p => p.Name == word)
                    .ToList();

            if (candidates.Count == 1 || isNullable)
            {
                commandText = word.Length == commandText.Length
                    ? string.Empty
                    : commandText.Substring(word.Length)
                        .TrimStart(' ');
            }

            if (candidates.Count != 1)
                return isNullable;

            output = candidates.First();
            return true;
        }
    }
    public class EventPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return "У вас нет прав на использование данной команды"; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<bool>(PvarsInfo.isevent);
        }
    }
    public class AllAdminPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return "У вас нет прав на использование данной команды"; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<bool>(PvarsInfo.admin);
        }
    }
    public class FounderAdminPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return "У вас нет прав на использование данной команды"; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<int>(PvarsInfo.adminlevel) >= (int)e_AdminLevels.A_FOUNDER;
        }
    }
    public class ViceAdminPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return "У вас нет прав на использование данной команды"; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<int>(PvarsInfo.adminlevel) >= (int)e_AdminLevels.A_VICE;
        }
    }
    public class LeadAdminPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return "У вас нет прав на использование данной команды"; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<int>(PvarsInfo.adminlevel) >= (int)e_AdminLevels.A_LEAD;
        }
    }
    public class RedAdminPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return "У вас нет прав на использование данной команды"; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<int>(PvarsInfo.adminlevel) >= (int)e_AdminLevels.A_RED;
        }
    }
    public class HelperPermChecker : IPermissionChecker
    {
        public string Message
        {
            get { return "У вас нет прав на использование данной команды"; }
        }
        public bool Check(BasePlayer player)
        {
            return player.PVars.Get<bool>(PvarsInfo.helper);
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
            return player.PVars.Get<bool>(PvarsInfo.ingame) ? base.Invoke(player, commandText) : false;
        }
        protected override bool SendUsageMessage(BasePlayer player)
        {
            player.SendClientMessage(Colors.GREY, "Подсказка: " + UsageMessage);
            return true;
        }
        public override bool GetArguments(string commandText, out object[] arguments, out int argumentsTextLength)
        {
            arguments = new object[Parameters.Length];
            argumentsTextLength = 0;
            var index = 0;
            var textLength = commandText.Length;
            foreach (var parameter in Parameters)
            {
                if (!parameter.CommandParameterType.Parse(ref commandText, out var arg, parameter.IsNullable))
                {
                    if (!parameter.IsOptional)
                    {
                        argumentsTextLength = textLength - commandText.Length;
                        return false;
                    }

                    arguments[index] = parameter.DefaultValue;
                }
                else
                {
                    arguments[index] = arg;
                }

                index++;
            }

            argumentsTextLength = textLength - commandText.Length;
            return true;
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
            player.SendClientMessage(Colors.GREY, permissionChecker.Message);
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
