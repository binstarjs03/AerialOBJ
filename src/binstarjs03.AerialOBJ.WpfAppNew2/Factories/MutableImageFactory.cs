﻿using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew2.Components;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Factories;
public class MutableImageFactory : IMutableImageFactory
{
    /// <exception cref="TaskCanceledException"></exception>
    public IMutableImage Create(Size<int> size, CancellationToken cancellationToken)
    {
        if (App.Current.CheckAccess())
            return new MutableImage(size);
        else
        {
            // TODO Inconsistency with RegionLoaderService, this one is throwing but RegionLoaderService is returning null
            MutableImage? result = App.Current.Dispatcher.Invoke(() => new MutableImage(size), DispatcherPriority.Background, cancellationToken);
            if (result is null)
                throw new TaskCanceledException();
            return result;
        }
    }
}