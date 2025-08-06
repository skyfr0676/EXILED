// -----------------------------------------------------------------------
// <copyright file="Interacting.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp559
{
#pragma warning disable CS0618
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp559;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp559Cake.ServerInteract"/>
    /// to add <see cref="Handlers.Scp559.Interacting"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp559), nameof(Handlers.Scp559.Interacting))]
    [HarmonyPatch(typeof(Scp559Cake), nameof(Scp559Cake.ServerInteract))]
    internal class Interacting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Scp559.Get(this);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Scp559), nameof(Scp559.Get), new[] { typeof(Scp559Cake) })),

                    // Player.Get(hub);
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // InteractingScp559EventArgs ev = new(Scp559, Player, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingScp559EventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp559.OnInteracting(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp559), nameof(Handlers.Scp559.OnInteracting))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp559EventArgs), nameof(InteractingScp559EventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
        }
    }
}