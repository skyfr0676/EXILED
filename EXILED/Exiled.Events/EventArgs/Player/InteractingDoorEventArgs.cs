// -----------------------------------------------------------------------
// <copyright file="InteractingDoorEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Doors;
    using Interactables;
    using Interactables.Interobjects.DoorUtils;
    using Interfaces;

    /// <summary>
    /// Contains all information before a player interacts with a door.
    /// </summary>
    public class InteractingDoorEventArgs : IPlayerEvent, IDoorEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingDoorEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="door">
        /// <inheritdoc cref="Door" />
        /// </param>
        /// <param name="colliderId">
        /// <inheritdoc cref="ColliderId"/>
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public InteractingDoorEventArgs(Player player, DoorVariant door, byte colliderId, bool isAllowed)
        {
            Player = player;
            Door = Door.Get(door);
            ColliderId = colliderId;
            Collider = InteractableCollider.TryGetCollider(door, colliderId, out InteractableCollider interactableCollider) ? interactableCollider : null;
            IsAllowed = isAllowed;
            CanInteract = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player can interact with the door.
        /// </summary>
        public bool CanInteract { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can access the door.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="API.Features.Doors.Door"/> instance.
        /// </summary>
        public Door Door { get; set; }

        /// <summary>
        /// Gets the <see cref="InteractableCollider"/> instance that the player interacted with.
        /// </summary>
        public InteractableCollider Collider { get; }

        /// <summary>
        /// Gets the ColliderId of <see cref="InteractableCollider"/> that the player interacted with.
        /// </summary>
        public byte ColliderId { get; }

        /// <summary>
        /// Gets the player who's interacting with the door.
        /// </summary>
        public Player Player { get; }
    }
}
