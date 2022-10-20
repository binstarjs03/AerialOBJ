using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core.WorldRegion;

namespace binstarjs03.AerialOBJ.WpfApp;
public class RegionWrapper
{
    private readonly Region? _region;

    public Region? Region => _region;

    public RegionWrapper(Region? region)
    {
        _region = region;
    }
}
