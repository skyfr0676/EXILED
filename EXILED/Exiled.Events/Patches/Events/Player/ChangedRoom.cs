// -----------------------------------------------------------------------
// <copyright file="ChangedRoom.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using LabApi.Events.Handlers;
    using MapGeneration;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CurrentRoomPlayerCache.ValidateCache"/> to add the <see cref="Player.RoomChanged"/> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.RoomChanged))]
    [HarmonyPatch(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache.ValidateCache))]
    internal class ChangedRoom
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label jump = generator.DefineLabel();
            int offset = 2;
            int index = newInstructions.FindIndex(x => x == (object)Method(typeof(PlayerEvents), "op_Inequality", new[] { typeof(Object), typeof(Object) })) + offset;

            newInstructions[index].labels.Add(jump);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // oldRoom
                new(OpCodes.Ldloc_2),

                // newRoom
                new(OpCodes.Ldloc_3),

                // if (oldRoom == newRoom) goto jump;
                new(OpCodes.Call, Method(typeof(object), nameof(object.ReferenceEquals), new[] { typeof(object), typeof(object) })),
                new(OpCodes.Brtrue_S, jump),

                // this._roleManager.gameObject.GetComponent<ReferenceHub>();
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CurrentRoomPlayerCache), nameof(CurrentRoomPlayerCache._roleManager))),
                new(OpCodes.Call, Method(typeof(Component), nameof(Component.GetComponent)).MakeGenericMethod(typeof(ReferenceHub))),

                // oldRoom
                new(OpCodes.Ldloc_2),

                // newRoom
                new(OpCodes.Ldloc_3),

                // RoomChangedEventArgs ev = new RoomChangedEventArgs(hub, oldRoom, newRoom);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoomChangedEventArgs))[0]),

                // Handlers.Player.OnRoomChanged(ev);
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnRoomChanged))),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
