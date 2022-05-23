using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionObject : MonoBehaviour
{
    public Material material;
    public float maxIntensity;
    public float minIntensity;
    public List<Vector2> ranges;
    public List<Color> colors;
    public Dictionary<Vector2, Color> lightsteps;
    public List<int> sampleIndices;
    //public int sampleCount;
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        lightsteps = new Dictionary<Vector2, Color>();
        for (int i = 0; i < ranges.Count; i++)
        {
            lightsteps.Add(ranges[i], colors[i]);
        }
    }

    public void SetLightBeat(float beatScalling)
    {
        
        foreach (var lightstep in lightsteps)
        {
            if (beatScalling >= lightstep.Key.x && beatScalling < lightstep.Key.y)
            {
                StartCoroutine(SetColor(0.5f, lightstep.Value));
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
        float ix = material.GetColor("_EmissionColor").r;
        float iy = material.GetColor("_EmissionColor").g;
        float iz = material.GetColor("_EmissionColor").b;
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
                material.SetColor("_EmissionColor", new Color(lx, ly, lz, 1));// = new Color(lx, ly, lz, 1);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        material.color = new Color(tx, ty, tz, 1);
    }
}