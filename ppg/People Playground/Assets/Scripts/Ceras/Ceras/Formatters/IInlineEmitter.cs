using System.Linq.Expressions;

namespace Ceras.Formatters
{
	internal interface IInlineEmitter
	{
		Expression EmitWrite(ParameterExpression bufferExp, ParameterExpression offsetExp, ParameterExpression valueExp, out int writtenSize);

		Expression EmitRead(ParameterExpression bufferExp, ParameterExpression offsetExp, ParameterExpression valueExp, out int readSize);
	}
}
