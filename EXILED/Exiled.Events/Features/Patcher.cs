// -----------------------------------------------------------------------
// <copyright file="Patcher.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Interfaces;
    using HarmonyLib;

    /// <summary>
    /// A tool for patching.
    /// </summary>
    public class Patcher
    {
        /// <summary>
        /// The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
        /// </summary>
        private static int patchesCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Patcher"/> class.
        /// </summary>
        internal Patcher()
        {
            Harmony = new($"exiled.events.{++patchesCounter}");
        }

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> that contains all patch types that haven't been patched.
        /// </summary>
        public static HashSet<Type> UnpatchedTypes { get; private set; } = GetAllPatchTypes();

        /// <summary>
        /// Gets a set of types and methods for which EXILED patches should not be run.
        /// </summary>
        public static HashSet<MethodBase> DisabledPatchesHashSet { get; } = new();

        /// <summary>
        /// Gets the <see cref="HarmonyLib.Harmony"/> instance.
        /// </summary>
        public Harmony Harmony { get; }

        /// <summary>
        /// Patches all events that target a specific <see cref="IExiledEvent"/>.
        /// </summary>
        /// <param name="event">The <see cref="IExiledEvent"/> all matching patches should target.</param>
        public void Patch(IExiledEvent @event)
        {
            try
            {
                List<Type> types = ListPool<Type>.Pool.Get(UnpatchedTypes.Where(x => x.GetCustomAttributes<EventPatchAttribute>().Any((epa) => epa.Event == @event)));

                foreach (Type type in types)
                {
                    List<MethodInfo> methodInfos = new PatchClassProcessor(Harmony, type).Patch();
                    if (DisabledPatchesHashSet.Any(x => methodInfos.Contains(x)))
                        ReloadDisabledPatches();
                    UnpatchedTypes.Remove(type);
                }

                ListPool<Type>.Pool.Return(types);
            }
            catch (Exception ex)
            {
                Log.Error($"Patching by event failed!\n{ex}");
            }
        }

        /// <summary>
        /// Patches all events.
        /// </summary>
        /// <param name="includeEvents">Whether to patch events as well as all required patches.</param>
        /// <param name="failedPatch">the number of failed patch returned.</param>
        public void PatchAll(bool includeEvents, out int failedPatch)
        {
            failedPatch = 0;

            try
            {
#if DEBUG
                bool lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;
#endif
                List<Type> toPatch = ListPool<Type>.Pool.Get(includeEvents ? UnpatchedTypes : UnpatchedTypes.Where((type) => !type.GetCustomAttributes<EventPatchAttribute>().Any()));
                for (int i = 0; i < toPatch.Count; i++)
                {
                    Type patch = toPatch[i];
                    try
                    {
                        Harmony.CreateClassProcessor(patch).Patch();
                        UnpatchedTypes.Remove(patch);
                    }
                    catch (HarmonyException exception)
                    {
                        Log.Error($"Patching by attributes failed!\n{exception}");

                        failedPatch++;
                        continue;
                    }
                }

                ListPool<Type>.Pool.Return(toPatch);

                Log.Debug("Events patched by attributes successfully!");
#if DEBUG
                Harmony.DEBUG = lastDebugStatus;
#endif
            }
            catch (Exception exception)
            {
                Log.Error($"Patching by attributes failed!\n{exception}");
            }
        }

        /// <summary>
        /// Checks the <see cref="DisabledPatchesHashSet"/> list and un-patches any methods that have been defined there. Once un-patching has been done, they can be patched by plugins, but will not be re-patchable by Exiled until a server reboot.
        /// </summary>
        public void ReloadDisabledPatches()
        {
            foreach (MethodBase method in DisabledPatchesHashSet)
            {
                Harmony.Unpatch(method, HarmonyPatchType.All, Harmony.Id);

                Log.Info($"Unpatched {method.Name}");
            }
        }

        /// <summary>
        /// Unpatches all events.
        /// </summary>
        public void UnpatchAll()
        {
            Log.Debug("Unpatching events...");
            Harmony.UnpatchAll(Harmony.Id);
            UnpatchedTypes = GetAllPatchTypes();

            Log.Debug("All events have been unpatched. Goodbye!");
        }

        /// <summary>
        /// Gets all types that have a <see cref="HarmonyPatch"/> attributed to them.
        /// </summary>
        /// <returns>A <see cref="HashSet{T}"/> of all patch types.</returns>
        internal static HashSet<Type> GetAllPatchTypes() => Assembly.GetExecutingAssembly().GetTypes().Where((type) => type.CustomAttributes.Any((customAtt) => customAtt.AttributeType == typeof(HarmonyPatch))).ToHashSet();
    }
}
