using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {

    public List<AudioClip> clips;
    AudioSource source;
    public bool Loop;
    public bool Randomize;
    public bool AutoPlay = true;
    void Start () {
        source = GetComponent<AudioSource>();
        if (AutoPlay) Play();
	}
    public void Play()
    {
        StartCoroutine(Player());
    }
    public void Stop()
    {
        StopAllCoroutines();
        source.Stop();
    }
    public void Pause(bool pause)
    {
        if (pause) source.Pause();
        else source.UnPause();
    }

    private IEnumerator Player()
    {
        var localClips = clips.ToArray();
        if(Randomize)localClips.Shuffle(new System.Random());
        while (true)
        {
            foreach (var item in localClips)
            {
                source.clip = item;
                source.Play();
                do yield return null; while (source.isPlaying);
            }
            if (!Loop) yield break;
            yield return null;
        }
    }
}
