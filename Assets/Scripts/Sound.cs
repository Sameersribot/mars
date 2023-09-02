using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public AudioClip clip;
    [Range(0f,1f)]
    public float volume;
    public float pitch;
}
