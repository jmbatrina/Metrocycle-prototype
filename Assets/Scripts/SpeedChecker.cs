using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChecker : MonoBehaviour
{
    public int speedLimit;
    private float speed;
    private float speedMax;

    // amount of allowable extra in speed
    // e.g. limit = 20 and leeway = 3 => warn at speed 23
    public float speedLeeway = 3f;

    void Awake() {
        speed = 0f;
    }

    void OnTriggerStay (Collider other) {
        speedMax = 120f;

        speed = GameManager.Instance.getBikeSpeed();
        if (speed > speedMax) speed = speedMax;
        
        if (speed > speedLimit+speedLeeway){
            Debug.Log("Exceeded speed limit!");
            GameManager.Instance.PopupSystem.popError(
                $"You have exceeded the {speedLimit} kph speed limit!",
                "Make sure to keep an eye on your speedometer."
            );
        }
    }
}
