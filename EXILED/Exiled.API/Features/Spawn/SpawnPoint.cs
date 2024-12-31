// -----------------------------------------------------------------------
// <copyright file="SpawnPoint.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using Exiled.API.Interfaces;
    using UnityEngine;

    /// <summary>
    /// Defines item spawn properties.
    /// </summary>
    public abstract class SpawnPoint : IPosition
    {
        /// <summary>
        /// Gets or sets this spawn point name.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the spawn chance.
        /// </summary>
        public abstract float Chance { get; set; }

        /// <summary>
        /// Gets or sets this spawn point position.
        /// </summary>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// Deconstructs the class into usable variables.
        /// </summary>
        /// <param name="chance"><inheritdoc cref="Chance"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        public void Deconstruct(out float chance, out Vector3 position)
        {
            chance = Chance;
            position = Position;
        }
    }
}