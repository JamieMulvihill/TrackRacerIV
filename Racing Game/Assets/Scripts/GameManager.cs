using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CarList list_;
    private Controller[] controller_;
    private Controller playerController_;
    public GameObject needle_;
    public GameObject spawnPoint_;
    public GameObject playerCar;
    public Text kphText_;
    public Text gearNumText_;
    public Text RPMText_;
    private float startPos_ = -155f, endPos_ = -389f, currentPos_;
    //public CarList carList_;
    public GameObject[] spawnPoints = new GameObject[4];
    public AudioSource gameTrack;

    // Start is called before the first frame updat
    private void Start()
    {
        controller_ = GameObject.FindObjectsOfType<Controller>();
        for (int i = 0; i < controller_.Length; i++)
        {
            if (controller_[i].gameObject.tag == "Player")
            {
                playerController_ = controller_[i];
            }
        }
        gameTrack.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
            //currentSpeed_ = controller_.KPH;
            kphText_.text = playerController_.KPH.ToString("0");
            UpdateNeedle();
       
    }

    void UpdateNeedle() {
        
            currentPos_ = startPos_ - endPos_;
            float temp = playerController_.engineRPM_ / 10000;
            RPMText_.text = temp.ToString();
            needle_.transform.eulerAngles = new Vector3(0, 0, startPos_ - temp * currentPos_);
        
    }

    public void ChangeGear() {
        for (int i = 0; i < controller_.Length; i++)
        {
            if (controller_[i].gearChange)
            {
                int gear = 1 + controller_[i].currentGear_;
                if (controller_[i] == playerController_)
                {
                    gearNumText_.text = gear.ToString();
                }
                controller_[i].gearChange = false;
            }
        }
    }
    public void MenuClick() {
        SceneManager.LoadScene("Start");
    }
}
