using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class PlayerActions : MonoBehaviour
{

    // Movement logic
    [SerializeField] LayerMask whatBlocksMovement;
    [SerializeField] LayerMask whatIsNextLevel;
    [SerializeField] LayerMask whatIsDeath;
    [SerializeField] LayerMask whatCanDestroy;
    [SerializeField] LayerMask whatCanPush;
    [SerializeField] float moveSpeed = 5f;
    private Vector3 movePoint = new Vector3(0, 0, 0);


    // State logic
    enum State
    {
        WaitingForMovement,
        WaitingForAttack,
        Locked,
        Moving,
        Attacking,
    }
    State currentState = State.Locked;
    int actionAmount = 0;
    Direction actionDirection = Direction.None;

    ActionManager actionsManager;
    ActionArrowDisplay arrowDisplay;
    Animator animator;

    private void Awake()
    {
        actionsManager = FindObjectOfType<ActionManager>();
        arrowDisplay = GetComponent<ActionArrowDisplay>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        movePoint = transform.position;
        SetupAction(actionsManager.GetCurrentAction());
    }

    void SetupAction((Constants.Action, Direction, int) action)
    {
        currentState = State.Locked;

        if (action.Item1 == Constants.Action.End)
        {
            if (CheckForLevelComplete())
            {
                animator.SetTrigger("FadeOut");
                currentState = State.Locked;
                GameManager.instance.AdvanceLevel();
            }
            else
            {
                RestartLevel.instance.ShowRestartCanvas("You're out of moves, and trapped in the dungeon forever");
                return;
            }            
        }


        switch (action.Item1)
        {
            case (Constants.Action.Move):
                currentState = State.WaitingForMovement;
                break;
            case (Constants.Action.Attack):
                currentState = State.WaitingForAttack;
                break;
        }

        actionDirection = action.Item2;
        actionAmount = action.Item3;

        if (action.Item2 != Direction.None && action.Item3 != 0)
        {
            StartCoroutine(PreformActionOnDelay());
        }
    }

    private void Update()
    {
        HandleMovement();
    }

    private IEnumerator PreformActionOnDelay()
    {
        State prevState = currentState;
        currentState = State.Locked;
        yield return new WaitForSeconds(.6f);
        currentState = prevState;
        PreformAction();
    }

    private void HandleMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, movePoint) <= Mathf.Epsilon && currentState == State.Moving) 
        {
            actionAmount--;
            if (actionAmount <= 0 )
            {
                PreformEndOfMoveActions();
            }
            else if (CheckForPlayerDeath())
            {
                Die();
            }
            else
            {
                currentState = State.WaitingForMovement;
                Move();
            }
            
        }
    }

    private bool CheckForLevelComplete()
    {
        var tileOffest = new Vector3(0.4f, 0.4f, 0);
        Collider2D hit = Physics2D.OverlapArea(transform.position + tileOffest, transform.position - tileOffest, whatIsNextLevel);
        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PreformAction(Direction direction=Direction.North, int amount=0)
    {
        if (currentState == State.Locked) { return; }

        arrowDisplay.HideArrow();

        if (actionAmount == 0)
        {
            actionAmount = amount;
        }
        if (actionDirection == Direction.None)
        {
            actionDirection = direction;
        }

        if (currentState == State.WaitingForMovement)
        {
            Move();
        }
        if (currentState == State.WaitingForAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (currentState != State.WaitingForAttack) { return; }
        currentState = State.Attacking;

        //Play animation and yield

        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, getDirectionVector(), 1f, whatCanDestroy);
        if (raycastHit)
        {
            Destroy(raycastHit.collider.gameObject);
        }

        PreformEndOfMoveActions();
    }

    void Move()
    {
        if (currentState != State.WaitingForMovement) { return; }

        animator.SetBool("Walking", true);

        Vector3 moveDirection = getDirectionVector();

        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, moveDirection, 1f, whatBlocksMovement);        
        if (raycastHit)
        {
            ImmovableImpact();
        }
        else
        {
            RaycastHit2D keyHit = Physics2D.Raycast(transform.position, moveDirection, 1f, whatCanPush);
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, moveDirection, 2f, LayerMask.GetMask("Wall"));
            RaycastHit2D doorHit = Physics2D.Raycast(transform.position, moveDirection, 2f, LayerMask.GetMask("Door"));

            if (keyHit && wallHit)
            {
                ImmovableImpact();
            }
            else if (keyHit && doorHit)
            {
                movePoint += moveDirection;

                if (Vector3.Distance(transform.position, movePoint) >= Mathf.Epsilon)
                {
                    currentState = State.Moving;
                }
                actionAmount = 1;
            }
            else
            {
                movePoint += moveDirection;

                if (Vector3.Distance(transform.position, movePoint) >= Mathf.Epsilon)
                {
                    currentState = State.Moving;
                }
            }            
        }
    }

    Vector3 getDirectionVector()
    {
        Vector3 directionVector;

        switch (actionDirection)
        {
            case Direction.North:
                directionVector = new Vector3(0, 1, 0);
                break;
            case Direction.South:
                directionVector = new Vector3(0, -1, 0);
                break;
            case Direction.West:
                directionVector = new Vector3(-1, 0, 0);
                break;
            case Direction.East:
                directionVector = new Vector3(1, 0, 0);
                break;
            default:
                directionVector = new Vector3(0, 0, 0);
                break; 
        }

        return directionVector;
    }

    void ImmovableImpact()
    {
        actionAmount = 0;
        PreformEndOfMoveActions();
    }

    void PreformEndOfMoveActions()
    {

        animator.SetBool("Walking", false);
     
        if (CheckForPlayerDeath())
        {
            Die();
        }
        else
        {
            SetupAction(actionsManager.StartNextAction());
        }
    }

    bool CheckForPlayerDeath()
    {
        var tileOffest = new Vector3(0.4f, 0.4f, 0);
        Collider2D hit = Physics2D.OverlapArea(transform.position + tileOffest, transform.position - tileOffest, whatIsDeath);
        if (hit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    void Die()
    {
        currentState = State.Locked;
        RestartLevel.instance.ShowRestartCanvas();
    }
}
