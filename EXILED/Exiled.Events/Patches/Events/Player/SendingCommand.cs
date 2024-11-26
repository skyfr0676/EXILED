// -----------------------------------------------------------------------
// <copyright file="SendingCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CommandProcessor.ProcessQuery(string, CommandSender)" />.
    /// Adds the <see cref="Handlers.Player.SendingCommand" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SendingCommand))]
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal static class SendingCommand
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_0) + 1;

            Label continueLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(SendingCommandEventArgs));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(sender)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(CommandSender) })),

                // strArray[0]
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Ldloc_0),

                // strArray.Skip<string>(1).ToArray<string>() // to get arguments
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Call, Method(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.Skip))),
                new(OpCodes.Call, Method(typeof(System.Linq.Enumerable), nameof(System.Linq.Enumerable.ToArray), null, new[] { typeof(string) })),

                // CommandType.RemoteAdmin
                new(OpCodes.Ldc_I4_2),

                // true
                new(OpCodes.Ldc_I4_0),

                // SendingCommandEventArgs ev = new SendingCommandEventArgs(Player.Get(sender), strArray[0], strArray.Skip<string>(1).ToArray<string>(), CommandType.RemoteAdmin, true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCommandEventArgs))[0]),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Exiled.Events.Handlers.Player.OnSendingCommand(ev);
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSendingCommand))),

                // if (!ev.IsAllowed)
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(SendingCommandEventArgs), nameof(SendingCommandEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                // sender.RaReply("SYSTEM#Command request has been canceled by a plugin.", false, true, string.Empty);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldstr, "SYSTEM# Command request has been canceled by a plugin."),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldfld, Field(typeof(string), nameof(string.Empty))),
                new(OpCodes.Callvirt, Method(typeof(CommandSender), nameof(CommandSender.RaReply))),

                // return "Command Request has been canceled by a plugin.";
                new(OpCodes.Ldstr, "Command request has been canceled by a plugin."),
                new(OpCodes.Ret),

                // strArray = ev.ArgumentsAndCommand;
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SendingCommandEventArgs), nameof(SendingCommandEventArgs.ArgumentsAndCommand))),
                new(OpCodes.Stloc_0),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}