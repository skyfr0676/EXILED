// -----------------------------------------------------------------------
// <copyright file="Scp1507.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using System;

    using Exiled.Events.EventArgs.Scp1507;
    using Exiled.Events.Features;

#pragma warning disable SA1623

    /// <summary>
    /// SCP-1507 related events.
    /// </summary>
    [Obsolete("Only availaible for Christmas and AprilFools.")]
    public static class Scp1507
    {
        /// <summary>
        /// Invokes before SCP-1507 attacks door.
        /// </summary>
        public static Event<AttackingDoorEventArgs> AttackingDoor { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-1507 screams.
        /// </summary>
        public static Event<ScreamingEventArgs> Screaming { get; set; } = new();

        /// <summary>
        /// Invoked before flamingos get spawned.
        /// </summary>
        public static Event<SpawningFlamingosEventArgs> SpawningFlamingos { get; set; } = new();

        /// <summary>
        /// Invoked before tape is used.
        /// </summary>
        public static Event<UsingTapeEventArgs> UsingTape { get; set; } = new();

        /// <summary>
        /// Called before SCP-1507 attacks door.
        /// </summary>
        /// <param name="ev">The <see cref="AttackingDoorEventArgs"/> instance.</param>
        public static void OnAttackingDoor(AttackingDoorEventArgs ev) => AttackingDoor.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-1507 screams.
        /// </summary>
        /// <param name="ev">The <see cref="ScreamingEventArgs"/> instance.</param>
        public static void OnScreaming(ScreamingEventArgs ev) => Screaming.InvokeSafely(ev);

        /// <summary>
        /// Called before flamingos get spawned.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningFlamingosEventArgs"/> instance.</param>
        public static void OnSpawningFlamingos(SpawningFlamingosEventArgs ev) => SpawningFlamingos.InvokeSafely(ev);

        /// <summary>
        /// Called before tape is used.
        /// </summary>
        /// <param name="ev">The <see cref="UsingTapeEventArgs"/> instance.</param>
        public static void OnUsingTape(UsingTapeEventArgs ev) => UsingTape.InvokeSafely(ev);
    }
}