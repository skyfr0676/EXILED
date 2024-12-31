// -----------------------------------------------------------------------
// <copyright file="DisguisedEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information after SCP-3114 disguised.
    /// </summary>
    public class DisguisedEventArgs : IScp3114Event, IRagdollEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisguisedEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="ragdoll">
        /// <inheritdoc cref="Ragdoll" />
        /// </param>
        public DisguisedEventArgs(Player player, Ragdoll ragdoll)
        {
            Player = player;
            Scp3114 = Player.Role.As<Scp3114Role>();
            Ragdoll = ragdoll;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <inheritdoc/>
        public Ragdoll Ragdoll { get; }
    }
}