using System.IO;

namespace binstarjs03.AerialOBJ.WpfApp.Components;
public class DefaultFileInfo : IFileInfo
{
	private readonly FileInfo _fileInfo;

	public DefaultFileInfo(string path)
	{
		_fileInfo= new FileInfo(path);
	}

    public string Name => _fileInfo.Name;
}
