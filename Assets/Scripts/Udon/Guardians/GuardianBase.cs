using UdonSharp;
using UnityEngine;

/// <summary>
/// Base class for all guardians. Provides simple bus integration and message filtering.
/// </summary>
public class GuardianBase : UdonSharpBehaviour
{
    [Header("Identity")]
    public string guardianName = "Guardian";
    public string role = "Undefined";

    /// <summary>
    /// Called by <see cref="LilybearOpsBus"/> when a message arrives.
    /// </summary>
    public virtual void Receive(string from, string to, string message)
    {
        if (to == guardianName || to == "*")
        {
            OnMessage(from, message);
        }
    }

    /// <summary>
    /// Override in child classes to handle specific messages.
    /// </summary>
    protected virtual void OnMessage(string from, string message) { }

    /// <summary>
    /// Utility for guardians to send a message through the bus.
    /// </summary>
    protected void Whisper(LilybearOpsBus bus, string to, string message)
    {
        bus.Say(guardianName, to, message);
    }
}
