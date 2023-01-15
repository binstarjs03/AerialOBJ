using System;
using System.Text.Json;

namespace binstarjs03.AerialOBJ.Core.Definitions;

public class KindNotFoundException : JsonException
{
	public KindNotFoundException() { }
	public KindNotFoundException(string message) : base(message) { }
	public KindNotFoundException(string message, Exception inner) : base(message, inner) { }
}
