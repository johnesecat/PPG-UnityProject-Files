namespace Ceras.Resolvers
{
	public interface IExternalObjectResolver
	{
		void Resolve<T>(int id, out T value);
	}
}
