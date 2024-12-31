// -----------------------------------------------------------------------
// <copyright file="RevealingEventArgs.cs" company="ExMod Team">
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
    /// Contains all information before SCP-3114 reveals.
    /// </summary>
    public class RevealingEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevealingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isManualReveal">
        /// <inheritdoc cref="IsManualReveal" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public RevealingEventArgs(Player player, bool isManualReveal, bool isAllowed = true)
        {
            Player = player;
            Scp3114 = Player.Role.As<Scp3114Role>();
            IsManualReveal = isManualReveal;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <summary>
        /// Gets a value indicating whether the reveal is manual.
        /// </summary>
        public bool IsManualReveal { get; }
    }
}