// -----------------------------------------------------------------------
// <copyright file="DecontaminationState.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Features;

    /// <summary>
    /// Represents the state of a <see cref="LightContainmentZoneDecontamination.DecontaminationController"/>.
    /// </summary>
    /// <seealso cref="Map.DecontaminationState"/>
    public enum DecontaminationState
    {
        /// <summary>
        /// Decontamination is disable.
        /// </summary>
        Disabled = -1,

        /// <summary>
        /// Decontamination has started.
        /// </summary>
        Start,

        /// <summary>
        /// It's remain 10 minutes.
        /// </summary>
        Remain10Minutes,

        /// <summary>
        /// It's remain 5 minutes.
        /// </summary>
        Remain5Minutes,

        /// <summary>
        /// It's remain 1 minutes.
        /// </summary>
        Remain1Minute,

        /// <summary>
        /// It's remain 30 seconds.
        /// </summary>
        Countdown,

        /// <summary>
        /// All doors is closed lock.
        /// </summary>
        Lockdown,

        /// <summary>
        /// The decontamination has been done.
        /// </summary>
        Finish,
    }
}
