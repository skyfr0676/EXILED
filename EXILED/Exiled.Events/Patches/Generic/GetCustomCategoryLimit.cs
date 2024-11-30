// -----------------------------------------------------------------------
// <copyright file="GetCustomCategoryLimit.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;

    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Configs;
    using UnityEngine;

    /// <summary>
    /// Patches the <see cref="InventoryLimits.GetCategoryLimit(ItemCategory, ReferenceHub)"/> delegate.
    /// Sync <see cref="Player.SetCategoryLimit(ItemCategory, sbyte)"/>, .
    /// Changes <see cref="ushort.MaxValue"/> to <see cref="ushort.MinValue"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetCategoryLimit), new Type[] { typeof(ItemCategory), typeof(ReferenceHub), })]
    internal static class GetCustomCategoryLimit
    {
#pragma warning disable SA1313
        private static void Postfix(ItemCategory category, ReferenceHub player, ref sbyte __result)
        {
            if (!Player.TryGet(player, out Player ply) || !ply.CustomCategoryLimits.TryGetValue(category, out sbyte limit))
                return;

            __result = (sbyte)Mathf.Clamp(limit + __result - InventoryLimits.GetCategoryLimit(null, category), sbyte.MinValue, sbyte.MaxValue);
        }
    }
}
