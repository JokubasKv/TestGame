using UnityEngine;
using UnityEngine.Events;

public class scr_ShotEffect : MonoBehaviour
{
    public GameObject effect;
    public bool playOnGoodGraphics;
    bool graphicSettings;

    public UnityEvent unityEvent;
    private void Start()
    {
        int settings=PlayerPrefs.GetInt("Graphics");
        if (settings==1)
            graphicSettings = true;
        else
            graphicSettings = false;

        Debug.Log(settings);
    }

    void Update()
    {
        int settings = PlayerPrefs.GetInt("Graphics");
        if (settings == 1)
            graphicSettings = true;
        else
            graphicSettings = false;
    }

    public void PlayEffect(Vector3 tr)
    {
        if (!playOnGoodGraphics)
        {
            Instantiate(effect, tr, Quaternion.identity);
            return;
        }
        if (playOnGoodGraphics && graphicSettings)
        {
            Instantiate(effect, tr, Quaternion.identity);
       }
    }
}
