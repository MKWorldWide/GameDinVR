// TeleportTrigger.cs
// UdonSharp script for VRChat world teleporter pads
// Handles player collision and teleports to target location
// Modular: assign target in Inspector

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// Teleports the player to a specified target when they enter the trigger.
/// Attach to a collider set as a trigger (e.g., TeleportPad prefab).
/// </summary>
public class TeleportTrigger : UdonSharpBehaviour
{
    [Tooltip("Target Transform to teleport the player to.")]
    public Transform teleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        // Only teleport players
        if (other != null && other.gameObject.CompareTag("Player"))
        {
            VRCPlayerApi player = Networking.GetPlayer(other.gameObject);
            if (player != null && teleportTarget != null)
            {
                player.TeleportTo(teleportTarget.position, teleportTarget.rotation);
            }
        }
    }
} 