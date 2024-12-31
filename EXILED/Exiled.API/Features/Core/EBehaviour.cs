// -----------------------------------------------------------------------
// <copyright file="EBehaviour.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using Exiled.API.Features;

    /// <summary>
    /// <see cref="EBehaviour"/> is a versatile component designed to enhance the functionality of playable characters.
    /// <br>It can be easily integrated with various types of playable characters, making it a valuable tool for user-defined playable character behaviours.</br>
    /// </summary>
    public abstract class EBehaviour : EActor
    {
        /// <summary>
        /// Gets or sets the owner of the <see cref="EBehaviour"/>.
        /// </summary>
        public virtual Player Owner { get; protected set; }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            Owner = Player.Get(Base);
            if (Owner is null)
            {
                Destroy();
                return;
            }
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            BehaviourUpdate_Implementation();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            if (Owner is null)
                return;
        }

        /// <summary>
        /// Fired every tick.
        /// <para>Code affecting the <see cref="EBehaviour"/>'s base implementation should be placed here.</para>
        /// </summary>
        protected virtual void BehaviourUpdate()
        {
        }

        /// <inheritdoc cref="BehaviourUpdate"/>
        private protected virtual void BehaviourUpdate_Implementation()
        {
            if (Owner is null)
            {
                Destroy();
                return;
            }

            BehaviourUpdate();
        }
    }
}