using System;
using System.Collections.Generic;
using UnityEngine;

public class ActivationPropagation : IDisposable
{
	private static readonly TraversalSetPool traversalSetPool = new TraversalSetPool();

	public static readonly HashSet<ActivationPropagation> ActiveSignalSet = new HashSet<ActivationPropagation>();

	public const int MaximumPropagations = 10000;

	public static readonly ushort[] AllChannels = new ushort[3] { 0, 1, 2 };

	public const ushort Green = 0;

	public const ushort Red = 1;

	public const ushort Blue = 2;

	[Obsolete]
	public int Identity;

	public HashSet<UnityEngine.Object> Path;

	public int TraversedCount;

	public bool Direct;

	public ushort Channel;

	public GameObject Sender;

	public GameObject Target;

	internal int PreviousActivityIntensity = -1;

	internal int ActivityIntensity;

	internal static int CurrentlyActiveSignals => traversalSetPool.AmountInUse;

	[Obsolete("Use TraversedCount instead")]
	public int Traversed
	{
		get
		{
			return TraversedCount;
		}
		set
		{
			TraversedCount = value;
		}
	}

	public bool Contains(UnityEngine.Object obj)
	{
		if (Path != null)
		{
			return Path.Contains(obj);
		}
		return false;
	}

	public ActivationPropagation(bool direct, ushort channel = 0, GameObject sender = null)
	{
		Path = traversalSetPool.RequestObject();
		Direct = direct;
		TraversedCount = 1;
		Channel = channel;
		Identity = 0;
		Sender = sender;
		ActiveSignalSet.Add(this);
	}

	public ActivationPropagation(UnityEngine.Object obj, ushort channel = 0, GameObject sender = null)
	{
		Path = traversalSetPool.RequestObject();
		Path.Add(obj);
		Direct = false;
		TraversedCount = 1;
		Channel = channel;
		Identity = 0;
		Sender = sender;
		ActiveSignalSet.Add(this);
	}

	public ActivationPropagation()
	{
		ActiveSignalSet.Add(this);
		Path = traversalSetPool.RequestObject();
	}

	public ActivationPropagation Branch(UnityEngine.Object ob)
	{
		ActivityIntensity++;
		ActivationPropagation activationPropagation = new ActivationPropagation();
		if (Path != null)
		{
			activationPropagation.Path.AddRange(Path);
		}
		activationPropagation.Path.Add(ob);
		activationPropagation.TraversedCount++;
		activationPropagation.Direct = false;
		activationPropagation.Channel = Channel;
		activationPropagation.Identity = Identity;
		return activationPropagation;
	}

	public void Dispose()
	{
		ActiveSignalSet.Remove(this);
		Path.Clear();
		traversalSetPool.ReturnToPool(Path);
	}
}
