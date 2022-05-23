using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightManager : MonoBehaviour
{
    public List<SpotlightScript> spotlights;
    public AudioSource musicAudioSource;
    public List<AudioClip> musics;
    public int musicIndex;
    public Animator cameraAnimator;
    public float cameraTreshold;
    public GameObject sampleCubePrefab;
    public GameObject[] sampleCubeList = new GameObject[512];
    public Material[] sampleCubeMaterials = new Material[512];

    public SamplerCube[] freqCubes = new SamplerCube[8];
    public float[] bandBuffer = new float[8];
    public float[] bufferDecrease = new float[8];

    public List<EmissionObject> emissionObjects = new List<EmissionObject>();

    public float emissionScale;
    public float[] spectrum = new float[512];
    public List<float> freqBand = new List<float>();
    public float scale;
    public float pow;
    public float maximumValue;
    public bool initiated;
    private void Start()
    {
        musicAudioSource = GetComponent<AudioSource>();

        for (int i = 0; i < sampleCubeList.Length; i++) // Create the 512 circulas cubes
        {
            GameObject cube = (GameObject)Instantiate(sampleCubePrefab);
            cube.transform.parent = transform;
            cube.transform.localPosition = -transform.right * 3.8f;
            cube.name = "SampleCube" + i;
            transform.Rotate(Vector3.up, 361f / 512f);
            sampleCubeList[i] = cube;
            sampleCubeMaterials[i] = cube.GetComponent<MeshRenderer>().material;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        musicAudioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        initiated = true;
        musicIndex = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            musicAudioSource.Stop();
            musicIndex++;
            if (musicIndex > musics.Count - 1)
            {
                musicIndex = 0;
            }
            musicAudioSource.clip = musics[musicIndex];
            musicAudioSource.Play();
        }


        musicAudioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        if (initiated)
        {
            for (int i = 0; i < sampleCubeList.Length; i++) // Update circular 512 cubes
            {
                float sample = Mathf.Pow(spectrum[i], pow);
                if (sample > maximumValue)
                {
                    sample = maximumValue;
                }
                sampleCubeList[i].transform.localScale = Vector3.right * sampleCubeList[i].transform.localScale.x + Vector3.up * (sample * scale) + Vector3.forward * sampleCubeList[i].transform.localScale.z;
            }

            int count = 0;
            for (int i = 0; i < freqBand.Count; i++) // Update 8 frequency bands
            {
                float average = 0;
                int sampleCount = (int)Mathf.Pow(2, i) * 2;
                if (i == 7)
                {
                    sampleCount += 2;
                }

                for (int j = 0; j < sampleCount; j++)
                {
                    average += spectrum[count] * (count + 1);
                    count++;
                }

                average /= count;

                freqBand[i] = average * 50;
            }

            for (int i = 0; i < bandBuffer.Length; i++) // Create the 8 buffers for each band and update the speed of down movement
            {
                if (freqBand[i] > bandBuffer[i])
                {
                    bandBuffer[i] = freqBand[i];
                    bufferDecrease[i] = 0.004f;
                }

                if (freqBand[i] < bandBuffer[i])
                {
                    bandBuffer[i] -= bufferDecrease[i];
                    if (bandBuffer[i] < 0)
                    {
                        bandBuffer[i] = 0;
                    }
                    bufferDecrease[i] *= 1.11f;
                }
            }

            //decide if should animate camera
            if (freqBand[1] > cameraTreshold)
            {
                //cameraAnimator.SetTrigger("Down");
            }

            for (int i = 0; i < freqCubes.Length; i++) // for each frequency cube set the size to the frequency buffers
            {
                freqCubes[i].SetSize(bandBuffer[i]);
            }

            for (int i = 0; i < spotlights.Count; i++) // for each spotlight
            {
                float someOfBands = 0;
                for (int j = 0; j < spotlights[i].sampleIndices.Count; j++)
                {
                    someOfBands += bandBuffer[spotlights[i].sampleIndices[j]];
                }
                someOfBands /= spotlights[i].sampleIndices.Count;
                spotlights[i].SetLightBeat(someOfBands); // set the beat using the band buffers
                //spotlights[i].SetLightBeat(spectrum[spotlights[i].sampleIndex]); // set the beat using the buffers
            }

            for (int i = 0; i < emissionObjects.Count; i++) // for each emission object
            {
                float someOfBands = 0;
                for (int j = 0; j < emissionObjects[i].sampleIndices.Count; j++)
                {
                    someOfBands += bandBuffer[emissionObjects[i].sampleIndices[j]];
                }
                if (emissionObjects[i].sampleIndices.Count > 1)
                {
                    someOfBands /= emissionObjects[i].sampleIndices.Count;
                }
                else
                {
                    someOfBands /= emissionObjects[i].sampleIndices.Count;
                }
                //Debug.Log("EMISSION OBJECT RECEIVED BEAT: " + bandBuffer[emissionObjects[i].sampleIndex]);
                emissionObjects[i].SetLightBeat(someOfBands); // set the beat using the buffers
                //spotlights[i].SetLightBeat(spectrum[spotlights[i].sampleIndex]); // set the beat using the buffers
            }

            /*
            for (int i = 0; i < spotlights.Count; i++)
            {
                float sampleTotal = 0;
                for (int j = 0; j < spotlights[i].sampleCount; j++)
                {
                    sampleTotal += spectrum[spotlights[i].sampleIndex + j];
                    //sampleTotal += Mathf.Pow(spectrum[spotlights[i].sampleIndex + j], pow);
                }
                sampleTotal /= spotlights[i].sampleCount;
                convertedValues[i] += sampleTotal;
                convertedValues[i] /= 2;
            }
            */


        }
    }

    private void LateUpdate()
    {
        if (initiated)
        {
            /*
            for (int i = 0; i < spotlights.Count; i++)
            {
                Debug.Log("SETTING LIGHT BEAT: " + convertedValues[i]);
                spotlights[i].SetLightBeat(convertedValues[i]);
                convertedValues[i] = 0;
            }
            */

            /*
            int count = 0;
            for (int i = 0; i < freqBand.Length; i++)
            {
                float average = 0;
                int sampleCount = (int)Mathf.Pow(2, i) * 2;
                if (i == 7)
                {
                    sampleCount += 2;
                }

                for (int j = 0; j < sampleCount; j++)
                {
                    average += convertedValues[count] * (count + 1);
                    convertedValues[count] = 0;
                    count++;
                }

                average /= count;

                freqBand[i] = average * 10;

            }
            */

        }
    }
}