using UdonSharp;
using UnityEngine;

/// <summary>
/// Serafina routes blessing requests to ShadowFlowers.
/// </summary>
public class SerafinaGuardian : GuardianBase
{
    public LilybearOpsBus bus;

    private void Start()
    {
        guardianName = "Serafina";
        role = "Comms & Routing";
    }

    protected override void OnMessage(string from, string message)
    {
        if (message.StartsWith("bless"))
        {
            Whisper(bus, "ShadowFlowers", "Please deliver a blessing to the hall.");
        }
    }
}
