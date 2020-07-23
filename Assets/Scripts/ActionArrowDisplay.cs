using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class ActionArrowDisplay : MonoBehaviour
{

    [Header("Sprites")]
    [SerializeField] GameObject moveSquare;
    [SerializeField] GameObject landSquare;
    [SerializeField] GameObject blockSquare;

    [Header("Other")]
    [SerializeField] Transform arrowHolder;

    [Header("Layer Masks")]
    [SerializeField] LayerMask whatBlocksAction;
    [SerializeField] LayerMask whatIsPushable;

    Direction facing = Direction.North;
    bool shouldDraw = true;
    int drawDistance = 2;
    Vector3 drawDirection = new Vector3(0, 1, 0);

    bool directionLocked = false;
    bool amountLocked = false;

    // Start is called before the first frame update
    void Start()
    {

        (Action, Direction, int) firstAction = FindObjectOfType<ActionManager>().GetCurrentAction();
        facing = firstAction.Item2;
        drawDistance = firstAction.Item3;

        if (facing != Direction.None)
        {
            directionLocked = true;
        }
        if (drawDistance != 0)
        {
            amountLocked = true;
        }

        DrawDots();
    }

    // Update is called once per frame
    void Update()
    {

        shouldDraw = false;

        if (!directionLocked)
        {
            shouldDraw = UpdateFacing();
        }

        if (!amountLocked)
        {
            shouldDraw |= UpdateAmount();
        }

        if (shouldDraw)
        {
            DrawDots();
        }
    }

    bool UpdateFacing()
    {
        Direction wasFacing = facing;

        Vector3 mousePosRelative = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float mouseAngle = Vector2.SignedAngle(new Vector2(transform.up.x, transform.up.y), new Vector2(mousePosRelative.x, mousePosRelative.y));

        if (-45f < mouseAngle && mouseAngle < 45f)
        {
            facing = Direction.North;
        }
        else if (45f <= mouseAngle && mouseAngle <= 135f)
        {
            facing = Direction.West;
        }
        else if (mouseAngle <= -135f || mouseAngle >= 135f)
        {            
            facing = Direction.South;
        }
        else
        {            
            facing = Direction.East;
        }

        UpdateDirection();
        return wasFacing != facing;
    }

    void UpdateDirection()
    {
        if (facing == Direction.North)
        {
            drawDirection = new Vector3(0, 1, 0);
        }
        else if (facing == Direction.West)
        {
            drawDirection = new Vector3(-1, 0, 0);
        }
        else if (facing == Direction.South)
        {
            drawDirection = new Vector3(0, -1, 0);
        }
        else
        {
            drawDirection = drawDirection = new Vector3(1, 0, 0); ;
        }
    }

    bool UpdateAmount()
    {
        int previousDistance = drawDistance;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        drawDistance = Mathf.RoundToInt(Vector3.Distance(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(mousePos.x, mousePos.y, 0)));

        if (drawDistance < 1) { drawDistance = 1; }

        return previousDistance != drawDistance;
    }

    void DrawDots()
    {        
        DestroyDots();
        int maxDraw = drawDistance;

        RaycastHit2D pushHit = Physics2D.Raycast(transform.position, drawDirection, maxDraw, whatIsPushable);
        if (pushHit)
        {
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, drawDirection, maxDraw+ 1, LayerMask.GetMask("Wall"));
            if (wallHit)
            {
                maxDraw = Mathf.Max(Mathf.FloorToInt(wallHit.distance) - 1, 1);
            }
        }
        for (int i=1; i<maxDraw + 1; i++)
        {            
            RaycastHit2D hit = Physics2D.Raycast(transform.position + drawDirection * i, drawDirection, 1, whatBlocksAction);
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position + drawDirection * i, -drawDirection, 1, whatBlocksAction);
            RaycastHit2D hit3 = Physics2D.Raycast(transform.position + drawDirection * i, drawDirection, 1, whatIsPushable);
            RaycastHit2D hit4 = Physics2D.Raycast(transform.position + drawDirection * (i+1), -drawDirection, 1, whatBlocksAction);

            if (hit2 && i==1 || hit3 && hit4 && i==1)
            {
                GameObject newDot = Instantiate(blockSquare, transform.position + drawDirection * i, Quaternion.identity) as GameObject;
                newDot.transform.SetParent(arrowHolder);
                if (!amountLocked) { drawDistance = 1; }
                return;               
            }
            if (hit || i == maxDraw)
            {
                GameObject newDot = Instantiate(landSquare, transform.position + drawDirection * i, Quaternion.identity) as GameObject;
                newDot.transform.SetParent(arrowHolder);
                if (!amountLocked) { drawDistance = i; }
                return;
            }
            else
            {
                GameObject newDot = Instantiate(moveSquare, transform.position + drawDirection * i, Quaternion.identity) as GameObject;
                newDot.transform.SetParent(arrowHolder);
            }
        }
    }

    void DestroyDots()
    {
        for (int i=0; i<arrowHolder.childCount; i++)
        {
            Destroy(arrowHolder.GetChild(i).gameObject);
        }

        
    }

    public void HideArrow()
    {
        DestroyDots();
        directionLocked = true;
        amountLocked = true;
    }

    public void SetNewArrow(Direction actionDirection, int actionAmount)
    {
        drawDistance = actionAmount;
        facing = actionDirection;
        shouldDraw = true;

        if (facing != Direction.None)
        {
            directionLocked = true;
            UpdateDirection();
        }
        else
        {
            directionLocked = false;
        }

        if (drawDistance != 0)
        {
            amountLocked = true;
        }
        else
        {
            amountLocked = false;
        }

        if (directionLocked && amountLocked)
        {
            shouldDraw = false;
        }
    }

    public (Direction, int) GetArrow()
    {
        print(drawDistance);
        return (facing, drawDistance);
    }
}
