using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    компонент для связи игровых объектов на сцене с картой.
*/
public class MazeActor : MonoBehaviour
{
    public enum MoveStatus
    {
        Ok,
        CellOcupied,
        InvalidDirection
    }

    protected virtual void Start()
    {

    }

    public virtual void MakeMove() { }

    protected bool MoveTo(Vector2Int direction)
    {
        return true;
    }
}
