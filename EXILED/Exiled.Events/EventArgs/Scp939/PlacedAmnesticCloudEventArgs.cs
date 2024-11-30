// -----------------------------------------------------------------------
// <copyright file="PlacedAmnesticCloudEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using API.Features.Hazards;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp939;

    using Scp939Role = API.Features.Roles.Scp939Role;

    /// <summary>
    /// Contains all information after SCP-939 used its amnestic cloud ability.
    /// </summary>
    public class PlacedAmnesticCloudEventArgs : IScp939Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacedAmnesticCloudEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        /// <inheritdoc cref="ReferenceHub" />
        /// </param>
        /// <param name="cloud">
        /// <inheritdoc cref="PlayerRoles.PlayableScps.Scp939.Scp939AmnesticCloudInstance" />
        /// </param>
        public PlacedAmnesticCloudEventArgs(ReferenceHub hub, Scp939AmnesticCloudInstance cloud)
        {
            Player = Player.Get(hub);
            AmnesticCloud = new AmnesticCloudHazard(cloud);
            Scp939 = Player.Role.As<Scp939Role>();
        }

        /// <summary>
        /// Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="AmnesticCloudHazard"/> instance.
        /// </summary>
        public AmnesticCloudHazard AmnesticCloud { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }
    }
}