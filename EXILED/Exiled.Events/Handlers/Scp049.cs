// -----------------------------------------------------------------------
// <copyright file="Scp049.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp049;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-049 related events.
    /// </summary>
    public static class Scp049
    {
        /// <summary>
        /// Invoked before SCP-049 finishes reviving a player.
        /// </summary>
        public static Event<FinishingRecallEventArgs> FinishingRecall { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-049 begins reviving a player.
        /// </summary>
        public static Event<StartingRecallEventArgs> StartingRecall { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-049 uses the good sense of the doctor ability.
        /// </summary>
        public static Event<ActivatingSenseEventArgs> ActivatingSense { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-049 uses the call ability.
        /// </summary>
        public static Event<SendingCallEventArgs> SendingCall { get; set; } = new();

        /// <summary>
        /// Invoked before SCP-049 attacks player.
        /// </summary>
        public static Event<AttackingEventArgs> Attacking { get; set; } = new();

        /// <summary>
        /// Called before SCP-049 finishes reviving a player.
        /// </summary>
        /// <param name="ev">The <see cref="FinishingRecallEventArgs" /> instance.</param>
        public static void OnFinishingRecall(FinishingRecallEventArgs ev) => FinishingRecall.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-049 starts to revive a player.
        /// </summary>
        /// <param name="ev">The <see cref="StartingRecallEventArgs" /> instance.</param>
        public static void OnStartingRecall(StartingRecallEventArgs ev) => StartingRecall.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-049 starts the good sense of the doctor ability.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingSenseEventArgs" /> instance.</param>
        public static void OnActivatingSense(ActivatingSenseEventArgs ev) => ActivatingSense.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-049 starts the call ability.
        /// </summary>
        /// <param name="ev">The <see cref="SendingCallEventArgs" /> instance.</param>
        public static void OnSendingCall(SendingCallEventArgs ev) => SendingCall.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-049 attacks player.
        /// </summary>
        /// <param name="ev">The <see cref="AttackingEventArgs"/> instance.</param>
        public static void OnAttacking(AttackingEventArgs ev) => Attacking.InvokeSafely(ev);
    }
}