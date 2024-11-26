// -----------------------------------------------------------------------
// <copyright file="CommandType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Used in SendingCommandEventArgs and SendingValidCommandEventArgs, indicate if the command used is coming from <see cref="Server"/>, <see cref="Client"/> or <see cref="RemoteAdmin"/>.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Indicate if the command is coming from server (through panel for example).
        /// </summary>
        Server,

        /// <summary>
        /// Indicate if the command is coming from client console.
        /// </summary>
        Client,

        /// <summary>
        /// Indicate if the command is coming from the remote admin.
        /// </summary>
        RemoteAdmin,
    }
}