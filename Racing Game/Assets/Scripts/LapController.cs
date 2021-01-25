using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LapController : MonoBehaviour
{
    public int totalLaps, currentLap;
    public Transform currentWaypoint, lapLinewaypoint;
    InputManager inputManager;
    Controller controller;
    public Text lapNum, maxLaps;
   
    PlacementController placementController;

    // Start is called before the first frame update
    void Start()
    {
        totalLaps = 3;
        currentLap = 0;
        inputManager = GetComponent<InputManager>();
        controller = GetComponent<Controller>();
        placementController = FindObjectOfType<PlacementController>();

        if (SceneManager.GetActiveScene().name == "Racing")
        {
            lapLinewaypoint = inputManager.waypoints.nodes[0];
            lapNum = GameObject.FindGameObjectWithTag("LapNum").GetComponent<Text>();
            maxLaps = GameObject.FindGameObjectWithTag("maxLaps").GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Start") return;
        if (gameObject.tag == "Player")
        {
            maxLaps.text = "/" + totalLaps.ToString();
            lapNum.text = currentLap.ToString();
        }
        currentWaypoint = inputManager.currentWaypoint;
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (currentWaypoint == lapLinewaypoint && other.tag == "LapLine")
        {          
            currentLap++;
            
            controller.lapCount = currentLap;

            if (currentLap > 3)
            {
                // Race is over
                if (gameObject.tag == "Player") {
                    placementController.raceOver = true;
                }
            }
        }
    }
}
