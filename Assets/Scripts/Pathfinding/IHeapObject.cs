using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHeapObject<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
