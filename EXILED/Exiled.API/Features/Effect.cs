// -----------------------------------------------------------------------
// <copyright file="Effect.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.ComponentModel;

    using CustomPlayerEffects;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    /// <summary>
    /// Useful class to save effect-related configs cleanly.
    /// </summary>
    public class Effect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        public Effect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        /// <param name="statusEffectBase">Get all the information of the effect>.</param>
        public Effect(StatusEffectBase statusEffectBase)
        {
            if (!statusEffectBase.TryGetEffectType(out EffectType effect))
                Log.Error($"EffectType not found please report to Exiled BugReport : {statusEffectBase}");
            Type = effect;
            Duration = statusEffectBase.Duration;
            Intensity = statusEffectBase.Intensity;
            IsEnabled = statusEffectBase.IsEnabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        /// <param name="type">The type of the effect>.</param>
        /// <param name="duration">The duration of the effect, in seconds.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="addDurationIfActive">Whether the effect will add duration onto the effect if already active or not.</param>
        /// <param name="isEnabled">Whether the effect should be enabled.</param>
        public Effect(EffectType type, float duration, byte intensity = 1, bool addDurationIfActive = false, bool isEnabled = true)
        {
            Type = type;
            Duration = duration;
            Intensity = intensity;
            AddDurationIfActive = addDurationIfActive;
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Gets or sets the effect type.
        /// </summary>
        [Description("The effect type")]
        public EffectType Type { get; set; }

        /// <summary>
        /// Gets or sets the effect duration.
        /// </summary>
        [Description("The effect duration")]
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets the effect intensity.
        /// </summary>
        [Description("The effect intensity")]
        public byte Intensity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the effect will add duration onto the effect if already active or not.
        /// </summary>
        [Description("If the effect is already active, setting to true will add this duration onto the effect.")]
        public bool AddDurationIfActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the effect should be enabled.
        /// </summary>
        [Description("Indicates whether the effect should be enabled")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Returns the effect in a human-readable format.
        /// </summary>
        /// <returns>A string containing effect-related data.</returns>
        public override string ToString() => $"({Type}) {Duration} {AddDurationIfActive}";
    }
}