using UdonSharp;
using UnityEngine;

/// <summary>
/// ShadowFlowers delivers blessings and ceremonial messages.
/// </summary>
public class ShadowFlowersGuardian : GuardianBase
{
    public TextMesh blessingText;
    public LilybearOpsBus bus;

    private void Start()
    {
        guardianName = "ShadowFlowers";
        role = "Sentiment & Rituals";
    }

    protected override void OnMessage(string from, string message)
    {
        if (message.Contains("blessing"))
        {
            var line = "🌸 May your path be protected and your heart be held.";
            if (blessingText != null) blessingText.text = line;
            Whisper(bus, "Lilybear", "Blessing delivered.");
        }
    }
}
