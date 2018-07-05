using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace GameOverlay.Graphics.Containers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GameOverlay.Graphics.IShapeContainer" />
    public class ConcurrentShapeContainer : ConcurrentBag<IShape>, IShapeContainer
    {
        public ConcurrentShapeContainer() : base()
        {

        }

        public ConcurrentShapeContainer(IEnumerable<IShape> collection) : base(collection)
        {

        }
    }
}
