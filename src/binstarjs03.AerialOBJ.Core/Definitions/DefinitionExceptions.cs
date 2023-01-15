using System;
using System.Text.Json;

namespace binstarjs03.AerialOBJ.Core.Definitions;

public class KindNotFoundException : JsonException
{
	public KindNotFoundException() { }
	public KindNotFoundException(string message) : base(message) { }
	public KindNotFoundException(string message, Exception inner) : base(message, inner) { }
}

public class UnrecognizedDefinitionKindException : Exception
{
	public UnrecognizedDefinitionKindException() { }
	public UnrecognizedDefinitionKindException(string message) : base(message) { }
	public UnrecognizedDefinitionKindException(string message, Exception inner) : base(message, inner) { }
}