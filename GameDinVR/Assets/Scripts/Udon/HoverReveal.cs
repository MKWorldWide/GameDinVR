// HoverReveal.cs
// UdonSharp script for interactive hover effects
// Reveals info or highlights objects when looked at or hovered over

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// Reveals a UI element or highlights an object when hovered/pointed at.
/// Attach to lore chambers, portals, or interactive objects.
/// </summary>
public class HoverReveal : UdonSharpBehaviour
{
    [Tooltip("GameObject to reveal or highlight on hover.")]
    public GameObject revealObject;

    private void Start()
    {
        if (revealObject != null)
            revealObject.SetActive(false);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (revealObject != null)
            revealObject.SetActive(true);
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (revealObject != null)
            revealObject.SetActive(false);
    }
} 