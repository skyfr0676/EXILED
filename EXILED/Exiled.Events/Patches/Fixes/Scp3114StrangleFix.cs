// -----------------------------------------------------------------------
// <copyright file="Scp3114StrangleFix.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using MEC;

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;

    /// <summary>
    /// Fix Scp3114 Strangle bug, where a target with 200+ slowness can eventually crash.
    /// </summary>
    internal class Scp3114StrangleFix
    {
        public static IEnumerator<float> Test()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                foreach (Player t in Player.Get(x => !x.IsNPC))
                {
                    Log.Info(t.Position);
                }
            }
        }
    }
}