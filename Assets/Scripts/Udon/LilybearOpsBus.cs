using UdonSharp;
using UnityEngine;

/// <summary>
/// Central in-scene bus that relays messages between guardians.
/// External bridges (e.g. Discord) call <see cref="Say"/> to inject events.
/// </summary>
public class LilybearOpsBus : UdonSharpBehaviour
{
    [Tooltip("Guardians listening on the bus")] public GuardianBase[] guardians;

    /// <summary>
    /// Broadcast a message to all guardians. The guardian decides if it should react.
    /// </summary>
    public void Say(string from, string to, string message)
    {
        foreach (var g in guardians)
        {
            if (g != null)
            {
                g.Receive(from, to, message);
            }
        }
    }
}
