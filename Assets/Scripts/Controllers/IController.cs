using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController 
{
}
public enum State
{
    Alive,
    Dead,
    Sleeping,
    Idle,
    Chasing,
    Hunting, 
    Attacking,
    Fleeing,
    Stunned,
    Investigating,
    Eating,
    Hungry,
}
