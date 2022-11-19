using System.IO;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;

public class LevelDatNotFoundException : IOException
{
	public LevelDatNotFoundException() { }
	public LevelDatNotFoundException(string message) : base(message) { }
	public LevelDatNotFoundException(string message, System.Exception inner) : base(message, inner) { }
}

public class RegionFolderNotFoundException : IOException
{
	public RegionFolderNotFoundException() { }
	public RegionFolderNotFoundException(string message) : base(message) { }
	public RegionFolderNotFoundException(string message, System.Exception inner) : base(message, inner) { }
}

