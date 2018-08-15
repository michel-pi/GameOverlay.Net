using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GameOverlay.Graphics.Containers
{
    /// <summary>
    /// </summary>
    /// <seealso cref="T:GameOverlay.Graphics.IShapeContainer" />
    public class ConcurrentShapeContainer : ConcurrentBag<IShape>, IShapeContainer
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GameOverlay.Graphics.Containers.ConcurrentShapeContainer" /> class.
        /// </summary>
        public ConcurrentShapeContainer()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GameOverlay.Graphics.Containers.ConcurrentShapeContainer" /> class.
        /// </summary>
        /// <param name="collection">
        ///     The collection whose elements are copied to the new
        ///     <see cref="T:System.Collections.Concurrent.ConcurrentBag`1" />.
        /// </param>
        public ConcurrentShapeContainer(IEnumerable<IShape> collection) : base(collection)
        {
        }
    }
}