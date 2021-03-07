using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Transform[] Lights;
    public float[] LightSizeMultiplier;
    public float[] LightSizeSpeed;
    public float[] LightTimeOffsets;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Lights.Length; i++)
        {
            StartCoroutine(LightPulse(Lights[i], LightSizeMultiplier[i], LightSizeSpeed[i], LightTimeOffsets[i]));
        }
    }

    // Pulse the light
    IEnumerator LightPulse(Transform light, float size, float speed, float timeOffset)
    {
        yield return new WaitForSeconds(timeOffset);
        while (light.localScale.x < size)
        {
            yield return new WaitForSeconds(0.1f);
            light.localScale = new Vector2(light.localScale.x + speed, light.localScale.y + speed);
        }
        while (light.localScale.x > 1)
        {
            yield return new WaitForSeconds(0.1f);
            light.localScale = new Vector2(light.localScale.x - speed, light.localScale.y - speed);
        }
        StartCoroutine(LightPulse(light, size, speed, timeOffset));
    }
}
