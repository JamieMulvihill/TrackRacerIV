using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlacementController : MonoBehaviour
{
    public Controller[] cars = new Controller[4];
    public Controller[] sortedCars = new Controller[4];
    public List<Controller> carList = new List<Controller>();
    public StartCountdown countdown;
    public List<Transform> checkPoints = new List<Transform>();
    public Text[] CarNames = new Text[4];
    public Text playerPosition;
    public Image leaderboard;
    public Button menuButton;

    public bool raceOver = false;

    // Start is called before the first frame update
    void Start()
    {

        if (SceneManager.GetActiveScene().name == "Racing")
        {
            playerPosition = GameObject.FindGameObjectWithTag("Position").GetComponent<Text>();
        }

        countdown = gameObject.GetComponent<StartCountdown>();

        cars = FindObjectsOfType<Controller>();
        for (int i = 0; i < cars.Length; i++) {
            carList.Add(cars[i]);
            CarNames[i].text = cars[i].gameObject.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!raceOver)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                leaderboard.gameObject.SetActive(true);
            }
            else
            {
                leaderboard.gameObject.SetActive(false);
            }

            for (int i = 0; i < carList.Count(); i++)
            {
                carList[i].raceStarted = countdown.raceStarted;
                carList[i].distanceToCheckpoint = (checkPoints[carList[i].currentCheckpoint].transform.position - carList[i].transform.position).magnitude;
                if (carList[i].distanceToCheckpoint < 20)
                {
                    carList[i].CheckpointControl();
                }
                if (carList[i].tag == "Player")
                {
                    playerPosition.text = (i + 1).ToString();
                    CarNames[i].text = carList[i].name + " (Player)";
                }
                else {
                    CarNames[i].text = carList[i].name;
                }

            }
            LeadCheck();
        }
        else {
            leaderboard.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
        }
        
    }
    void LeadCheck() {

        sortedCars = cars.OrderBy(car => car.distanceToCheckpoint).ToArray();

        
        carList.Sort(new CarCompare());
        
    }

    public class CarCompare : IComparer<Controller> {
        public int Compare(Controller carA, Controller carB) {
            int lap = carB.lapCount.CompareTo(carA.lapCount);//Note im comparing X to Y here so its ascending order.

            if (lap != 0)
            {
                //if they have not the same number of unit, return the higher one.
                return lap;
            }
            else {
                //if they have the checkpoint, compare the distance
                int checkpoint = carB.currentCheckpoint.CompareTo(carA.currentCheckpoint);//Note im comparing Y to X here so its descending order.
                if (checkpoint != 0)
                {
                    //If they don't have the same number of units, return the lower one.
                    return checkpoint;
                }
                else
                {

                    int distance = carA.distanceToCheckpoint.CompareTo(carB.distanceToCheckpoint);//Note im comparing Y to X here so its descending order.
                    if (distance != 0)
                    {
                        //If they don't have the same number of units, return the lower one.
                        return distance;
                    }
                    else
                    {
                        //They have the same number of units and score, roll a dice.
                        if (UnityEngine.Random.value > 0.5f)//this is a straight up 50/50
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
        }
    }
}
