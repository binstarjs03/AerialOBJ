using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.WpfAppNew.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components;
public class ChunkRegionViewport : Viewport
{
    public event Action? HeightLimitChanged;

    private int _heightLimit = 319;
    private List<RegionModel> _regionModels = new();

    public int HeightLimit
    {
        get => _heightLimit;
        set
        {
            if (value != _heightLimit)
            {
                _heightLimit = value;
                HeightLimitChanged?.Invoke();
            }
        }
    }

    public ChunkRegionViewport() : base() { }

    protected override void Update()
    {
        //throw new NotImplementedException();
    }
}
