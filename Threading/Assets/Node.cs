using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Russell
{
    [Serializable]
    public class Node
    {
        public float gCost;
        public float hCost;
        public float fCost = Single.MaxValue;
        public bool isBlocked;
        public Node parentNode;
        public Vector3 position;
        public Vector2Int gridPosition;


    }
}
