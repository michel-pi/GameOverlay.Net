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
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeContainer"/> class.
        /// </summary>
        public ShapeContainer() : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeContainer"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public ShapeContainer(int capacity) : base(capacity)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeContainer"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ShapeContainer(IEnumerable<IShape> collection) : base(collection)
        {

        }
    }
}
