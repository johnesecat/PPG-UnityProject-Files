using Linefy;
using UnityEngine;

public class PressureGridController : MonoBehaviour
{
	public int Width = 64;

	public int Height = 64;

	public float GridScale = 10f;

	public WidthMode WidthMode;

	public float Retention = 0.8f;

	public float BalanceForce = 0.6f;

	public float BrushDistance = 5f;

	public Grid<PressureCell> Grid;

	private Dots dots;

	private void Awake()
	{
		Grid = new Grid<PressureCell>(Width, Height);
		dots = new Dots(Width * Height);
		for (int i = 0; i < Grid.Flat.Length; i++)
		{
			Grid.Flat[i] = new PressureCell();
		}
		dots.transparent = true;
		dots.boundSize = -1f;
	}

	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Backslash))
		{
			for (int i = 0; i < Grid.Width; i++)
			{
				for (int j = 0; j < Grid.Height; j++)
				{
					if (Vector2.Distance(GetWorldPosition(i, j), Global.main.MousePosition) < BrushDistance)
					{
						Grid.Get(i, j).Density.Current = 1f;
					}
				}
			}
		}
		for (int k = 0; k < Grid.Width; k++)
		{
			for (int l = 0; l < Grid.Height; l++)
			{
				PressureCell pressureCell = Grid.Get(k, l);
				float previous = pressureCell.DensityVelocity.Previous;
				previous *= Retention;
				previous += 0f - pressureCell.Density.Previous * BalanceForce;
				pressureCell.DensityVelocity.Current = previous;
				pressureCell.Density.Current += previous;
			}
		}
		for (int m = 0; m < Grid.Width; m++)
		{
			for (int n = 0; n < Grid.Height; n++)
			{
				PressureCell pressureCell2 = Grid.Get(m, n);
				pressureCell2.Density.Previous = pressureCell2.Density.Current;
			}
		}
	}

	public Vector2 GetWorldPosition(int x, int y)
	{
		return new Vector2(x, y) * GridScale;
	}

	private void LateUpdate()
	{
		RenderGrid();
	}

	private void RenderGrid()
	{
		dots.widthMode = WidthMode;
		int num = 0;
		foreach (var item2 in Grid)
		{
			PressureCell item = item2.Value;
			float width = item.Density.Current * 5f + 5f;
			Vector2 vector = new Vector2(item2.X, item2.Y) * GridScale;
			Color color = Color.Lerp(Color.white, Color.red, item.Density.Current);
			color.a = 1f;
			dots.SetEnabled(num, enabled: true);
			dots.SetPositionAndWidth(num, vector, width);
			dots.SetColor(num, color);
			num++;
		}
		dots.Draw();
	}
}
