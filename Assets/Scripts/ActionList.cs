using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using UnityEngine.UI;
using TMPro;

public class ActionList : MonoBehaviour
{

    [SerializeField] GameObject actionCard;

    List<GameObject> actions = new List<GameObject>();

    ActionManager actionManager;


    // Start is called before the first frame update
    void Start()
    {
        actionManager = FindObjectOfType<ActionManager>();
        foreach ((ActionObject, int) action in actionManager.GetAllActions())
        {
            GameObject newAction = Instantiate(actionCard) as GameObject;
            newAction.transform.SetParent(this.transform, false);
            if (newAction != null)
            {
                actions.Add(newAction);
            }
            newAction.GetComponentInChildren<Image>().sprite = action.Item1.icon;
            if (action.Item1.action == Action.Attack)
            {
                newAction.GetComponentInChildren<TextMeshProUGUI>().text = "Hit";
            }
            else if (action.Item2 == 0)
            {
                newAction.GetComponentInChildren<TextMeshProUGUI>().text = "Any";
            }
            else if (action.Item2 < 0)
            {
                newAction.GetComponentInChildren<TextMeshProUGUI>().text = "Run";
            }
            else
            {
                newAction.GetComponentInChildren<TextMeshProUGUI>().text = action.Item2.ToString();
            }

        }
        actions[0].GetComponent<Animator>().SetBool("IsSelected", true);
    }

    public void AdvanceAction()
    {
        StartCoroutine(PlayCard());
    }

    private IEnumerator PlayCard()
    {
        if (actions.Count <= 0) { yield break; }
        GameObject playedAction = actions[0];
        playedAction.GetComponent<Animator>().SetTrigger("IsPlayed");
        yield return new WaitForSeconds(.6f);
        Destroy(playedAction);
        actions.RemoveAt(0);
        if (actions.Count <= 0) { yield break; }
        actions[0].GetComponent<Animator>().SetBool("IsSelected", true);
    }
}
