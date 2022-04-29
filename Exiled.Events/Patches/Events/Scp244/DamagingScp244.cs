// -----------------------------------------------------------------------
// <copyright file="DamagingScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244DeployablePickup.Damage"/> to add missing logic to the <see cref="Scp244DeployablePickup"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.Damage))]
    internal static class DamagingScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();

            Label continueProcessing = generator.DefineLabel();

            LocalBuilder eventHandler = generator.DeclareLocal(typeof(DamagingScp244EventArgs));

            // Tested by Yamato and Undid-Iridium
#pragma warning disable SA1118 // Parameter should not span multiple lines

            // Remove grenade damage check, let event handler do it.
            newInstructions.RemoveRange(0, 5);

            int insertOffset = 5;

            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.ModelDestroyed)))) + insertOffset;

            newInstructions.RemoveRange(index, 3);

            // Insert event handler at start of function to determine whether to allow function to run or not.
            newInstructions.InsertRange(index, new[]
            {
                // Load Field (Because of get; set; it's property getter) of instance EStack[Scp244Deployable.State]
                new (OpCodes.Callvirt, PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State))),

                // Load value 2 (Enum of Scp244State.Destroyed) EStack[Scp244Deployable.State, 2]
                new (OpCodes.Ldc_I4_2),

                // Jump to return false label EStack[]
                new (OpCodes.Beq, returnFalse),

                // Continue processing, and load arg 0 (instance) again EStack[Scp244DeployablePickup Instance]
                new (OpCodes.Ldarg_0),

                // Load arg 1 (param 0) EStack[Scp244DeployablePickup Instance, Float damage]
                new (OpCodes.Ldarg_1),

                // Load arg 2 (param 1) EStack[Scp244DeployablePickup Instance, Float damage, DamageHandleBase handler]
                new (OpCodes.Ldarg_2),

                // Pass all 3 variables to DamageScp244 New Object, get a new object in return EStack[DamagingScp244EventArgs Instance] (Handler determins allowed??)
                new (OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingScp244EventArgs))[0]),

                // Copy it for later use again EStack[]
                new (OpCodes.Stloc, eventHandler.LocalIndex),

                // Load event back unto EStack[DamagingScp244EventArgs Instance]
                new (OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call Method on Instance EStack[]
                new (OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnDamagingScp244))),

                // Load event back unto EStack[DamagingScp244EventArgs Instance]
                new (OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new (OpCodes.Callvirt, PropertyGetter(typeof(DamagingScp244EventArgs), nameof(DamagingScp244EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, false return occurs below
                new (OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(returnFalse),
                new (OpCodes.Ret),

                // Continue processing, and load arg 0 (instance) again EStack[Scp244DeployablePickup Instance]
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(continueProcessing),

                // Load Scp244DeployablePickup instance EStack[Scp244DeployablePickup Instance, Scp244DeployablePickup Instance]
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load Scp244 health EStack[Scp244DeployablePickup Instance, health]
                new (OpCodes.Ldfld, Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._health))),

                // Load event back EStack[Scp244DeployablePickup Instance, health, DamagingScp244EventArgs Instance]
                new (OpCodes.Ldloc, eventHandler.LocalIndex),

                // Load damage handler from DamagingScp244EventArgs EStack[Scp244DeployablePickup Instance, health, Handler]
                new (OpCodes.Callvirt, PropertyGetter(typeof(DamagingScp244EventArgs), nameof(DamagingScp244EventArgs.Handler))),

                // Load damage handler from DamagingScp244EventArg.Handler EStack[Scp244DeployablePickup Instance, health, Damage]
                new (OpCodes.Callvirt, PropertyGetter(typeof(DamageHandler), nameof(DamageHandler.Damage))),

                // Game then does a sub, and stloc.
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
