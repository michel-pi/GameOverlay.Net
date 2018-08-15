namespace GameOverlay.Graphics
{
    /// <summary>
    /// </summary>
    public interface IShape
    {
        /// <summary>
        ///     Draws the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        void Draw(D2DDevice device);
    }
}