 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public enum DRIVER {  AI, HUMAN }

    public DRIVER driveController;
    public float vertical_;
    public float horizontal_;
    public bool handbrake_;
    public bool isBoosting_;


    public TrackWaypoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    public Transform currentWaypoint;
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 5)] public float steerForce;


    private Vector3 startPos;
    public int current = 0;
    // Start is called before the first frame update
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Start") return;
        waypoints = GameObject.FindGameObjectWithTag("Path").GetComponent<TrackWaypoints>();
        nodes = waypoints.nodes;
        currentWaypoint = nodes[current];
        startPos = gameObject.transform.position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Start") return;
        switch (driveController) {
            case DRIVER.AI:
                AiDriver();
                break;
            case DRIVER.HUMAN:
                HumanDriver();
                break;
        }
       
    }

    void AiDriver() {
        vertical_ = .92f;
        AiSteer();
    }

    void HumanDriver() {
        vertical_ = Input.GetAxis("Vertical");
        horizontal_ = Input.GetAxis("Horizontal");
        handbrake_ = (Input.GetAxis("Jump") != 0) ? true : false;
        if (Input.GetKey(KeyCode.LeftShift)) isBoosting_ = true; else isBoosting_ = false;

    }
    void AiSteer() {
        Vector3 relative = transform.InverseTransformPoint(currentWaypoint.transform.position);
        horizontal_ = (relative.x / relative.magnitude) * steerForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WayPoint")
        {
            
            if (current == nodes.Count - 1)
            {
                current = 0;
                currentWaypoint = nodes[current];
            }
            else
            {
                if (gameObject.tag == "Player")
                {
                    currentWaypoint = nodes[current];
                    current++;
                }
                else {
                    currentWaypoint = nodes[current + 1];
                    current++;
                }
            }
        }
    }
}
