using UnityEngine;
using System;

public class ClockHandController : MonoBehaviour
{
    public RectTransform hourHand;
    public RectTransform minuteHand;

    void Update()
    {
        DateTime now = DateTime.Now;

        float hour = now.Hour % 12 + now.Minute / 60f;  // 0–12 arasý
        float minute = now.Minute + now.Second / 60f;   // 0–60 arasý

        float hourAngle = -hour * 30f;      // 360/12 = 30 derece her saat
        float minuteAngle = -minute * 6f;   // 360/60 = 6 derece her dakika

        if (hourHand != null) hourHand.localRotation = Quaternion.Euler(0, 0, hourAngle);
        if (minuteHand != null) minuteHand.localRotation = Quaternion.Euler(0, 0, minuteAngle);
    }
}
