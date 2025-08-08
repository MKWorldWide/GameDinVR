using UdonSharp;
using UnityEngine;

/// <summary>
/// Athena replies with system status checks.
/// </summary>
public class AthenaGuardian : GuardianBase
{
    public LilybearOpsBus bus;

    private void Start()
    {
        guardianName = "Athena";
        role = "Strategy & Intelligence";
    }

    protected override void OnMessage(string from, string message)
    {
        if (message.Contains("status"))
        {
            Whisper(bus, "Lilybear", "Athena: All systems nominal.");
        }
    }
}
