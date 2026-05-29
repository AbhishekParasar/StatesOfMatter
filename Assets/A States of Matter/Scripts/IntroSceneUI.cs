using System.Collections;
using UnityEngine;

public class IntroSceneUI : MonoBehaviour
{
    public GameObject introAudioGO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        introAudioGO.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
