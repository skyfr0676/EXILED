// -----------------------------------------------------------------------
// <copyright file="IScp1507Event.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using System;

    using Exiled.API.Features.Roles;

    /// <summary>
    /// Event args used for all <see cref="Scp1507Role"/> related events.
    /// </summary>
    [Obsolete("Only availaible for Christmas and AprilFools.")]
    public interface IScp1507Event : IPlayerEvent
    {
        /// <summary>
        /// Gets the <see cref="Scp1507Role"/> triggering the event.
        /// </summary>
        public Scp1507Role Scp1507 { get; }
    }
}