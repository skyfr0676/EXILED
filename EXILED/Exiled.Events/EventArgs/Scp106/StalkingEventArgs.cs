// -----------------------------------------------------------------------
// <copyright file="StalkingEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp106;

    using Scp106Role = API.Features.Roles.Scp106Role;

    /// <summary>
    /// Contains all information before SCP-106 uses the stalk ability.
    /// </summary>
    public class StalkingEventArgs : IScp106Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StalkingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public StalkingEventArgs(Player player)
        {
            Player = player;
            Scp106 = player.Role.As<Scp106Role>();
            IsAllowed = true;
            MinimumVigor = Scp106StalkAbility.MinVigorToSubmerge;
        }

        /// <summary>
        /// Gets or sets the required minimum vigor to stalk.
        /// </summary>
        public float MinimumVigor { get; set; }

        /// <summary>
        /// Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp106Role Scp106 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-106 can stalk.
        /// </summary>
        /// <remarks>IsAllowed doesn't indicate whether vigor is sufficient for Stalking. <see cref="MinimumVigor"></see> needs to be changed to override the base game check.</remarks>
        public bool IsAllowed { get; set; }
    }
}