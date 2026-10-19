public class ToStringEventDataTypeConverter : EventDataTypeConverterBase<string>
{
	public override void Convert(object convertValue)
	{
		string arg = convertValue.ToString();
		Converted?.Invoke(arg);
	}
}
