// -----------------------------------------------------------------------
// <copyright file="RespawningTeam.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.API.Features.Waves;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles;
    using Respawning.Waves;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patch the <see cref="WaveSpawner.SpawnWave" />.
    /// Adds the <see cref="Server.RespawningTeam" /> event.
    /// </summary>
    [EventPatch(typeof(Server), nameof(Server.RespawningTeam))]
    [HarmonyPatch(typeof(WaveSpawner), nameof(WaveSpawner.SpawnWave))]
    internal static class RespawningTeam
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(SpawnableWaveBase), nameof(SpawnableWaveBase.PopulateQueue)))) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(RespawningTeamEventArgs));

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // GetPlayers(list);
                    new CodeInstruction(OpCodes.Ldloc_S, 2).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetPlayers))),

                    // maxWaveSize
                    new(OpCodes.Ldloc_3),

                    // wave
                    new(OpCodes.Ldarg_0),

                    // RespawningTeamEventArgs ev = new(players, num, wave)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RespawningTeamEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Server.OnRespawningTeam(ev)
                    new(OpCodes.Call, Method(typeof(Server), nameof(Server.OnRespawningTeam))),

                    // if (ev.IsAllowed)
                    //    goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // this.NextKnownTeam == null
                    // return;
                    new(OpCodes.Ldsfld, Field(typeof(WaveSpawner), nameof(WaveSpawner.SpawnQueue))),
                    new(OpCodes.Callvirt, Method(typeof(Queue<RoleTypeId>), nameof(Queue<RoleTypeId>.Clear))),
                    new(OpCodes.Newobj, Constructor(typeof(List<ReferenceHub>), new Type[0])),
                    new(OpCodes.Ret),

                    // load "ev" four times
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // num = ev.MaximumRespawnAmount
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.MaximumRespawnAmount))),
                    new(OpCodes.Stloc_3),

                    // list = GetHubs(ev.Players)
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.Players))),
                    new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetHubs))),
                    new(OpCodes.Stloc_S, 2),

                    // wave = ev.Wave.Base;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.Wave))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TimedWave), nameof(TimedWave.Base))),
                    new(OpCodes.Starg_S, 0),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static List<Player> GetPlayers(List<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();

        private static List<ReferenceHub> GetHubs(List<Player> players) => players.Select(player => player.ReferenceHub).ToList();

        private static void RefillQueue(Queue<RoleTypeId> newQueue)
        {
            WaveSpawner.SpawnQueue.Clear();
            foreach (RoleTypeId role in newQueue)
                WaveSpawner.SpawnQueue.Enqueue(role);
        }
    }
}
