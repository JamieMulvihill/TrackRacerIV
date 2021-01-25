using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public AudioSource buttonSound;
    public AudioClip soundEffect;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Racing") {
            DontDestroyOnLoad(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Racing") {
            Destroy(gameObject);
        }
    }

    public void PlayButtonClick() {

        buttonSound.PlayOneShot(soundEffect);
        SceneManager.LoadScene("Start");
    }

    public void QuitButtonClick()
    {
        Application.Quit();
    }
}
