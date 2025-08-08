using UdonSharp;
using UnityEngine;

/// <summary>
/// Entry point for external bridges (e.g. OSC/HTTP from Serafina) to feed messages into the bus.
/// </summary>
public class DiscordMessageRelay : UdonSharpBehaviour
{
    public LilybearOpsBus bus;

    /// <summary>
    /// Called by external systems to relay Discord messages.
    /// </summary>
    public void Relay(string author, string content)
    {
        bus.Say(author, "*", content);
    }
}
