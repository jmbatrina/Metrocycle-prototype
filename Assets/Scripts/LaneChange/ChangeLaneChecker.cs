using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeLaneChecker : MonoBehaviour
{
    public GameObject bikeLane;
    public GameObject busLane;

    [SerializeField]
    public GameObject[] bicycleAllowedInLanes;
    public bool isBikeRoad;     // if true, bikes are allowed in all lanes
    private HashSet<GameObject> bicycleAllowed_Set = new HashSet<GameObject>();

    private blinkers blinkerScript;

    private int previousLane;
    private string errorText = "";
    private string blinkerName;

    private float lastDetectTime;

    void Start(){
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();

        previousLane = -1;
        blinkerName = GameManager.Instance.blinkerName();

        foreach (GameObject lane in bicycleAllowedInLanes) {
            bicycleAllowed_Set.Add(lane);
        }

        bicycleAllowed_Set.Add(bikeLane);
        lastDetectTime = -1;
    }

    public void enteredLane(GameObject lane) {
        string lanePrefix = Metrocycle.Constants.laneNamePrefix;
        int lanePartStart = lane.name.LastIndexOf(lanePrefix) + lanePrefix.Length;
        int newLane = int.Parse(lane.name.Substring(lanePartStart));

        // NOTE: Problem: last remembered lane is "sticky"
        //  e.g. if we have two roads each with 2 lanes  ===(A) ====(B)
        //       if bike leaves road A at lane 1, drives through road B and changes to lane 2
        //       then in the perspective of road A, bike is still at lane 1
        // HACK: we make roads "forget" the last lane when enough time has passed (Set to 2s)
        //       assumption: current lane within road is updated regularly; this is true since
        //       we have evenly spaced lane detects of small enough size within roads (assuming use of MTS_ER3D automated waypoints)
        if (Time.time - lastDetectTime > 2) {
            previousLane = -1;
        }
        lastDetectTime = Time.time;

        checkEnteredBusOrBikeLane(lane);
        checkBicycleEnteredForbiddenLane(lane);
        checkBlinkerForLaneChange(newLane);
    }

    public void checkBlinkerForLaneChange(int newLane) {
        if (previousLane == -1) {
            // This is the first lane we entered so just record it and do nothing;
            previousLane = newLane;
            return;
        }

        if (newLane == previousLane) {
            return;
        }

        // HACK: For now, lets assume that the lanes on a road are numbered
        //       increasing from 0, left to Right
        //       left lanes have even values, right lanes have odd values
        Direction direction = (newLane > previousLane) ? Direction.RIGHT : Direction.LEFT;

        // changed parity => moved from right lane to left lane
        if (newLane % 2 != previousLane % 2) {
            direction = Direction.LEFT;
        }

        Debug.Log("Changed lane to the " + ((direction == Direction.LEFT) ? "Left" : "Right"));

        GameManager.Instance.checkProperTurnOrLaneChange(direction);

        previousLane = newLane;

        // Successful lane change, reset blinkerActivationTime
        // this is to prevent changing multiple lanes at once
        // HACK: modify property directly. Should use func/message
        blinkerScript.blinkerActivationTime = Time.time;
    }

    public void checkEnteredBusOrBikeLane(GameObject lane) {
        if (GameManager.Instance.getBikeType() == Metrocycle.BikeType.Bicycle) {
            return;
        }

        if (lane == bikeLane) {
            errorText = "Motorcycles are not allowed on the Bike Lane!";

            GameManager.setErrorReason(Metrocycle.ErrorReason.EXCLUSIVE_BIKELANE);
        } else if (lane == busLane) {
            errorText = "The Bus lane is only for public utility buses (PUBs) and ambulances or goverment vehicles responding to emergencies.";

            GameManager.setErrorReason(Metrocycle.ErrorReason.EXCLUSIVE_BUSLANE);
        } else {
            return;
        }

        GameManager.Instance.PopupSystem.popError(
            "Uh oh!", errorText
        );
    }

    public void checkBicycleEnteredForbiddenLane(GameObject lane) {
        if (GameManager.Instance.getBikeType() != Metrocycle.BikeType.Bicycle
            || isBikeRoad
        ) {
            return;
        }

        if (!bicycleAllowed_Set.Contains(lane)) {
            errorText = "Bicycles are not allowed in this lane which is used by motored vehicles.";
            GameManager.Instance.PopupSystem.popError(
                "Uh oh!", errorText
            );

            GameManager.setErrorReason(Metrocycle.ErrorReason.BIKE_NOTALLOWED);
        }
    }

    public void addBikeAllowedLane(GameObject lane) {
        bicycleAllowed_Set.Add(lane);
    }
}