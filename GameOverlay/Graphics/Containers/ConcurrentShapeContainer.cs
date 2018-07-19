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
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentShapeContainer"/> class.
        /// </summary>
        public ConcurrentShapeContainer() : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentShapeContainer"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.</param>
        public ConcurrentShapeContainer(IEnumerable<IShape> collection) : base(collection)
        {

        }
    }
}
