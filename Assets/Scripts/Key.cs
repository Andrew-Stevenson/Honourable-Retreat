using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Lock assignedLock;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == assignedLock.gameObject)
        {
            Destroy(assignedLock.gameObject);
            Destroy(gameObject);
        }
    }
}
