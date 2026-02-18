using UnityEngine;
using System.Collections.Generic;

public class Pool<T> where T : Component
{
    private Queue<T> pool = new();
    private T prefab;
    private Transform parent;

    public Pool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
            Add();
    }

    private void Add()
    {
        var obj = Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    public T Get()
    {
        if (pool.Count == 0)
            Add();

        var obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
