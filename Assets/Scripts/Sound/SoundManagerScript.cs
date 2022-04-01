using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip jumpSound, walkSound;
    static AudioSource audioSrc;


    // Start is called before the first frame update
    void Start()
    {
        jumpSound = Resources.Load<AudioClip>("jumpSound");
        walkSound = Resources.Load<AudioClip>("walkSound");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "jumpSound":
                audioSrc.PlayOneShot(jumpSound);
                break;
            case "walkSound":
                audioSrc.PlayOneShot(walkSound);
                break;
            default:
                break;
        }
    }
}
