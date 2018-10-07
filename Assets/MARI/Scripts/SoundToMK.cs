using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MK.Glow;

public class SoundToMK : MonoBehaviour {

    // Use this for initialization
    public Material[] mkMats;
    public SoundVisual SV;
    public string glowPowerProp = "_MKGlowPower";
    public MKGlowFree manager;
    public float pitchmax;
    public float pitchFall;
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        for (int i = 0; i < mkMats.Length; i++)
        {
            var item = mkMats[i];
            item.SetFloat(glowPowerProp, (SV.visualScale[i]/ SV.maxScale) *2.4f);
            //manager.GlowIntensityInner = SV.
            var invertedPitch =  Mathf.Clamp(SV.pitchValue / pitchmax , 0, 1);
            invertedPitch = 1 - invertedPitch;
            var finalPitch = invertedPitch * 2;
            var pf = finalPitch > manager.BlurSpreadInner ? pitchFall * 2 :  pitchFall;
            manager.BlurSpreadInner = Mathf.Lerp(manager.BlurSpreadInner, Mathf.Clamp(invertedPitch * 2, 0.6f, 2), pf * Time.deltaTime);
        }
	}
}
