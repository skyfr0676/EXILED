// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    using Enums;
    using Interactables.Interobjects.DoorUtils;
    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A set of tools to easily work with the alpha warhead.
    /// </summary>
    public static class Warhead
    {
        private static AlphaWarheadOutsitePanel alphaWarheadOutsitePanel;

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadController"/> component.
        /// </summary>
        public static AlphaWarheadController Controller => AlphaWarheadController.Singleton;

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadNukesitePanel"/> component.
        /// </summary>
        public static AlphaWarheadNukesitePanel SitePanel => AlphaWarheadOutsitePanel.nukeside;

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadOutsitePanel"/> component.
        /// </summary>
        public static AlphaWarheadOutsitePanel OutsitePanel => alphaWarheadOutsitePanel != null ? alphaWarheadOutsitePanel : (alphaWarheadOutsitePanel = UnityEngine.Object.FindFirstObjectByType<AlphaWarheadOutsitePanel>());

        /// <summary>
        /// Gets the <see cref="GameObject"/> of the warhead lever.
        /// </summary>
        public static GameObject Lever => SitePanel.lever.gameObject;

        /// <summary>
        /// Gets or sets a value indicating whether DeadmanSwitch detonation is enabled.
        /// </summary>
        public static bool DeadmanSwitchEnabled
        {
            get => DeadmanSwitch.IsDeadmanSwitchEnabled;
            set => DeadmanSwitch.IsDeadmanSwitchEnabled = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether automatic detonation is enabled.
        /// </summary>
        public static bool AutoDetonate
        {
            get => Controller._autoDetonate;
            set => Controller._autoDetonate = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether doors will be opened when the warhead activates.
        /// </summary>
        public static bool OpenDoors
        {
            get => Controller._openDoors;
            set => Controller._openDoors = value;
        }

        /// <summary>
        /// Gets all of the warhead blast doors.
        /// </summary>
        public static IReadOnlyCollection<BlastDoor> BlastDoors => BlastDoor.Instances;

        /// <summary>
        /// Gets or sets a value indicating whether the warhead lever is enabled.
        /// </summary>
        public static bool LeverStatus
        {
            get => SitePanel.Networkenabled;
            set => SitePanel.Networkenabled = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the warhead's outside panel has been opened.
        /// </summary>
        public static bool IsKeycardActivated
        {
            get => AlphaWarheadActivationPanel.IsUnlocked;
            set => AlphaWarheadActivationPanel.IsUnlocked = value;
        }

        /// <summary>
        /// Gets or sets the warhead status.
        /// </summary>
        public static WarheadStatus Status
        {
            get => IsInProgress ? IsDetonated ? WarheadStatus.Detonated : WarheadStatus.InProgress : LeverStatus ? WarheadStatus.Armed : WarheadStatus.NotArmed;
            set
            {
                switch (value)
                {
                    case WarheadStatus.NotArmed:
                    case WarheadStatus.Armed:
                        Stop();
                        LeverStatus = value is WarheadStatus.Armed;
                        break;

                    case WarheadStatus.InProgress:
                        Start();
                        break;

                    case WarheadStatus.Detonated:
                        Detonate();
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the warhead has already been detonated.
        /// </summary>
        public static bool IsDetonated => Controller.AlreadyDetonated;

        /// <summary>
        /// Gets a value indicating whether the warhead detonation is in progress.
        /// </summary>
        public static bool IsInProgress => Controller.Info.InProgress;

        /// <summary>
        /// Gets or sets the warhead detonation timer.
        /// </summary>
        public static float DetonationTimer
        {
            get => AlphaWarheadController.TimeUntilDetonation;
            set => Controller.ForceTime(value);
        }

        /// <summary>
        /// Gets the warhead real detonation timer.
        /// </summary>
        public static float RealDetonationTimer => Controller.CurScenario.TimeToDetonate;

        /// <summary>
        /// Gets or sets a value indicating whether the warhead can be disabled.
        /// </summary>
        public static bool IsLocked
        {
            get => Controller.IsLocked;
            set => Controller.IsLocked = value;
        }

        /// <summary>
        /// Gets or sets the amount of kills caused by the warhead (shown on the summary screen).
        /// </summary>
        public static int Kills
        {
            get => Controller.WarheadKills;
            set => Controller.WarheadKills = value;
        }

        /// <summary>
        /// Gets a value indicating whether the warhead can be started.
        /// </summary>
        public static bool CanBeStarted => !IsInProgress && !IsDetonated && Controller.CooldownEndTime <= NetworkTime.time;

        /// <summary>
        /// Closes the surface blast doors.
        /// </summary>
        public static void CloseBlastDoors()
        {
            foreach (BlastDoor door in BlastDoors)
                door.SetDoorState(true, false);
        }

        /// <summary>
        /// Open the surface blast doors.
        /// </summary>
        public static void OpenBlastDoors()
        {
            foreach (BlastDoor door in BlastDoors)
                door.SetDoorState(false, true);
        }

        /// <summary>
        /// Opens or closes all doors on the map, based on the provided <paramref name="open"/>.
        /// </summary>
        /// <param name="open">Whether to open or close all doors on the map.</param>
        public static void TriggerDoors(bool open) => DoorEventOpenerExtension.TriggerAction(open ? DoorEventOpenerExtension.OpenerEventType.WarheadStart : DoorEventOpenerExtension.OpenerEventType.WarheadCancel);

        /// <summary>
        /// Starts the warhead countdown.
        /// </summary>
        public static void Start()
        {
            Controller.InstantPrepare();
            Controller.StartDetonation(false);
        }

        /// <summary>
        /// Starts the warhead countdown.
        /// </summary>
        /// <param name="isAutomatic">Indicates whether the warhead is started automatically.</param>
        /// <param name="suppressSubtitles">If <see langword="true"/>, subtitles will not be displayed during the countdown.</param>
        /// <param name="trigger">The <see cref="Player"/> of the entity that triggered the warhead.</param>
        public static void Start(bool isAutomatic, bool suppressSubtitles = false, Player trigger = null)
        {
            Controller.InstantPrepare();
            Controller.StartDetonation(isAutomatic, suppressSubtitles, trigger == null ? null : trigger.ReferenceHub);
        }

        /// <summary>
        /// Stops the warhead.
        /// </summary>
        public static void Stop() => Controller.CancelDetonation();

        /// <summary>
        /// Stops the warhead detonation process.
        /// </summary>
        /// <param name="disabler">
        /// The <see cref="Player"/> who is disabling the warhead.
        /// If <see langword="null"/>, the warhead will be stopped without a specific player reference.
        /// </param>
        public static void Stop(Player disabler) => Controller.CancelDetonation(disabler.ReferenceHub);

        /// <summary>
        /// Detonates the warhead.
        /// </summary>
        public static void Detonate() => Controller.ForceTime(0f);

        /// <summary>
        /// Detonates the warhead after the specified remaining time.
        /// </summary>
        /// <param name="remaining">
        /// The time in seconds until the warhead detonates.
        /// If set to <see langword="0"/>, the warhead will detonate immediately.
        /// </param>
        public static void Detonate(float remaining) => Controller.ForceTime(remaining);

        /// <summary>
        /// Shake all players, like if the warhead has been detonated.
        /// </summary>
        public static void Shake() => Controller.RpcShake(false);

        /// <summary>
        /// Shake all players, like if the warhead has been detonated.
        /// </summary>
        /// <param name="archieve">
        /// If <see langword="true"/>, the shake effect will be archived.
        /// </param>
        public static void Shake(bool archieve) => Controller.RpcShake(archieve);

        /// <summary>
        /// Gets whether the provided position will be detonated by the alpha warhead.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <param name="includeOnlyLifts">If <see langword="true"/>, only lifts will be checked.</param>
        /// <returns>Whether the given position is prone to being detonated.</returns>
        public static bool CanBeDetonated(Vector3 pos, bool includeOnlyLifts = false) => AlphaWarheadController.CanBeDetonated(pos, includeOnlyLifts);
    }
}
