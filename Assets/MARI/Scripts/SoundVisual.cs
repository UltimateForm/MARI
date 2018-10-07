using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundVisual : MonoBehaviour {


    public const int SAMPLE_SIZE = 1024;
    public float rmsValue;
    public float dbValue;
    public float pitchValue;
    public float spectrumPercentage = 0.5f;
    public AudioSource source;
    public float sampleRate;
    private float[] samples;
    private float[] spectrum;
    public UnityEvent onUpdate;
	void Start ()
    {
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;
        //SpawnCubes();
        visualScale = new float[amntVisual];

    }

    public float radius = 5f;
    private Vector3[] basePos;

    void Update () {
        ReadSamples();
        ReadRMS();
        ReadDecibels();
        ReadSpectrum();
        ReadPitch();
        //UpdateVisualSpheric();
        UpdateVisual();
        onUpdate?.Invoke();
	}
    public float fallSpeed = 100f;


    public float[] ReadSamples()
    {
        source.GetOutputData(samples, 0);
        return samples;
    }

    private float ReadRMS()
    {
        //GET RMS
        int i = 0;
        float sum = 0;
        for (i = 0; i < SAMPLE_SIZE; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);
        return rmsValue;
    }

    private float ReadDecibels()
    {
        //GET DECIBELS
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);
        return dbValue;
    }

    private float[] ReadSpectrum()
    {
        //GET AUDIO SPECTRUM
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        return spectrum;
    }
    public float handleLenght = 0.5f;
    private float ReadPitch()
    {
        //GET PITCH
        float maxV = 0;
        var maxN = 0;
        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f)) continue;
            maxV = spectrum[i];
            maxN = i;
        }
        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
        return pitchValue;
    }

    private Transform[] visualList;
    public float[] visualScale;
    private int amntVisual =6;
    public float maxScale = 4;
    private void SpawnCubes()
    {
        visualList = new Transform[amntVisual];
        visualScale = new float[amntVisual];
        for (int i = 0; i < amntVisual; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.GetComponent<BoxCollider>().enabled = false;
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right * i;
        }
        UpdateVisualSpheric();
    }
    public float height;
    public void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int avgSize = (int)((SAMPLE_SIZE * spectrumPercentage) / amntVisual/2);
        
        while (visualIndex < amntVisual/2 && spectrumIndex<spectrum.Length)
        {
            int j = 0;
            float sum = 0;
            while (j < avgSize && spectrumIndex < spectrum.Length)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }
            var scaleY = (sum / avgSize) * 100f;
            visualScale[visualIndex] -= Time.deltaTime * fallSpeed;
            if (visualScale[visualIndex]<scaleY)
            {
                visualScale[visualIndex] =  Mathf.Clamp( scaleY, 0, maxScale);
            }

            //visualList[visualIndex].localScale = new Vector3(1,0,height) + Vector3.up *  visualScale[visualIndex];
            //visualList[(visualList.Length - 1) - visualIndex].localScale = visualList[visualIndex].localScale;
            //var pos = visualList[visualIndex].localPosition;
            //visualList[visualIndex].position = Vector3.ClampMagnitude(visualList[visualIndex].position, 10);
            //pos.y = visualList[visualIndex].localScale.y / 2;
            //visualList[visualIndex].position = pos;
            visualIndex++;
            //var scale = visualList[visualIndex].localScale;
            //scale.y = (sum / avgSize) * 50f;
            //visualList[visualIndex].localScale = Vector3.Lerp(visualList[visualIndex].localScale, scale,0.5f);

            //visualIndex++;
        }
        //Debug.LogError(spectrumIndex);
    }
    public void UpdateVisualSpheric()
    {
        var center = Vector3.zero;
        float spaceBetween = 360f / visualList.Length;
        var currentRot = 0f;
        foreach (var item in visualList)
        {
            item.position = RotatePointAroundPivot(Vector3.forward * 10, Vector3.zero, Vector3.up * currentRot);
            //item.rotation = Quaternion.LookRotation(item.up, item.position);
            currentRot += spaceBetween;
        }
        Debug.LogError(currentRot);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void WriteRMS(Text txt)
    {
        txt.text = rmsValue.ToString();
    }

    public void WriteDBValue(Text txt)
    {
        txt.text = dbValue.ToString();
    }

    public void WritePitch(Text txt)
    {
        txt.text = pitchValue.ToString();
    }
}
