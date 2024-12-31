// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Cassie;
    using Exiled.Events.Features;

    /// <summary>
    /// Cassie related events.
    /// </summary>
    public static class Cassie
    {
#pragma warning disable SA1623 // Property summary documentation should match accessors

        /// <summary>
        /// Invoked before sending a cassie message.
        /// </summary>
        public static Event<SendingCassieMessageEventArgs> SendingCassieMessage { get; set; } = new();

        /// <summary>
        /// Called before sending a cassie message.
        /// </summary>
        /// <param name="ev">The <see cref="SendingCassieMessageEventArgs" /> instance.</param>
        public static void OnSendingCassieMessage(SendingCassieMessageEventArgs ev) => SendingCassieMessage.InvokeSafely(ev);
    }
}