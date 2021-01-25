using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
    public GameObject stage_;
    private GameObject car_;
    public float rotationSpeed_;
    public CarList carList_;
    public Text carName;
    private int carIndex_ = 0;
    GameObject selectedCar;

    private void Start()
    {
        selectedCar = Instantiate(carList_.Cars[carIndex_], Vector3.zero, Quaternion.identity) as GameObject;
        selectedCar.transform.parent = stage_.transform;
        selectedCar.name = carList_.Cars[carIndex_].name;
        carName.text = selectedCar.name;
        PlayerPrefs.SetInt("index", carIndex_);
    }
    private void FixedUpdate()
    {
        stage_.transform.Rotate(Vector3.up * rotationSpeed_ * Time.deltaTime);
    }
    public void RightButtonClick() {
        if (carIndex_ < carList_.Cars.Length - 1) {
            Destroy(selectedCar);
            carIndex_++;
            PlayerPrefs.SetInt("index", carIndex_);
            selectedCar = Instantiate(carList_.Cars[carIndex_], Vector3.zero, Quaternion.identity) as GameObject;
            selectedCar.transform.parent = stage_.transform;
            selectedCar.name = carList_.Cars[carIndex_].name;
            carName.text = selectedCar.name;
        }
    }

    public void LeftButtonClick()
    {
        if (carIndex_ > 0 )
        {
            Destroy(selectedCar);
            carIndex_--;
            PlayerPrefs.SetInt("index", carIndex_);
            selectedCar = Instantiate(carList_.Cars[carIndex_], Vector3.zero, Quaternion.identity) as GameObject;
            selectedCar.transform.parent = stage_.transform;
            selectedCar.name = carList_.Cars[carIndex_].name;
            carName.text = selectedCar.name;
        }
    }

    public void StartGame() {
        SceneManager.LoadScene("Racing");
    }
}
