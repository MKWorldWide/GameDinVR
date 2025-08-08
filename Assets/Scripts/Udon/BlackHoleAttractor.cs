// BlackHoleAttractor.cs
// Quantum-documented UdonSharp script for simulating a distant black hole's pull
//
// Feature Context:
//   - Applies a subtle gravitational force toward a designated event horizon
//   - Enhances space-world immersion with minimal performance cost
//
// Dependencies:
//   - UnityEngine
//   - UdonSharp
//   - VRC.SDKBase for player movement APIs
//
// Usage Example:
//   Attach to an empty GameObject at scene root.
//   Set 'eventHorizon' to the Transform representing the black hole center.
//
// Performance:
//   - Uses LateUpdate with Time.deltaTime for smooth, frame-independent motion
//   - Gravity strength kept low to avoid nausea or rapid drift
//
// Security:
//   - No networking or external calls; operates purely client-side
//
// Changelog:
//   - v1.0.0: Initial quantum singularity version

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BlackHoleAttractor : UdonSharpBehaviour
{
    [Header("Event Horizon")]
    [Tooltip("Transform representing the black hole's center.")]
    public Transform eventHorizon;

    [Header("Gravity Settings")]
    [Tooltip("Force applied each frame toward the event horizon.")]
    public float gravityStrength = 2f;

    void LateUpdate()
    {
        // Ensure the local player and event horizon exist
        if (Networking.LocalPlayer == null || eventHorizon == null) return;

        // Direction from player to black hole center
        Vector3 dir = (eventHorizon.position - Networking.LocalPlayer.GetPosition()).normalized;

        // Apply a gentle pull to the player's velocity
        Vector3 newVelocity = Networking.LocalPlayer.GetVelocity() + dir * gravityStrength * Time.deltaTime;
        Networking.LocalPlayer.SetVelocity(newVelocity);
    }
}

