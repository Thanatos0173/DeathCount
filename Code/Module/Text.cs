using System;
using Microsoft.Xna.Framework;
using Monocle;
using Microsoft.Xna.Framework.Input;
using static MonoMod.InlineRT.MonoModRule;
using System.Collections.Generic;
using Celeste.Mod.MiaInfoGetter;

namespace Celeste.Mod.MiaInfoGetter.TextClass
{
    public class Text : Entity
    {
        public Text()
        {
        }
        public string text { get; set; } = "";
       
        public override void Render()
        {
            // Console.WriteLine(Main.Settings.ShowSpawnDeaths + " " + Main.Settings.ShowLevelDeaths + " " + Main.Settings.ShowLevelMinDeaths + " " + Main.Settings.ShowTotalDeaths);
            text = "You are not connected to internet. Please connect.";
            DrawText("Death(s) : " + text, 10, 10);
        }


        public Color GenerateRgb(string backgroundColor)
        {
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(backgroundColor);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return new Color(r, g, b);
        }

        private void DrawText(String text, int x, int y)
        {
            Color color = new Color(255f, 255f, 255f);
            ActiveFont.DrawOutline(text, position: new Vector2(x, y), justify: new Vector2(0f, 0f),
                                scale: new Vector2(0.5f, 0.5f), color: GenerateRgb("#ff0000"), stroke: 2f, strokeColor: GenerateRgb("#000000"));
        }
    }
}