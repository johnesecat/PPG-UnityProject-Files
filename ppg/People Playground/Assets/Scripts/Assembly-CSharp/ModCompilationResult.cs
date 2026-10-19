public struct ModCompilationResult
{
	public bool Success;

	public string Errors;

	public bool Suspicious;

	public ModCompilationResult(bool success, string errors, bool suspicious)
	{
		Success = success;
		Errors = errors;
		Suspicious = suspicious;
	}
}
