// -----------------------------------------------------------------------
// <copyright file="FirearmPickup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Firearms;

    using BaseFirearm = InventorySystem.Items.Firearms.FirearmPickup;

    /// <summary>
    /// A wrapper class for a Firearm pickup.
    /// </summary>
    public class FirearmPickup : Pickup, IWrapper<BaseFirearm>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseFirearm"/> class.</param>
        internal FirearmPickup(BaseFirearm pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal FirearmPickup(ItemType type)
            : base(type)
        {
            Base = (BaseFirearm)((Pickup)this).Base;
            IsDistributed = true;

            // TODO not finish
            /*
            if (type is ItemType.ParticleDisruptor && Status.Ammo == 0)
                Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, 0);*/
        }

        /// <summary>
        /// Gets the <see cref="BaseFirearm"/> that this class is encapsulating.
        /// </summary>
        public new BaseFirearm Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is already distributed.
        /// </summary>
        public bool IsDistributed { get; set; }

        // TODO NOT FINISH
        /*{
            get => Base.Distributed;
            set => Base.Distributed = value;
        }*/

        // TODO not finish

        /*
        /// <summary>
        /// Gets or sets the <see cref="FirearmStatus"/>.
        /// </summary>
        public FirearmStatus Status
        {
            get => Base.NetworkStatus;
            set => Base.NetworkStatus = value;
        }
        */

        /// <summary>
        /// Gets or sets a value indicating how many ammo have this <see cref="FirearmPickup"/>.
        /// </summary>
        /// <remarks>This will be updated only when item will be picked up.</remarks>
        public int Ammo { get; set; }

        /*
        /// <summary>
        /// Gets or sets the <see cref="FirearmStatusFlags"/>.
        /// </summary>
        public FirearmStatusFlags Flags
        {
            get => Base.NetworkStatus.Flags;
            set => Base.NetworkStatus = new(Base.NetworkStatus.Ammo, value, Base.NetworkStatus.Attachments);
        }
        */

        /// <summary>
        /// Gets or sets a value indicating whether the attachment code have this <see cref="FirearmPickup"/>.
        /// </summary>
        public uint Attachments
        {
            get => Base.Worldmodel.AttachmentCode;
            set => Base.Worldmodel.AttachmentCode = value;
        }

        /// <summary>
        /// Returns the FirearmPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing FirearmPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{IsDistributed}| -{/*Ammo*/0}-";
    }
}
