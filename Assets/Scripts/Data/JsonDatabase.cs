using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class JsonDatabase<T>
{
    public List<T> items = new List<T>();
}
