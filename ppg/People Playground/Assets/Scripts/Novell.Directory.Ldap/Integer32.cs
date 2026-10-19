public class Integer32
{
	private int _wintv;

	public int intValue
	{
		get
		{
			return _wintv;
		}
		set
		{
			_wintv = value;
		}
	}

	public Integer32(int ival)
	{
		_wintv = ival;
	}
}
