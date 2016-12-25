using System.Collections.Generic;
using System.Windows.Forms;

namespace SnakeGame
{
    class Input
    {
        private static Dictionary<Keys, bool> keys = new Dictionary<Keys, bool>();

        public static void ChangeState(Keys key, bool state)
        {
            keys[key] = state;
        }

        public static bool Pressed(Keys key)
        {
            bool value = false;
            keys.TryGetValue(key, out value);
            return value;
        }
    }
}
