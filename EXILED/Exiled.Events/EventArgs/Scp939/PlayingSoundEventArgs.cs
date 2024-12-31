// -----------------------------------------------------------------------
// <copyright file="PlayingSoundEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp939.Mimicry;

    /// <summary>
    /// Contains all information before SCP-939 plays a sound effect.
    /// </summary>
    public class PlayingSoundEventArgs : IScp939Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayingSoundEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="sound">
        /// The sound that is being played.
        /// </param>
        /// <param name="isReady">
        /// Whether SCP-939's environmental mimicry cooldown is ready.
        /// </param>
        /// <param name="cooldown">
        /// The cooldown of the environmental mimicry.
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public PlayingSoundEventArgs(Player player, EnvMimicrySequence sound, bool isReady, float cooldown, bool isAllowed = true)
        {
            Player = player;
            Scp939 = Player.Role.As<Scp939Role>();
            Sound = sound;
            IsReady = isReady;
            Cooldown = cooldown;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-939 can play the sound.
        /// </summary>
        /// <remarks>This will default to <see langword="false"/> if <see cref="IsReady"/> is <see langword="false"/>. In this case, setting it to <see langword="true"/> will override the cooldown.</remarks>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the sound being played.
        /// </summary>
        public EnvMimicrySequence Sound { get; }

        /// <summary>
        /// Gets a value indicating whether SCP-939's environmental mimicry cooldown is ready.
        /// </summary>
        public bool IsReady { get; }

        /// <summary>
        /// Gets or sets a value indicating SCP-939's environmental mimicry cooldown.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }
    }
}