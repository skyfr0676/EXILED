// -----------------------------------------------------------------------
// <copyright file="StartingRecallEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information before SCP-049 begins recalling a player.
    /// </summary>
    public class StartingRecallEventArgs : IScp049Event, IRagdollEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingRecallEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player"/>
        /// </param>
        /// <param name="ragdoll">
        /// <inheritdoc cref="Ragdoll"/>
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed"/>
        /// </param>
        public StartingRecallEventArgs(Player player, Ragdoll ragdoll, bool isAllowed = true)
        {
            Ragdoll = ragdoll;
            Target = Ragdoll.Owner;
            Scp049 = player.Role.As<Scp049Role>();
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's getting recalled.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the recall can begin.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp049Role Scp049 { get; }

        /// <summary>
        /// Gets the Ragdoll who's getting recalled.
        /// </summary>
        public Ragdoll Ragdoll { get; }
    }
}
