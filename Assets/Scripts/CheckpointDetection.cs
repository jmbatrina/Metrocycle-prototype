using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckpointDetection : MonoBehaviour
{
    public GameObject pairCollider = null; // activated after collision

    public bool deactivateAfterCollision;

    public bool showPopup = true;
    [TextArea(3, 10)] public string popupTitle;
    [TextArea(3, 10)] public string popupText;

    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered collision with " + other.gameObject.name);
        if (pairCollider != null) {
            pairCollider.SetActive(true);
        }

        if (deactivateAfterCollision) {
            gameObject.SetActive(false);
        }

        if (showPopup) {
            GameManager.Instance.PopupSystem.popPrompt(popupTitle, popupText);
        }
    }
}
