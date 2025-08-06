// -----------------------------------------------------------------------
// <copyright file="Chamber.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Lockers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Pickups;
    using MapGeneration.Distributors;
    using UnityEngine;

    /// <summary>
    /// A wrapper for <see cref="LockerChamber"/>.
    /// </summary>
    public class Chamber : IWrapper<LockerChamber>, IWorldSpace
    {
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> with <see cref="LockerChamber"/> and <see cref="Chamber"/>.
        /// </summary>
        internal static readonly Dictionary<LockerChamber, Chamber> Chambers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Chamber"/> class.
        /// </summary>
        /// <param name="chamber"><see cref="LockerChamber"/> instance.</param>
        /// <param name="locker"><see cref="Lockers.Locker"/> where this chamber is located.</param>
        public Chamber(LockerChamber chamber, Locker locker)
        {
            Base = chamber;
            Locker = locker;
            Id = (byte)Array.IndexOf(locker.Base.Chambers, chamber);

            Chambers.Add(chamber, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Chamber"/> which contains all the <see cref="Chamber"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Chamber> List => Chambers.Values;

        /// <inheritdoc/>
        public LockerChamber Base { get; }

        /// <summary>
        /// Gets the <see cref="Lockers.Locker"/> where this chamber is located at.
        /// </summary>
        public Locker Locker { get; }

        /// <inheritdoc/>
        public Vector3 Position => Base.transform.position;

        /// <inheritdoc/>
        public Quaternion Rotation => Base.transform.rotation;

        /// <summary>
        /// Gets the <see cref="Chamber"/> <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the <see cref="Chamber"/> <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets or sets all pickups that should be spawned when the door is initially opened.
        /// </summary>
        public IEnumerable<Pickup> ToBeSpawned
        {
            get => Base.ToBeSpawned.Select(Pickup.Get);
            set
            {
                Base.ToBeSpawned.Clear();

                foreach (Pickup pickup in value)
                    Base.ToBeSpawned.Add(pickup.Base);
            }
        }

        /// <summary>
        /// Gets or sets all spawn points.
        /// </summary>
        /// <remarks>
        /// Used if <see cref="UseMultipleSpawnpoints"/> is set to <see langword="true"/>.
        /// </remarks>
        public IEnumerable<Transform> Spawnpoints
        {
            get => Base._spawnpoints;
            set => Base._spawnpoints = value.ToArray();
        }

        /// <summary>
        /// Gets or sets all the acceptable items which can be spawned in this chamber.
        /// </summary>
        public IEnumerable<ItemType> AcceptableTypes
        {
            get => Base.AcceptableItems;
            set => Base.AcceptableItems = value.ToArray();
        }

        /// <summary>
        /// Gets or sets required permissions to open this chamber.
        /// </summary>
        public KeycardPermissions RequiredPermissions
        {
            get => (KeycardPermissions)Base.RequiredPermissions;
            set => Base.RequiredPermissions = (Interactables.Interobjects.DoorUtils.DoorPermissionFlags)value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple spawn points  should be used.
        /// </summary>
        /// <remarks>
        /// If <see langword="true"/>, <see cref="Spawnpoints"/> will be used over <see cref="Spawnpoint"/>.
        /// </remarks>
        public bool UseMultipleSpawnpoints
        {
            get => Base._useMultipleSpawnpoints;
            set => Base._useMultipleSpawnpoints = value;
        }

        /// <summary>
        /// Gets or sets a spawn point for the items in the chamber.
        /// </summary>
        /// <remarks>
        /// Used if <see cref="UseMultipleSpawnpoints"/> is set to <see langword="false"/>.
        /// </remarks>
        public Transform Spawnpoint
        {
            get => Base.Spawnpoint;
            set => Base.Spawnpoint = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether items should be spawned as soon as they one chamber is opened.
        /// </summary>
        public bool InitiallySpawn
        {
            get => Base.SpawnOnFirstChamberOpening;
            set => Base.SpawnOnFirstChamberOpening = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before a player can interact with the chamber again.
        /// </summary>
        public float Cooldown
        {
            get => Base.TargetCooldown;
            set => Base.TargetCooldown = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the chamber is currently open.
        /// </summary>
        public bool IsOpen
        {
            get => Base.IsOpen;
            set
            {
                Base.SetDoor(value, null);
                Locker.Base.RefreshOpenedSyncvar();
            }
        }

        /// <summary>
        /// Gets the id of this chamber in <see cref="Locker"/>.
        /// </summary>
        public byte Id { get; }

        /// <summary>
        /// Gets the <see cref="Stopwatch"/> of current cooldown.
        /// </summary>
        /// <remarks>Used in <see cref="CanInteract"/> check.</remarks>
        public Stopwatch CurrentCooldown => Base._stopwatch;

        /// <summary>
        /// Gets a value indicating whether the chamber is interactable.
        /// </summary>
        public bool CanInteract => Base.CanInteract;

        /// <summary>
        /// Adds an item to the current chamber.
        /// </summary>
        /// <param name="item">The pickup to add.</param>
        public void AddItem(Pickup item)
        {
            Transform parent = UseMultipleSpawnpoints && Spawnpoints.Any()
                ? Spawnpoints.GetRandomValue()
                : Spawnpoint;

            if (IsOpen)
            {
                item.Transform.SetParent(parent);

                if (!item.IsSpawned)
                    item.Spawn();

                return;
            }

            // If the item is already spawned on the network, unspawn it before proceeding.
            if (item.IsSpawned)
                item.UnSpawn();

            // Set the item's parent transform.
            item.Transform.SetParent(parent);

            // Lock the item in place.
            item.IsLocked = true;

            // Notify any pickup distributor triggers.
            (item.Base as IPickupDistributorTrigger)?.OnDistributed();

            // If the item has a Rigidbody component, make it kinematic and reset its position and rotation.
            if (item.Rigidbody != null)
            {
                item.Rigidbody.isKinematic = true;
                item.Rigidbody.transform.localPosition = Vector3.zero;
                item.Rigidbody.transform.localRotation = Quaternion.identity;

                // Add the Rigidbody to the list of bodies to be unfrozen later.
                SpawnablesDistributorBase.BodiesToUnfreeze.Add(item.Rigidbody);
            }

            Base.Content.Add(item.Base);
            item.Spawn();
            if (Base.WasEverOpened)
                item.IsLocked = false;
        }

        /// <summary>
        /// Spawns a specified item from <see cref="AcceptableTypes"/>.
        /// </summary>
        /// <param name="type"><see cref="ItemType"/> from <see cref="AcceptableTypes"/>.</param>
        /// <param name="amount">Amount of items that should be spawned.</param>
        public void SpawnItem(ItemType type, int amount) => Base.SpawnItem(type, amount);

        /// <summary>
        /// Adds an item of the specified type to the chamber's spawn list.
        /// If the chamber is open and <paramref name="spawnIfIsOpen"/> is set to <see langword="true"/>,
        /// the item is spawned immediately at a random spawn point within the chamber.
        /// </summary>
        /// <param name="itemType">The type of item to add to the spawn list.</param>
        /// <param name="quantity">The number of items to add. Defaults to 1.</param>
        /// <param name="spawnIfIsOpen">
        /// If <see langword="true"/> and the chamber is open, the item is immediately spawned at a random spawn point.
        /// Otherwise, the item is added to the spawn list and will spawn when the chamber is opened.
        /// </param>
        public void AddItemToSpawn(ItemType itemType, int quantity = 1, bool spawnIfIsOpen = false)
        {
            for (int i = 0; i < quantity; i++)
            {
                Pickup pickup = Pickup.Create(itemType);

                if (spawnIfIsOpen && IsOpen)
                {
                    pickup.Position = GetRandomSpawnPoint();
                    pickup.Spawn();
                    continue;
                }

                Base.ToBeSpawned.Add(pickup.Base);
            }
        }

        /// <summary>
        /// Gets a random spawn point within the chamber.
        /// If multiple spawn points are available and <see cref="UseMultipleSpawnpoints"/> is <see langword="true"/>,
        /// a random spawn point is selected from the available points.
        /// Otherwise, the default spawn point is used.
        /// </summary>
        /// <returns>A <see cref="Vector3"/> representing the position of the selected spawn point.</returns>
        public Vector3 GetRandomSpawnPoint()
        {
            if (UseMultipleSpawnpoints && Spawnpoints.Any())
            {
                return Spawnpoints.GetRandomValue().position;
            }

            return Spawnpoint.position;
        }

        /// <summary>
        /// Gets the chamber by its <see cref="LockerChamber"/>.
        /// </summary>
        /// <param name="chamber"><see cref="LockerChamber"/>.</param>
        /// <returns><see cref="Chamber"/>.</returns>
        internal static Chamber Get(LockerChamber chamber) => chamber == null ? null : Chambers.TryGetValue(chamber, out Chamber chmb) ? chmb : new(chamber, Locker.Get(x => x.Chambers.Any(x => x.Base == chamber)).FirstOrDefault());
    }
}
