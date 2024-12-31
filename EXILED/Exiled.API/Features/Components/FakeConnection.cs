// -----------------------------------------------------------------------
// <copyright file="FakeConnection.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Components
{
    using System;

    using Mirror;

    /// <summary>
    /// A fake network connection.
    /// </summary>
    public class FakeConnection : NetworkConnectionToClient
    {
        /// <inheritdoc cref="NetworkConnectionToClient"/>
        public FakeConnection(int networkConnectionId)
            : base(networkConnectionId)
        {
        }

        /// <inheritdoc />
        public override string address => "localhost";

        /// <inheritdoc />
        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }
    }
}