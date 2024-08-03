// -----------------------------------------------------------------------
// <copyright file="ArmorDropPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pools;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Usables.Scp330;
using UnityEngine;

namespace Exiled.Events.Patches.Generic
{
    using HarmonyLib;
    using InventorySystem.Searching;
    using PlayerRoles.PlayableScps.Scp173;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173ObserversTracker.UpdateObserver(ReferenceHub)"/>.
    /// </summary>
    [HarmonyPatch(typeof(BodyArmorUtils), nameof(BodyArmorUtils.RemoveEverythingExceedingLimits))]
    internal static class ArmorDropPatch
    {
        static void T()
        {
            int num2 = 0;
            foreach (KeyValuePair<ushort, ItemBase> keyValuePair in new Dictionary<ushort, ItemBase>())
            {
                if (keyValuePair.Value.Category is not ItemCategory.Armor and not ItemCategory.None)
                {
                    int num1 = Mathf.Abs((int) InventoryLimits.GetCategoryLimit(null, keyValuePair.Value.Category));
                    ++num2;
                    if (num2 > num1)
                        Log.Info("test");
                    Log.Info("test");
                }
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            int continueIndex = newInstructions.FindIndex(x => x.Is(OpCodes.Call, Method(typeof(Dictionary<ushort, ItemBase>.Enumerator), nameof(Dictionary<ushort, ItemBase>.Enumerator.MoveNext)))) - 1;
            newInstructions[continueIndex].WithLabels(continueLabel);

            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Ldc_I4_S, 9));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloca_S, 4),
                new(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<ushort, ItemBase>), nameof(KeyValuePair<ushort, ItemBase>.Value))),
                new(OpCodes.Ldfld, Field(typeof(ItemBase), nameof(ItemBase.Category))),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Beq_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}