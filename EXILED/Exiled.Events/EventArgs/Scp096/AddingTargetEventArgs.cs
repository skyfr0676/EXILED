// -----------------------------------------------------------------------
// <copyright file="AddingTargetEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;

    using Interfaces;

    using Scp096Role = API.Features.Roles.Scp096Role;

    /// <summary>
    /// Contains all information before adding a target to SCP-096.
    /// </summary>
    public class AddingTargetEventArgs : IScp096Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingTargetEventArgs" /> class.
        /// </summary>
        /// <param name="scp096">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        /// <param name="isLooking">
        /// <inheritdoc cref="IsLooking" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public AddingTargetEventArgs(Player scp096, Player target, bool isLooking, bool isAllowed = true)
        {
            Player = scp096;
            Scp096 = scp096.Role.As<Scp096Role>();
            Target = target;
            IsLooking = isLooking;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Player" /> that is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp096Role Scp096 { get; }

        /// <summary>
        /// Gets the <see cref="Player" /> being added as a target.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets a value indicating whether the target was being target cause of looking it's face.
        /// </summary>
        public bool IsLooking { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the target is allowed to be added.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}