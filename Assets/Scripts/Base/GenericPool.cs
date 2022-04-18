using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPool<T> where T : class
{

	private List<T> pool = null;
	public Func<T> CreateFunction;

	public Action<T> OnPush, OnPop;
	public int Count => pool.Count;

	public GenericPool(int count, Func<T> CreateFunction = null, Action<T> OnPush = null, Action<T> OnPop = null)
	{
		pool = new List<T>(count);

		this.CreateFunction = CreateFunction;
		this.OnPush = OnPush;
		this.OnPop = OnPop;

		if (count > 0)
		{
			T obj = CreateFunction();
			if (obj == null)
				return;

			Push(obj);

			for (int i = 1; i < count; i++)
				Push(CreateFunction());
		}
	}

	public T Pop()
	{
		T objToPop;

		if (pool.Count == 0)
		{
			objToPop = CreateFunction();
		}
		else
		{
			int index = pool.Count - 1;
			objToPop = pool[index];
			pool.RemoveAt(index);

			while (objToPop == null)
			{
				if (pool.Count > 0)
				{
					objToPop = pool[--index];
					pool.RemoveAt(index);
				}
				else
				{
					objToPop = CreateFunction();
					break;
				}
			}
		}

		OnPop?.Invoke(objToPop);

		return objToPop;
	}

	public void Push(T obj)
	{
		if (obj == null) return;

		OnPush?.Invoke(obj);

		if (pool.Count == 0)
			pool.Add(obj);
		else
		{
			int randomIndex = UnityEngine.Random.Range(0, pool.Count);
			pool.Add(pool[randomIndex]);
			pool[randomIndex] = obj;
		}
	}
}
