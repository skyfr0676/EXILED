// -----------------------------------------------------------------------
// <copyright file="DamagingDoorEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features.Doors;
    using Interactables.Interobjects.DoorUtils;
    using Interfaces;

    /// <summary>
    /// Contains all information before damage is dealt to a <see cref="DoorVariant" />.
    /// </summary>
    public class DamagingDoorEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamagingDoorEventArgs" /> class.
        /// </summary>
        /// <param name="door">
        /// <inheritdoc cref="DoorVariant" />
        /// </param>
        /// <param name="damage">The damage being dealt.</param>
        /// <param name="doorDamageType">
        /// <inheritdoc cref="DoorDamageType" />
        /// </param>
        public DamagingDoorEventArgs(DoorVariant door, float damage, DoorDamageType doorDamageType)
        {
            Door = Door.Get(door);
            Damage = damage;
            DamageType = doorDamageType;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Doors.Door" /> object that is damaged.
        /// </summary>
        public Door Door { get; }

        /// <summary>
        /// Gets or sets the damage dealt to the door.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the door can be broken.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the <see cref="DoorDamageType"/> dealt to the door.
        /// </summary>
        public DoorDamageType DamageType { get; }
    }
}