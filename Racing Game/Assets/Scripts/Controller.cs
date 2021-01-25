using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    internal enum DriveType {
        FRONTWHEELDRIVE,
        REARWHEELDRIVE,
        FOURWHEELDRIVE
    }

    internal enum GearBox
    {
        AUTOMATIC,
        MANUAL
    }

    public GameObject centreOfMass;
    private GameManager gameManager_;
    [SerializeField] private DriveType driveType_;
    [SerializeField] private GearBox gearType_;
    private InputManager inputManager_;
    private Rigidbody rigidbody_;
    private WheelCollider[] wheels_ = new WheelCollider[4];
    private GameObject[] wheelMesh_ = new GameObject[4];
    private GameObject wheelMeshs_;
    private GameObject wheelColliders_;
    private GameObject nitrousEffect;
    private bool inReverse = false;
    private WheelFrictionCurve forwardFriction, sidewaysFriction;

    public AnimationCurve enginePower_;
    public float maxPower_;
    public float torque_ = 2000f;
    public float steerFactor_ = 4f;
    public float radius_ = 6;
    public float KPH = 0f;
    public float downforce_ = 50f;
    public float brakePower_ = 0f;
    public float nitrous_;
    public float wheelsRPM_ = 0f;
    public float engineRPM_ = 0f;
    public float minRPM, maxRPM;
    public float[] gears = new float[5];
    public float[] slip = new float[4];
    public float smoothTime_;
    public float handbrakeFriction = 2f;
    public float handbrakeFrictionMultiplier = 2f;
    public float tempo = 0;
    public float driftFactor = 0f;
    public float distanceToCheckpoint = 0;
    public int currentGear_;
    public bool toggleSmoke = false;
    public bool gearChange = false;
    public bool raceStarted = false;

    public int lapCount, currentCheckpoint;

    public AudioSource nitrousSource;
    public AudioClip nitrousEffectSound;

    // Start is called before the first frame update
    void Start()
    {
        inputManager_ = GetComponent<InputManager>();
        rigidbody_ = GetComponent<Rigidbody>();
        //centreOfMass = GameObject.Find("CentreMass");
        centreOfMass = gameObject.transform.GetChild(4).gameObject;

        wheelMeshs_ = gameObject.transform.GetChild(1).gameObject;
        //wheelMeshs_ = GameObject.Find("WheelMesh");
        wheelMesh_[0] = wheelMeshs_.transform.Find("Tire_FL").gameObject;
        wheelMesh_[1] = wheelMeshs_.transform.Find("Tire_FR").gameObject;
        wheelMesh_[2] = wheelMeshs_.transform.Find("Tire_RL").gameObject;
        wheelMesh_[3] = wheelMeshs_.transform.Find("Tire_RR").gameObject;

        wheelColliders_ = gameObject.transform.GetChild(2).gameObject;
        //wheelColliders_ = GameObject.Find("WheelCollider");
        wheels_[0] = wheelColliders_.transform.Find("Tire_FL").GetComponent<WheelCollider>();
        wheels_[1] = wheelColliders_.transform.Find("Tire_FR").GetComponent<WheelCollider>();
        wheels_[2] = wheelColliders_.transform.Find("Tire_RL").GetComponent<WheelCollider>();
        wheels_[3] = wheelColliders_.transform.Find("Tire_RR").GetComponent<WheelCollider>();

        rigidbody_.centerOfMass = centreOfMass.transform.localPosition;

        if (SceneManager.GetActiveScene().name == "Racing" && gameObject.tag == "Player")
        {
            gameManager_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }

        nitrousEffect = gameObject.transform.GetChild(6).gameObject;

        lapCount = 0;
        currentCheckpoint = 8;

    }

    private void FixedUpdate()
    {
        //if in selection screen exit function
        if (SceneManager.GetActiveScene().name == "Start") return;

        if (raceStarted)
        {
            WheelBehavior();
            MoveVehicle();
            Steering();
            ApplyDownforce();
            CalculateFriction();
            CalculateEnginePower();
            ChangeGear();
            CalculateTraction();
        }
    }

    void MoveVehicle()
    {

        if (driveType_ == DriveType.FOURWHEELDRIVE) {
            // Accelerate and Reverse
            for (int i = 0; i < wheels_.Length; i++)
            {
                wheels_[i].motorTorque = maxPower_ / 4;
            }
        }

        else if (driveType_ == DriveType.REARWHEELDRIVE)
        {
            // Accelerate and Reverse
            for (int i = 2; i < wheels_.Length; i++)
            {
                wheels_[i].motorTorque = maxPower_ / 2;
            }
        }

        else 
        { 
            // Accelerate and Reverse
            for (int i = 0; i < wheels_.Length-2; i++)
            {
                wheels_[i].motorTorque = maxPower_ / 2;
            }
        }

        KPH = rigidbody_.velocity.magnitude * 3.6f;

        if (inputManager_.isBoosting_)
        {
            nitrousEffect.SetActive(true);
            rigidbody_.AddForce(Vector3.forward * nitrous_);
            
        }
        else {
            nitrousEffect.SetActive(false);
        }
    }

    void Steering() {


        //Ackerman Steering Formula
        // Right Turn
        if (inputManager_.horizontal_ > 0)
        {
            wheels_[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius_ + (1.5f / 2))) * inputManager_.horizontal_;
            wheels_[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius_ - (1.5f / 2))) * inputManager_.horizontal_;
        }

        // Left Turn
        else if (inputManager_.horizontal_ < 0)
        {
            wheels_[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius_ - (1.5f / 2))) * inputManager_.horizontal_;
            wheels_[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius_ + (1.5f / 2))) * inputManager_.horizontal_;
        }

        else {
            wheels_[0].steerAngle = 0;
            wheels_[1].steerAngle = 0;
        }
    }

    void WheelBehavior() {
        Vector3 wheelPos = Vector3.zero;
        Quaternion wheelRotate = Quaternion.identity;

        for (int i = 0; i < wheels_.Length; i++) {
            wheels_[i].GetWorldPose(out wheelPos, out wheelRotate);
            wheelMesh_[i].transform.position = wheelPos;
            wheelMesh_[i].transform.rotation = wheelRotate;
        }
    }

    void ApplyDownforce() {
        rigidbody_.AddForce(-transform.up * downforce_ * rigidbody_.velocity.magnitude);
    }

    void CalculateEnginePower() {
        CalculateWheelRPM();
        maxPower_ = enginePower_.Evaluate(engineRPM_) * (gears[currentGear_] * inputManager_.vertical_);
        float velocity = 0f;
        engineRPM_ = Mathf.SmoothDamp(engineRPM_, 1000 + (Mathf.Abs(wheelsRPM_) * 3.6f * (gears[currentGear_])), ref velocity, smoothTime_);
    }

    void ChangeGear() {
        if (!isGrounded()) return;

        if (gearType_ == GearBox.AUTOMATIC) {
            if (engineRPM_ > maxRPM && currentGear_ < gears.Length - 1 && !inReverse) {
                currentGear_++;
                gearChange = true;
                if(gameObject.tag == "Player")
                    gameManager_.ChangeGear();
            }
        }

        else{
            if (Input.GetKeyDown(KeyCode.P))
            {
                currentGear_++;
                gearChange = true;
                if (gameObject.tag == "Player")
                    gameManager_.ChangeGear();
            }
        }
        

        if (engineRPM_ < minRPM && currentGear_ > 0) {
            currentGear_--;
            gearChange = true;
            if (gameObject.tag == "Player")
                gameManager_.ChangeGear();
        }
    }

    private bool isGrounded() {
        if (wheels_[0].isGrounded && wheels_[1].isGrounded && wheels_[2].isGrounded && wheels_[3].isGrounded)
            return true;
        else
            return false;
    }

    void CalculateWheelRPM()
    {
        float sum = 0;
        int Revs = 0;
        for (int i = 0; i < wheels_.Length; i++)
        {
            sum += wheels_[i].rpm;
            Revs++;
        }
        wheelsRPM_ = (Revs != 0) ? sum / Revs : 0;

        if (wheelsRPM_ < 0 && !inReverse)
        {
            inReverse = true;
            gearChange = true;
            if (gameObject.tag == "Player")
                gameManager_.ChangeGear();
        }
        else if (wheelsRPM_ > 0 && inReverse)
        {
            inReverse = false;
            gearChange = true;
            if (gameObject.tag == "Player")
                gameManager_.ChangeGear();
        }
    }
    void CalculateFriction() {
        for (int i = 0; i < wheels_.Length; i++) {
            WheelHit wheelHit;
            wheels_[i].GetGroundHit(out wheelHit);

            slip[i] = wheelHit.sidewaysSlip;
        }
    }

    void CalculateTraction()
    {

        float driftSmoothFactor = .7f * Time.deltaTime;

        if (inputManager_.handbrake_)
        {

            sidewaysFriction = wheels_[0].sidewaysFriction;
            forwardFriction = wheels_[0].forwardFriction;

            float velocity = 0;

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handbrakeFriction, ref velocity, driftSmoothFactor);

            for (int i = 0; i < 4; i++)
            {
                wheels_[i].forwardFriction = forwardFriction;
                wheels_[i].sidewaysFriction = sidewaysFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;

            for (int i = 0; i < 2; i++)
            {
                wheels_[i].sidewaysFriction = sidewaysFriction;
                wheels_[i].forwardFriction = forwardFriction;
            }
            rigidbody_.AddForce(transform.forward * (KPH / 400) * 10000);
        }
        else
        {

            sidewaysFriction = wheels_[0].sidewaysFriction;
            forwardFriction = wheels_[0].forwardFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue
                = ((KPH * handbrakeFrictionMultiplier) / 300) + 1;

            for (int i = 0; i < 4; i++)
            {
                wheels_[i].forwardFriction = forwardFriction;
                wheels_[i].sidewaysFriction = sidewaysFriction;
            }
        }

        for (int i = 2; i < 4; i++)
        {
            WheelHit wheelHit;
            wheels_[i].GetGroundHit(out wheelHit);

            if (wheelHit.sidewaysSlip >= 0.2f || wheelHit.sidewaysSlip <= -0.2f /*|| wheelHit.forwardSlip >= .3f || wheelHit.forwardSlip <= -.3f*/)
                toggleSmoke = true;
            else
                toggleSmoke = false;

            if (wheelHit.sidewaysSlip < 0)
                driftFactor = (1 + -inputManager_.horizontal_) * Mathf.Abs(wheelHit.sidewaysSlip);

            if (wheelHit.sidewaysSlip > 0)
                driftFactor = (1 + -inputManager_.horizontal_) * Mathf.Abs(wheelHit.sidewaysSlip);
        }
    }

    private IEnumerator timedLoop() {
        while (true) {
            yield return new WaitForSeconds(.7f);
            radius_ = 6 + KPH / 20;
        }
    }

    public void CheckpointControl()
    {

        if (currentCheckpoint < 8)
        {
            currentCheckpoint++;
        }
        else
        {
            currentCheckpoint = 0;
            lapCount++;
        }
    }
}
