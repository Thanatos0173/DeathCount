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

            if (Main.Settings.DeathCountMethodNumber == 0) { 
                DrawText("Death Count : " + Main.Deaths, Main.Settings.XPosition, Main.Settings.YPosition);

            }
            if (Main.Settings.DeathCountMethodNumber == 1)
            {
                SaveData data = SaveData.Instance;
                DrawText("Death Count : " + data.TotalDeaths.ToString(), Main.Settings.XPosition, Main.Settings.YPosition);
            //Caca
            }
        }

        private void DrawText(String text, int x, int y)
        {
            ActiveFont.DrawOutline(text,position: new Vector2(x, y),justify: new Vector2(0f, 0f),
                                scale: new Vector2(0.5f, 0.5f),color: Color.White, stroke: 2f,strokeColor: Color.Black) ;
        }
    }
}