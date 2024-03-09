using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* How to use:
 * - Add script to Waypoint objects of an MTS Lane
 * - Add Colliders to Waypoint objects, configure size
 * - add ALL lanes of this road to allLaneHolders
 * - add adjacent lanes of this road to adjacentLaneHolders
 */

public class ChangeLaneDetect : MonoBehaviour
{
    [SerializeField]
    public GameObject[] allLaneHolders;
    [SerializeField]
    public GameObject[] adjacentLaneHolders;
    private GameObject currentLaneHolder;

    void Start() {
        currentLaneHolder = transform.parent.gameObject;

        // OPTIMIZATION: remove adjacent (and current) lanes
        // to allLaneHolders since they are handled differently
        // (see OnTriggerEnter below)
        // Hence, the lanes in adjacentLaneHolders do NOT get disabled first
        // and then re-enabled in the next for loop
        List <GameObject> adjacentList = adjacentLaneHolders.ToList();
        adjacentList.Add(currentLaneHolder);

        allLaneHolders = (allLaneHolders.ToList()
                          .Except(adjacentList)
                          .ToArray());
    }

    void OnTriggerEnter (Collider other) {
        GameObject currentLaneHolder = transform.parent.gameObject;
        Debug.Log("Entered Lane " + currentLaneHolder);
        // Disable all colliders in this lane since we don't
        //   need to check that we are in the same lane
        currentLaneHolder.SetActive(false);

        // Disable all lane colliders
        foreach (GameObject laneHolder in allLaneHolders) {
            laneHolder.SetActive(false);
        }

        // only enable adjacent lane colliders
        foreach (GameObject laneHolder in adjacentLaneHolders) {
            laneHolder.SetActive(true);
        }
    }
}
