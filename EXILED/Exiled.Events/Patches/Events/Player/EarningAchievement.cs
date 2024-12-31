// -----------------------------------------------------------------------
// <copyright file="EarningAchievement.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Achievements;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="AchievementHandlerBase.ServerAchieve"/>.
    /// Adds the <see cref="Handlers.Player.EarningAchievement"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.EarningAchievement))]
    [HarmonyPatch(typeof(AchievementHandlerBase), nameof(AchievementHandlerBase.ServerAchieve))]
    internal static class EarningAchievement
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label ret = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(EarningAchievementEventArgs));

            const int offset = 2;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brfalse_S) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(conn);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(NetworkConnection) })),

                    // achievementName
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // EarningAchievementEventArgs ev = new(Player, AchievementName)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EarningAchievementEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnEarningAchievement(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEarningAchievement))),

                    // if (!ev.Isallowed) return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EarningAchievementEventArgs), nameof(EarningAchievementEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, ret),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
