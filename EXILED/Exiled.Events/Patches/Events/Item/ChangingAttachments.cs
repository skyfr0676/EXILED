// -----------------------------------------------------------------------
// <copyright file="ChangingAttachments.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Items;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches
    /// <see cref="AttachmentsServerHandler.ServerReceiveChangeRequest(NetworkConnection, AttachmentsChangeRequest)" />.
    /// Adds the <see cref="Handlers.Item.ChangingAttachments" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.ChangingAttachments))]
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceiveChangeRequest))]
    internal static class ChangingAttachments
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAttachmentsEventArgs));
            LocalBuilder curCode = generator.DeclareLocal(typeof(uint));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // curCode = Firearm::GetCurrentAttachmentsCode
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                    new(OpCodes.Stloc_S, curCode.LocalIndex),

                    // If the Firearm::GetCurrentAttachmentsCode isn't changed, prevents the method from being executed
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                    new(OpCodes.Ldloc_S, curCode.LocalIndex),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, ret),

                    // API::Features::Player::Get(NetworkConnection::identity::netId)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(uint) })),

                    // firearm
                    new(OpCodes.Ldloc_1),

                    // AttachmentsChangeRequest::AttachmentsCode
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChangingAttachmentsEventArgs ev = new ChangingAttachmentsEventArgs(Player, Firearm, uint, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAttachmentsEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers::Item::OnChangingAttachments(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingAttachments))),

                    // ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // **AttachmentsChangeRequest = ev::NewCode + curCode - ev::CurrentCode
                    new(OpCodes.Ldarga_S, 1),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.NewCode))),
                    new(OpCodes.Ldloc_S, curCode.LocalIndex),
                    new(OpCodes.Add),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.CurrentCode))),
                    new(OpCodes.Sub),
                    new(OpCodes.Stfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}