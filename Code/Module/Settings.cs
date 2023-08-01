using Celeste.Mod.UI;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace Celeste.Mod.DeathCount
{
    public class SettingsClass : EverestModuleSettings
    {
        [SettingInGame(true)]
        public int XPosition { get; set; } = 10;
        public int YPosition { get; set; } = 10;
        public int DeathCountMethodNumber { get; set; } = 0;
        public int FastPositionNumber { get; set; } = 1;
        public bool DoFastPosition { get; set; } = true;
        public string TextColor { get; set; } = "FFFFFF";
        public string Stroke { get; set; } = "000000";

        public float Scale { get; set; } = 0.5f;


        private static readonly string[] DeathCountMethod = new string[]
        {
            "Start when spawning",
            "Global"
        };

        private static readonly string[] FastPosition = new string[]
        {
            "Top-Right",
            "Top-Left",
            "Bottom-Right",
            "Bottom-Left"
        };

        private static List<TextMenu.Option<int>> optionList1 = new List<TextMenu.Option<int>>();
        private static List<TextMenu.Option<int>> optionList2 = new List<TextMenu.Option<int>>();

        


        private void Edit(bool value,TextMenu menu)
        {
            DoFastPosition = value;
            foreach(TextMenu.Option<int> option in optionList1)
            {
                option.Disabled = !DoFastPosition;
            }
            foreach (TextMenu.Option<int> option in optionList2)
            {
                option.Disabled = DoFastPosition;
            }

        }

        private string IntString(int value)
        {
            return value.ToString();
        }

        private string Foo(int value)// I had no idea for the name
        {
            return DeathCountMethod[value];
        }

        private string Foo2(int value)
        {
            return FastPosition[value];
        }

        private void FastPositionPlacement(int value)
        {
//            "Top-Right",
//            "Top-Left",
//            "Bottom-Right",
//            "Bottom-Left"
            FastPositionNumber = value;
            if (FastPosition[FastPositionNumber] == "Top-Right")
            {
                XPosition = 1913 - (int)ActiveFont.Measure("Death Count : " + Main.Deaths).X;
                YPosition = 0;
            }
            else if (FastPosition[FastPositionNumber] == "Bottom-Left") {
                XPosition = 0;
                YPosition = 1068 - (int)ActiveFont.Measure("Death Count : " + Main.Deaths).Y;
            }
            else if (FastPosition[FastPositionNumber] == "Bottom-Right")
            {
                XPosition = 1913 - (int)ActiveFont.Measure("Death Count : " + Main.Deaths).X;
                YPosition = 1068 - (int)ActiveFont.Measure("Death Count : " + Main.Deaths).Y;
            }
            else
            {
                XPosition = 0;
                YPosition = 0;
            }
        }

        
        public void CreateXPositionEntry(TextMenu menu, bool inGame)
        {
            TextMenu.Option<int> option = new TextMenu.Slider("X Position", IntString, 0, 1913 - (int)ActiveFont.Measure("Death Count : " + Main.Deaths).X, XPosition)
                 .Change(newValue => XPosition = newValue);
            option.Disabled = !DoFastPosition;
            menu.Add(option);
            optionList1.Add(option);
                                  
        }
        public void CreateYPositionEntry(TextMenu menu, bool inGame)
        {
            TextMenu.Option<int> option = new TextMenu.Slider("Y Position", IntString, 0, 1068 - (int)ActiveFont.Measure("Death Count : " + Main.Deaths).Y, YPosition)
                 .Change(newValue => YPosition = newValue);
            option.Disabled = !DoFastPosition;
            menu.Add(option);
            optionList1.Add(option);

        }

        public void CreateScaleEntry(TextMenu menu, bool inGame)
        {
            TextMenu.Option<int> option = new TextMenu.Slider("Scale", IntString, 0, 10, (int)Scale/10)
                 .Change(newValue => Scale = (newValue)/10f);
            menu.Add(option);
         
        }
        public void CreateFastPositionNumberEntry(TextMenu menu, bool inGame)
        {
            TextMenu.Option<int> option = new TextMenu.Slider("Fast Position : ", Foo2, 0, FastPosition.Length - 1, FastPositionNumber)
                .Change(FastPositionPlacement);
            menu.Add(
                option

            );
            option.Disabled = DoFastPosition;
            optionList2.Add(option);
        }
        public void CreateDeathCountMethodNumberEntry(TextMenu menu, bool inGame)
        {
            menu.Add(
                new TextMenu.Slider("Death Count Method : ",Foo,0, DeathCountMethod.Length - 1,DeathCountMethodNumber)
                .Change(newValue => DeathCountMethodNumber = newValue)           
            );
        }
        
        public void CreateDoFastPositionEntry(TextMenu menu, bool inGame)
        {
            menu.Add(new TextMenu.OnOff("Use Fast Position For Placing : ", DoFastPosition)
                .Change(newValue => Edit(newValue,menu)));  
        }

        public void CreateTextColorEntry(TextMenu menu, bool inGame)
        {
            TextMenu.Item textMenu =new TextMenu.Button("Text Color: #"+TextColor)
                .Pressed(() => {
                 menu.SceneAs<Overworld>().Goto<OuiModOptionString>()
                        .Init<OuiModOptions>("#" + TextColor,
                            value => TextColor =value.ToUpper().Replace("#", ""), 6);
                });
            textMenu.Disabled = inGame;
            menu.Add(textMenu);

        }
        public void CreateStrokeEntry(TextMenu menu, bool inGame)
        {
            TextMenu.Item textMenu = new TextMenu.Button("Stroke Color: #"+Stroke)
                .Pressed(() => {
                    menu.SceneAs<Overworld>().Goto<OuiModOptionString>()
                           .Init<OuiModOptions>("#" + Stroke,
                            value => Stroke = value.ToUpper().Replace("#",""), 6);
                });
            textMenu.Disabled = inGame;
            menu.Add(textMenu);

        }
    }
}

