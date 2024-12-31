// -----------------------------------------------------------------------
// <copyright file="IWrapper.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    /// <summary>
    /// Defines the contract for classes that wrap a base-game object.
    /// </summary>
    /// <typeparam name="T">The base-game class that is being wrapped.</typeparam>
    public interface IWrapper<T>
    {
        /// <summary>
        /// Gets the base <typeparamref name="T"/> that this class is wrapping.
        /// </summary>
        public T Base { get; }
    }
}