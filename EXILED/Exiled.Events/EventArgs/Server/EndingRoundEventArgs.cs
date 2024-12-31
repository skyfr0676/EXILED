// -----------------------------------------------------------------------
// <copyright file="EndingRoundEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using API.Enums;
    using Interfaces;

    /// <summary>
    /// Contains all information before ending a round.
    /// </summary>
    public class EndingRoundEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndingRoundEventArgs" /> class.
        /// </summary>
        /// <param name="leadingTeam">
        /// <inheritdoc cref="LeadingTeam" />
        /// </param>
        /// <param name="classList">
        /// <inheritdoc cref="RoundSummary.SumInfo_ClassList" />
        /// </param>
        /// <param name="isForceEnded">
        /// <inheritdoc cref="IsForceEnded" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public EndingRoundEventArgs(LeadingTeam leadingTeam, RoundSummary.SumInfo_ClassList classList, bool isForceEnded, bool isAllowed)
        {
            LeadingTeam = leadingTeam;
            ClassList = classList;
            IsForceEnded = isForceEnded;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the round summary class list.
        /// </summary>
        public RoundSummary.SumInfo_ClassList ClassList { get; set; }

        /// <summary>
        /// Gets or sets the leading team.
        /// </summary>
        public LeadingTeam LeadingTeam { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the round is ended by API call.
        /// </summary>
        public bool IsForceEnded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the round is going to finish or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}