// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp939LungeAbility.ServerProcessCmd(NetworkReader)" />
    /// to add the <see cref="Scp939.Lunging" /> event.
    /// </summary>
    [EventPatch(typeof(Scp939), nameof(Scp939.Lunging))]
    [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.TriggerLunge))]
    internal static class Lunge
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player::Get(Owner)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // LungingEventArgs ev = new (...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LungingEventArgs))[0]),

                // Scp939.OnLunging(ev)
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnLunging))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}