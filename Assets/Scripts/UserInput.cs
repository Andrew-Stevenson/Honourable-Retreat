using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class UserInput : MonoBehaviour
{

    private PlayerActions actionsController;
    private ActionArrowDisplay arrowDisplay;

    public bool allowClick = true;

    // Start is called before the first frame update
    void Start()
    {
        actionsController = GetComponent<PlayerActions>();
        arrowDisplay = GetComponent<ActionArrowDisplay>();
    }

    // Update is called once per frame
    void Update()
    {            
        if (Input.GetButtonDown("Fire1")) {
            if (allowClick)
            {
                (Direction, int) actionInput = arrowDisplay.GetArrow();
                actionsController.PreformAction(actionInput.Item1, actionInput.Item2);
            }            
        }

    }
}
