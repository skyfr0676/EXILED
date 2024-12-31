// -----------------------------------------------------------------------
// <copyright file="ExitStalking.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp106;
    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp106;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp106StalkAbility.ServerProcessCmd"/>.
    /// To add the <see cref="Handlers.Scp106.ExitStalking"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp106), nameof(Handlers.Scp106.ExitStalking))]
    [HarmonyPatch(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.ServerSetStalk))]
    public class ExitStalking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ExitStalkingEventArgs));

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertySetter(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.StalkActive)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (value is false) continue;
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Beq_S, continueLabel),

                    // Player.Get(this.Owner);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ExitStalking ev = new(Player, isAllowed)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExitStalkingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp106.OnExitStalking(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp106), nameof(Handlers.Scp106.OnExitStalking))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExitStalkingEventArgs), nameof(ExitStalkingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}