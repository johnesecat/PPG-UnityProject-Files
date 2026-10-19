using System.Collections.Generic;
using UnityEngine;

public abstract class Pool<T> where T : class
{
	public readonly int MaximumCapacity;

	private readonly Stack<T> freeToUse;

	private readonly List<T> currentlyInUse;

	public int CreatedAmount { get; private set; }

	public int AmountInUse => currentlyInUse.Count;

	public Pool(int maxCapacity = 1000)
	{
		MaximumCapacity = maxCapacity;
		freeToUse = new Stack<T>(maxCapacity);
		currentlyInUse = new List<T>(maxCapacity);
	}

	public virtual T RequestObject()
	{
		if (freeToUse.Count > 0)
		{
			return GetExistingFromPool();
		}
		if (CreatedAmount < MaximumCapacity)
		{
			Prefill();
			return GetExistingFromPool();
		}
		Debug.LogWarning("Pool can't satisfy demand because the MaximumCapacity is too low.");
		return GetOverCapacityFallback();
	}

	public virtual void ReturnToPool(T obj)
	{
		if (currentlyInUse.Remove(obj))
		{
			freeToUse.Push(obj);
		}
		else
		{
			Debug.LogWarning("Object that was returned to the pool was already in the pool.");
		}
	}

	public virtual void Prefill()
	{
		T item = CreateFresh();
		CreatedAmount++;
		freeToUse.Push(item);
	}

	protected virtual T GetExistingFromPool()
	{
		if (freeToUse.Count == 0)
		{
			return null;
		}
		T val = freeToUse.Pop();
		ResetObjectForNextUse(val);
		currentlyInUse.Add(val);
		return val;
	}

	protected abstract T CreateFresh();

	protected abstract void ResetObjectForNextUse(T obj);

	protected abstract T GetOverCapacityFallback();
}
