using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialcolTrigger : MonoBehaviour
{
    /*enum phaseTrigger {R2, L3, Strafe, Stick, Boos }

    [SerializeField]
    phaseTrigger crntTrigger;*/

    [SerializeField]
    CanvasGroup CanvasGroup;

    tutorialController tutorialController;

    // Start is called before the first frame update
    void Start()
    {
        tutorialController = FindObjectOfType<tutorialController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        tutorialController.SetCanvas(CanvasGroup);

        Destroy(this);
    }
}
