// -----------------------------------------------------------------------
// <copyright file="ValidatingVisibility.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    #pragma warning disable SA1313

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp939;

    /// <summary>
    ///     Patches <see cref="Scp939VisibilityController.ValidateVisibility(ReferenceHub)" />
    ///     to add the <see cref="Scp939.ValidatingVisibility" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp939), nameof(Handlers.Scp939.ValidatingVisibility))]
    [HarmonyPatch(typeof(Scp939VisibilityController), nameof(Scp939VisibilityController.ValidateVisibility))]
    internal class ValidatingVisibility
    {
        private static void Postfix(Scp939VisibilityController __instance, ReferenceHub hub, ref bool __result)
        {
            ValidatingVisibilityEventArgs ev = new(__instance.Owner, hub, __result);
            Handlers.Scp939.OnValidatingVisibility(ev);
            __result = ev.IsAllowed;
        }
    }
}