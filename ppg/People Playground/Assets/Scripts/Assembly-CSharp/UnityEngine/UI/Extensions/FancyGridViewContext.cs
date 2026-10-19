using System;

namespace UnityEngine.UI.Extensions
{
	public class FancyGridViewContext : IFancyGridViewContext, IFancyScrollRectContext, IFancyCellGroupContext
	{
		ScrollDirection IFancyScrollRectContext.ScrollDirection { get; set; }

		Func<(float ScrollSize, float ReuseMargin)> IFancyScrollRectContext.CalculateScrollSize { get; set; }

		GameObject IFancyCellGroupContext.CellTemplate { get; set; }

		Func<int> IFancyCellGroupContext.GetGroupCount { get; set; }

		Func<float> IFancyGridViewContext.GetStartAxisSpacing { get; set; }

		Func<float> IFancyGridViewContext.GetCellSize { get; set; }
	}
}
