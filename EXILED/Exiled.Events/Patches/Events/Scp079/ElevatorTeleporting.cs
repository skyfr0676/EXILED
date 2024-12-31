// -----------------------------------------------------------------------
// <copyright file="ElevatorTeleporting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp079ElevatorStateChanger.ServerProcessCmd(NetworkReader)" />.
    /// Adds the <see cref="Scp079.ElevatorTeleporting" /> event for SCP-079.
    /// </summary>
    [EventPatch(typeof(Scp079), nameof(Scp079.ElevatorTeleporting))]
    [HarmonyPatch(typeof(Scp079ElevatorStateChanger), nameof(Scp079ElevatorStateChanger.ServerProcessCmd))]
    internal static class ElevatorTeleporting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(ElevatorTeleportingEventArgs));

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_2) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(base.Owner)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<Scp079Role>), nameof(StandardSubroutine<Scp079Role>.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // base.CurrentCamSync.CurrentCamera.Room
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Scp079AbilityBase), nameof(Scp079AbilityBase.CurrentCamSync))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync.CurrentCamera))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079Camera), nameof(Scp079Camera.Room))),

                // chamber
                new(OpCodes.Ldloc_2),

                // (float)this._cost
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp079ElevatorStateChanger), nameof(Scp079ElevatorStateChanger._cost))),
                new(OpCodes.Conv_R4),

                // ElevatorTeleportingEventArgs ev = new(Player, RoomIdentifier, ElevatorChamber, float)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ElevatorTeleportingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Scp079.OnElevatorTeleporting(ev);
                new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnElevatorTeleporting))),

                // if (!ev.IsAllowed)
                //   return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ElevatorTeleportingEventArgs), nameof(ElevatorTeleportingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(Scp079ElevatorStateChanger), nameof(Scp079ElevatorStateChanger._cost)))) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // ev.AuxiliaryPowerCost
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ElevatorTeleportingEventArgs), nameof(ElevatorTeleportingEventArgs.AuxiliaryPowerCost))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}