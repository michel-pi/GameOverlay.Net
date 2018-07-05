using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOverlay.Graphics.Primitives
{
    public class Text : IShape
    {
        public string Content;

        public D2DFont Font;

        public void Draw(D2DDevice device)
        {
            throw new NotImplementedException();
        }
    }
}
