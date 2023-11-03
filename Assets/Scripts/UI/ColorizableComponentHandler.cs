using TMPro;
using UnityEngine;

namespace UI
{
    public class ColorizableComponentHandler : MonoBehaviour
    {
        public void SetGreen(Component c)
        {
            Colorize(c, Color.green);
        }

        public void SetWhite(Component c)
        {
            Colorize(c, Color.white);
        }

        public void SetRed(Component c)
        {
            Colorize(c, Color.red);
        }

        private void Colorize(Component c, Color col)
        {
            switch (c.GetType().Name)
            {
                case "TextMeshProUGUI" : ((TextMeshProUGUI)c).color = col;
                    break;
                case "Outline" : ((UnityEngine.UI.Outline)c).effectColor = col;
                    break;
                default : Debug.LogError("Error : Unknown type \"" + c.GetType().Name + "\"");
                    break;
            }
        }
    }
}
