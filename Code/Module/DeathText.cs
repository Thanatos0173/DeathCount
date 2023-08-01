using System;
using Microsoft.Xna.Framework;
using Monocle;
using Microsoft.Xna.Framework.Input;
using static MonoMod.InlineRT.MonoModRule;

namespace Celeste.Mod.DeathCount
{
    class DeathText : Entity
    {
        public DeathText()
        {
            Tag = Tags.HUD;
        }
        public override void Render()
        {

            if (Main.Settings.DeathCountMethodNumber == 0 && Main.Settings.IsEnabled) { 
                DrawText("Death Count : " + Main.Deaths, Main.Settings.XPosition, Main.Settings.YPosition);

            }
            if (Main.Settings.DeathCountMethodNumber == 1 && Main.Settings.IsEnabled)
            {
                SaveData data = SaveData.Instance;
                DrawText("Total Deaths : " + data.TotalDeaths.ToString(), Main.Settings.XPosition, Main.Settings.YPosition);
            }
        }
        public Color GenerateRgb(string backgroundColor)
        {
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(backgroundColor);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return new Color(r,g,b);
        }

        private void DrawText(String text, int x, int y)
        {
            Color color = new Color(255f, 255f, 255f);
            ActiveFont.DrawOutline(text,position: new Vector2(x, y),justify: new Vector2(0f, 0f),
                                scale: new Vector2(Main.Settings.Scale, Main.Settings.Scale),color: GenerateRgb("#"+Main.Settings.TextColor), stroke: 2f,strokeColor: GenerateRgb("#"+Main.Settings.Stroke)) ;
        }
    }
}