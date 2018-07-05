using System;
using System.Collections.Generic;

namespace GameOverlay.Graphics.Containers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GameOverlay.Graphics.IShapeContainer" />
    public class ShapeContainer : List<IShape>, IShapeContainer
    {
        public ShapeContainer() : base()
        {

        }

        public ShapeContainer(int capacity) : base(capacity)
        {

        }

        public ShapeContainer(IEnumerable<IShape> collection) : base(collection)
        {

        }
    }
}
