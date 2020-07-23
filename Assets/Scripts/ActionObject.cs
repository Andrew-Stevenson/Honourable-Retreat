using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

[CreateAssetMenu(menuName ="ActionObject")]
public class ActionObject : ScriptableObject
{
    public Action action;
    public Direction direction;
    public Sprite icon;
}
