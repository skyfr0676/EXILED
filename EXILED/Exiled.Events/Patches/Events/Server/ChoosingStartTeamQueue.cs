// -----------------------------------------------------------------------
// <copyright file="ChoosingStartTeamQueue.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;
    using HarmonyLib;
    using PlayerRoles.RoleAssign;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoleAssigner.OnRoundStarted" /> setter.
    /// Adds the <see cref="Handlers.Server.ChoosingStartTeamQueue" />.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.ChoosingStartTeamQueue))]
    [HarmonyPatch(typeof(RoleAssigner), nameof(RoleAssigner.OnRoundStarted))]
    internal static class ChoosingStartTeamQueue
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Dup) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                // new ChoosingStartTeamEventArgs(string)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChoosingStartTeamQueueEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // Handlers.Server.OnChoosingStartTeam(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnChoosingStartTeam))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChoosingStartTeamQueueEventArgs), nameof(ChoosingStartTeamQueueEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new(OpCodes.Pop),
                new(OpCodes.Ret),

                // ev.GetTeamRespawnQueue();
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(ChoosingStartTeamQueueEventArgs), nameof(ChoosingStartTeamQueueEventArgs.GetTeamRespawnQueue))).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}