// -----------------------------------------------------------------------
// <copyright file="IssuingMute.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using VoiceChat;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="VoiceChatMutes.IssueLocalMute(string, bool)" />.
    /// Adds the <see cref="Handlers.Player.IssuingMute" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.IssuingMute))]
    [HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.IssueLocalMute))]
    internal static class IssuingMute
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(IssuingMuteEventArgs));

            const int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldsfld) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(userId)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(string) })),

                    // intercom
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // IssuingMuteEventArgs ev = new(Player, bool, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(IssuingMuteEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnIssuingMute(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnIssuingMute))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(IssuingMuteEventArgs), nameof(IssuingMuteEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // intercom = ev.IsIntercom
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(IssuingMuteEventArgs), nameof(IssuingMuteEventArgs.IsIntercom))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}