using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialcolTrigger : MonoBehaviour
{
    enum phaseTrigger {R2, L3, Strafe, Stick }

    [SerializeField]
    phaseTrigger crntTrigger;

    tutorialController tutorialController;

    // Start is called before the first frame update
    void Start()
    {
        tutorialController = FindObjectOfType<tutorialController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (crntTrigger == phaseTrigger.R2)
        {
            tutorialController.setR2Canvas();
        }
        if (crntTrigger == phaseTrigger.L3)
        {
            tutorialController.setL3Canvas();
        }
        if (crntTrigger == phaseTrigger.Strafe)
        {
            tutorialController.setStrafeCanvas();
        }
        if (crntTrigger == phaseTrigger.Stick)
        {
            tutorialController.setStickCanvas();
        }
        Destroy(this);
    }
}
