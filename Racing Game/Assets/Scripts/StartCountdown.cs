using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCountdown : MonoBehaviour
{
    public Image countDown;
    public Sprite[] sprites = new Sprite[5];
    public float currentTime;
    public int currentSecond;
    public bool raceStarted = false;
    public AudioSource source;
    public AudioClip secondSound, goSound;

    // Start is called before the first frame update
    void Start()
    {
        currentSecond = 0;
        currentTime = 0;
        countDown.sprite = sprites[currentSecond];
        countDown.gameObject.SetActive(true); 
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > currentSecond + 1 && currentSecond < 3)
        {
            source.PlayOneShot(secondSound);
            currentSecond++;
            countDown.sprite = sprites[currentSecond];
        }
       
        else if (currentTime > currentSecond + 1 && currentSecond == 3 && !raceStarted) {
            StartCoroutine(Go());
            countDown.sprite = sprites[currentSecond + 1];
            source.PlayOneShot(goSound);
            raceStarted = true;
        }
    }

    IEnumerator Go() {
        yield return new WaitForSeconds(.5f);
        countDown.gameObject.SetActive(false);
    }
}
