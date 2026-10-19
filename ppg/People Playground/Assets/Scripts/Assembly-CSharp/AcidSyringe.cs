using UnityEngine;

public class AcidSyringe : SyringeBehaviour
{
	public class AcidLiquid : GorseBlood
	{
		public new const string ID = "ACID";

		public override string GetDisplayName()
		{
			return "Acid";
		}

		public AcidLiquid()
		{
			Color = new Color(0.49f, 0.44f, 0.52f);
		}
	}

	public override string GetLiquidID()
	{
		return "ACID";
	}
}
