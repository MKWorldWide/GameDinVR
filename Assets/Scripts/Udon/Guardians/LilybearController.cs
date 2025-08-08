using UdonSharp;
using UnityEngine;

/// <summary>
/// Lilybear routes commands and can broadcast to all guardians.
/// </summary>
public class LilybearController : GuardianBase
{
    [TextArea] public string lastMessage;
    public LilybearOpsBus bus;

    private void Start()
    {
        guardianName = "Lilybear";
        role = "Voice & Operations";
    }

    protected override void OnMessage(string from, string message)
    {
        lastMessage = from + ": " + message;
        if (message.StartsWith("/route "))
        {
            var payload = message.Substring(7);
            Whisper(bus, "*", payload);
        }
    }
}
