// -----------------------------------------------------------------------
// <copyright file="DrinkingCoffee.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable CS0618 // Le type ou le membre est obsolète
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="global::Coffee.ServerInteract"/>
    /// to add <see cref="Handlers.Player.DrinkingCoffee"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.DrinkingCoffee))]
    [HarmonyPatch(typeof(global::Coffee), nameof(global::Coffee.ServerInteract))]
    internal class DrinkingCoffee
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            Label retLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub);
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Coffee.Get(this)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Coffee), nameof(Coffee.Get), new[] { typeof(global::Coffee) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // DrinkingCoffeeEventArgs ev = new(Player, Coffee, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DrinkingCoffeeEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnDrinkingCoffee(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDrinkingCoffee))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DrinkingCoffeeEventArgs), nameof(DrinkingCoffeeEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}