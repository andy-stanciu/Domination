using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapObject<T>
{
    private T[] objects;
    private int currentItemCount;

    public Heap(int maxSize)
    {
        objects = new T[maxSize];
    }

    public int Count { get { return currentItemCount; } }

    public void UpdateObject(T obj)
    {
        SortUp(obj);
    }

    public void Add(T obj)
    {
        obj.HeapIndex = currentItemCount;
        objects[currentItemCount] = obj;
        SortUp(obj);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstObj = objects[0];
        currentItemCount--;
        objects[0] = objects[currentItemCount];
        objects[0].HeapIndex = 0;
        SortDown(objects[0]);

        return firstObj;
    }

    public bool Contains(T obj)
    {
        return Equals(objects[obj.HeapIndex], obj);
    }

    private void SortDown(T obj)
    {
        while (true)
        {
            int childIndexLeft = obj.HeapIndex * 2 + 1;
            int childIndexRight = obj.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (objects[childIndexLeft].CompareTo(objects[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (obj.CompareTo(objects[swapIndex]) < 0)
                {
                    Swap(obj, objects[swapIndex]);
                }
                else return;
            }
            else return;
        }
    }

    private void SortUp(T obj)
    {
        int parentIndex = (obj.HeapIndex - 1) / 2;

        while (true)
        {
            T parentObj = objects[parentIndex];

            if (obj.CompareTo(parentObj) > 0)
            {
                //Lower f cost
                Swap(obj, parentObj);
            }
            else break;

            parentIndex = (obj.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T obj1, T obj2)
    {
        objects[obj1.HeapIndex] = obj2;
        objects[obj2.HeapIndex] = obj1;

        int buffer = obj1.HeapIndex;
        obj1.HeapIndex = obj2.HeapIndex;
        obj2.HeapIndex = buffer;
    }
}
