using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public GameObject player_;
    private Controller controller_;
    public GameObject cameraBoom_;
    public float speed_;
    public float defaultFOV_ = 0f, boostFOV_ = 120f;
    [Range(0, 2)] public float smoothTime_;
    [SerializeField] private Camera camera_;
    private void Start()
    {
        player_ = GameObject.FindGameObjectWithTag("Player");
        
        cameraBoom_ = player_.transform.GetChild(3).gameObject;
        controller_ = player_.GetComponent<Controller>();
        defaultFOV_ = camera_.fieldOfView;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CameraTrail();
        BoostFOVCalculation();

        //This will change the camera slide in.
        //speed_ = (controller_.KPH >= 80) ? 8 : controller_.KPH / 10;
    }

    private void CameraTrail() {
        speed_ = Mathf.Lerp(speed_, controller_.KPH / 10, Time.deltaTime);

        gameObject.transform.position = Vector3.Lerp(transform.position, cameraBoom_.transform.position, Time.deltaTime * speed_);
        gameObject.transform.LookAt(player_.gameObject.transform.position);
    }
    void BoostFOVCalculation()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            camera_.fieldOfView = Mathf.Lerp(camera_.fieldOfView, boostFOV_, Time.deltaTime * smoothTime_);
        }
        else
            camera_.fieldOfView = Mathf.Lerp(camera_.fieldOfView, defaultFOV_, Time.deltaTime * smoothTime_);
    }
}
