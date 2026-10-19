using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class tankController : MonoBehaviour
{
	public Rigidbody2D[] wheels;

	private bool driving;

	private LineRenderer line;

	public Transform node1;

	public Transform node2;

	public List<Vector3> points = new List<Vector3>();

	private void Start()
	{
		line = GetComponent<LineRenderer>();
	}

	public void Use()
	{
		driving = !driving;
	}

	private void Update()
	{
		points.Clear();
		points.Add(node1.position);
		Rigidbody2D[] array = wheels;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 item = array[i].transform.position - 0.32f * base.transform.up;
			points.Add(item);
		}
		points.Add(node2.position);
		line.SetPositions(points.ToArray());
	}

	private void FixedUpdate()
	{
		if (driving)
		{
			float angularVelocity = ((base.transform.root.localScale.x != -1f) ? 1 : (-1)) * 1256;
			Rigidbody2D[] array = wheels;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].angularVelocity = angularVelocity;
			}
		}
		else
		{
			Rigidbody2D[] array = wheels;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].angularVelocity *= 0.01f;
			}
		}
	}
}
