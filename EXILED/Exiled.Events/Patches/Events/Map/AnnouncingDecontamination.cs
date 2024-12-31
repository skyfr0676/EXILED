// -----------------------------------------------------------------------
// <copyright file="AnnouncingDecontamination.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using Handlers;

    using HarmonyLib;

    using LightContainmentZoneDecontamination;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DecontaminationController.UpdateSpeaker" />.
    /// Adds the <see cref="AnnouncingDecontamination" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.AnnouncingDecontamination))]
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.UpdateSpeaker))]
    internal static class AnnouncingDecontamination
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // this._nextPhase
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(DecontaminationController), nameof(DecontaminationController._nextPhase))),

                    // this._curFunction
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(DecontaminationController), nameof(DecontaminationController._curFunction))),

                    // AnnouncingDecontaminationEventArgs ev = new(int, PhaseFunction)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingDecontaminationEventArgs))[0]),

                    // Map.OnAnnouncingDecontamination(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingDecontamination))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}