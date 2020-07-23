using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ActionManager : MonoBehaviour
{
    [SerializeField] List<ActionObject> actionsList;
    [SerializeField] List<int> amountsList;
    int actionsCounter = 0;

    ActionList actionList;
    ActionArrowDisplay arrowDisplay;

    private void Start()
    {
        if (actionsList.Count != amountsList.Count)
        {
            Debug.LogError("Amounts in actions list don't match!");
        }
        actionList = FindObjectOfType<ActionList>();
        arrowDisplay = FindObjectOfType<ActionArrowDisplay>();
    }

    public (Action, Direction, int) StartNextAction()
    {
        actionsCounter++;
        if (actionList)
        {
            actionList.AdvanceAction();
        }        
        if (actionsCounter >= actionsList.Count)
        {
            return (Action.End, Direction.None, 0);
        }
        arrowDisplay.SetNewArrow(actionsList[actionsCounter].direction, amountsList[actionsCounter]);
        return (actionsList[actionsCounter].action, actionsList[actionsCounter].direction, amountsList[actionsCounter]);
    }

    public (Action, Direction, int) GetCurrentAction()
    {
        return (actionsList[actionsCounter].action, actionsList[actionsCounter].direction, amountsList[actionsCounter]);
    }

    public IEnumerable<(ActionObject, int)> GetAllActions()
    {
        for (int i=0; i < actionsList.Count; i++)
        {
            yield return (actionsList[i], amountsList[i]);
        }
    }
}
