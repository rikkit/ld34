using Microsoft.Xna.Framework;

namespace LD34
{
    public static class ColourEx
    {
        public static Color WithAlpha(this Color c, float a)
        {
            return new Color(c, a);
        }
    }
}