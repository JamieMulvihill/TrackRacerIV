using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public CarList carList_;
    public GameObject[] spawnPoints = new GameObject[4];

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i != PlayerPrefs.GetInt("index"))
            {
                GameObject enemyCar = Instantiate(carList_.Cars[i], spawnPoints[i].transform.position, Quaternion.identity) as GameObject;
                enemyCar.name = carList_.Cars[i].name;
                enemyCar.tag = "Opponent";
                enemyCar.GetComponent<InputManager>().driveController = InputManager.DRIVER.AI;
            }
            else {
                GameObject playerCar = Instantiate(carList_.Cars[i], spawnPoints[i].transform.position, Quaternion.identity) as GameObject;
                playerCar.name = carList_.Cars[i].name;
                playerCar.tag = "Player";
                playerCar.GetComponent<InputManager>().driveController = InputManager.DRIVER.HUMAN;
            }
        }
    }

}
