// -----------------------------------------------------------------------
// <copyright file="Scp0492.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp0492;
    using Exiled.Events.Features;

    /// <summary>
    /// <see cref="API.Features.Roles.Scp0492Role"/> related events.
    /// </summary>
    public class Scp0492
    {
        /// <summary>
        /// Invoked before a player triggers the bloodlust effect for 049-2.
        /// </summary>
        public static Event<TriggeringBloodlustEventArgs> TriggeringBloodlust { get; set; } = new ();

        /// <summary>
        /// Called after 049-2 gets his benefits from consumed ability.
        /// </summary>
        public static Event<ConsumedCorpseEventArgs> ConsumedCorpse { get; set; } = new();

        /// <summary>
        /// Called before 049-2 gets his benefits from consuming ability.
        /// </summary>
        public static Event<ConsumingCorpseEventArgs> ConsumingCorpse { get; set; } = new();

        /// <summary>
        /// Called before a player triggers the bloodlust effect for 049-2.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringBloodlustEventArgs"/> instance.</param>
        public static void OnTriggeringBloodlust(TriggeringBloodlustEventArgs ev) => TriggeringBloodlust.InvokeSafely(ev);

        /// <summary>
        /// Invokes after 049-2 gets his benefits from consumed ability.
        /// </summary>
        /// <param name="ev"><inheritdoc cref="ConsumedCorpseEventArgs"/> instance.</param>
        public static void OnConsumedCorpse(ConsumedCorpseEventArgs ev) => ConsumedCorpse.InvokeSafely(ev);

        /// <summary>
        /// Invokes before 049-2 gets his benefits from consuming ability.
        /// </summary>
        /// <param name="ev"><inheritdoc cref="ConsumingCorpseEventArgs"/> instance.</param>
        public static void OnConsumingCorpse(ConsumingCorpseEventArgs ev) => ConsumingCorpse.InvokeSafely(ev);
    }
}
