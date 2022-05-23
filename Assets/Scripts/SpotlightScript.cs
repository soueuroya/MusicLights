using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightScript : MonoBehaviour
{
    public Light spotlight;
    public float maxIntensity;
    public float minIntensity;
    public List<Vector2> ranges;
    public List<Color> colors;
    public Dictionary<Vector2, Color> lightsteps;
    public List<int> sampleIndices;
    public GameObject lightSource;
    public Material lightSourceMaterial;
    public Animator animator;
    //public int sampleCount;
    void Start()
    {
        lightsteps = new Dictionary<Vector2, Color>();
        for (int i = 0; i < ranges.Count; i++)
        {
            lightsteps.Add(ranges[i], colors[i]);
        }
        lightSourceMaterial = lightSource.GetComponent<MeshRenderer>().material;
        animator = gameObject.GetComponent<Animator>();
    }

    public void SetLightBeat(float beatScalling)
    {
        //Debug.Log("SPOTLIGHT RECEIVED BEAT: " + beatScalling);
        foreach (var lightstep in lightsteps)
        {
            if (beatScalling >= lightstep.Key.x && beatScalling < lightstep.Key.y)
            {
                StartCoroutine(SetColor(0.5f, lightstep.Value));
                StartCoroutine(SetIntensity(0.01f, beatScalling));
                return;
            }
        }
    }

    private IEnumerator SetColor(float totalTime, Color targetColor)
    {
        float timeElapsed = 0;
        float tx = targetColor.r;
        float ty = targetColor.g;
        float tz = targetColor.b;
        float ix = spotlight.color.r;
        float iy = spotlight.color.g;
        float iz = spotlight.color.b;
        if (ix != tx || iy != ty || iz != tz)
        {
            float lx = 0;
            float ly = 0;
            float lz = 0;
            float percentage = 0;
            while (timeElapsed < totalTime)
            {
                percentage = timeElapsed / totalTime;
                lx = Mathf.Lerp(ix, tx, percentage);
                ly = Mathf.Lerp(iy, ty, percentage);
                lz = Mathf.Lerp(iz, tz, percentage);
                spotlight.color = new Color(lx, ly, lz, 1);
                lightSourceMaterial.SetColor("_EmissionColor", spotlight.color);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        spotlight.color = new Color(tx, ty, tz, 1);
    }


    private IEnumerator SetIntensity(float totalTime, float targetIntensity)
    {
        float elapsedTime = 0;
        float inten = 0;
        float ii = spotlight.intensity;
        if (ii < targetIntensity)
        {
            while (elapsedTime < totalTime)
            {
                inten = Mathf.Lerp(ii, targetIntensity, elapsedTime/totalTime);
                spotlight.intensity = inten;
                elapsedTime += Time.deltaTime;
                if (animator != null)
                {
                    Debug.Log("SETTING ANIMATION SPEED: " + Mathf.Pow(inten, 4));
                    animator.speed = inten;
                }
                yield return null;
            }
        }
        else
        {
            while (elapsedTime < totalTime)
            {
                inten = Mathf.Lerp(ii, targetIntensity, elapsedTime / totalTime);
                spotlight.intensity = inten;
                elapsedTime += Time.deltaTime;
                if (animator != null)
                {
                    Debug.Log("SETTING ANIMATION SPEED: " + Mathf.Pow(inten, 4));
                    animator.speed = inten;
                }
                yield return null;
            }
        }
        spotlight.intensity = targetIntensity;
    }
}
