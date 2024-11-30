// -----------------------------------------------------------------------
// <copyright file="Scp018Projectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using System;
    using System.Reflection;

    using Exiled.API.Interfaces;
    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using BaseScp018Projectile = InventorySystem.Items.ThrowableProjectiles.Scp018Projectile;

    /// <summary>
    /// A wrapper class for Scp018Projectile.
    /// </summary>
    public class Scp018Projectile : TimeGrenadeProjectile, IWrapper<BaseScp018Projectile>
    {
        private static FieldInfo maxVelocityField;
        private static FieldInfo velocityPerBounceField;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp018Projectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseScp018Projectile"/> class.</param>
        public Scp018Projectile(BaseScp018Projectile pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp018Projectile"/> class.
        /// </summary>
        internal Scp018Projectile()
            : base(ItemType.SCP018)
        {
            Base = (BaseScp018Projectile)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="ExplosionGrenade"/> that this class is encapsulating.
        /// </summary>
        public new BaseScp018Projectile Base { get; }

        /// <summary>
        /// Gets or sets the pickup's PhysicsModule.
        /// </summary>
        public new Scp018Physics PhysicsModule
        {
            get => Base.PhysicsModule as Scp018Physics;
            [Obsolete("Unsafe.", true)]
            set
            {
                Base.PhysicsModule.DestroyModule();
                Base.PhysicsModule = value;
            }
        }

        /// <summary>
        /// Gets or sets the pickup's max velocity.
        /// </summary>
        public float MaxVelocity
        {
            get => PhysicsModule._maxVel;
            set
            {
                maxVelocityField ??= AccessTools.Field(typeof(Scp018Physics), nameof(Scp018Physics._maxVel));
                maxVelocityField.SetValue(PhysicsModule, value);
            }
        }

        /// <summary>
        /// Gets or sets the pickup's velocity per bounce.
        /// </summary>
        public float VelocityPerBounce
        {
            get => PhysicsModule._maxVel;
            set
            {
                velocityPerBounceField ??= AccessTools.Field(typeof(Scp018Physics), nameof(Scp018Physics._velPerBounce));
                velocityPerBounceField.SetValue(PhysicsModule, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-018 can injure teammates.
        /// </summary>
        public bool IgnoreFriendlyFire => Base.IgnoreFriendlyFire;

        /// <summary>
        /// Gets or sets the time for SCP-018 not to ignore the friendly fire.
        /// </summary>
        public float FriendlyFireTime
        {
            get => Base._friendlyFireTime;
            set => Base._friendlyFireTime = value;
        }

        /// <summary>
        /// Gets the current damage of SCP-018.
        /// </summary>
        public float Damage => Base.CurrentDamage;

        /// <summary>
        /// Returns the Scp018Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Scp018Pickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{Damage}- ={IgnoreFriendlyFire}=";
    }
}
