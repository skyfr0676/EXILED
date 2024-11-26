// -----------------------------------------------------------------------
// <copyright file="SendingCommandEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Enums;
    using API.Features;
    using Interfaces;

    /// <summary>
    /// Contains all information before a player sends a command.
    /// </summary>
    public class SendingCommandEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendingCommandEventArgs"/> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player"/>
        /// </param>
        /// <param name="command">
        /// <inheritdoc cref="Command"/>
        /// </param>
        /// <param name="arguments">
        /// <inheritdoc cref="Arguments"/>
        /// </param>
        /// <param name="type">
        /// <inheritdoc cref="Type"/>
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed"/>
        /// </param>
        public SendingCommandEventArgs(Player player, string command, string[] arguments, CommandType type, bool isAllowed = true)
        {
            Player = player;
            Command = command;
            Arguments = arguments;
            Type = type;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who executed the command. can be <see cref="Exiled.API.Features.Server.Host"/>.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the main command used.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets or Sets the arguments used in the command. can be null if the user set no arguments.
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// Gets where the command come from.
        /// </summary>
        public CommandType Type { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the command can be executed (or show "invalid command message" if it's invalid).
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets Command and arguments combined to facilitate <see cref="Patches.Events.Player.SendingCommand"/> patch.
        /// </summary>
        public string[] ArgumentsAndCommand => HarmonyLib.CollectionExtensions.AddRangeToArray(new[] { Command }, Arguments);
    }
}