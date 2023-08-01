using IL.MonoMod;
using Monocle;
using On.Celeste;
using System;
using System.Windows.Forms;

namespace Celeste.Mod.DeathCount
{
    public class Main : EverestModule
    {
        public override Type SettingsType => typeof(SettingsClass);
        public static SettingsClass Settings => (SettingsClass)Instance._Settings;

        public static Main Instance;

        public static int Deaths = 0;

        public Main()
        {
            Instance = this;
        }

        public override void Load()
        {
            Logger.Log("DeathCound", "Loaded");
            On.Celeste.Level.LoadLevel += AddDeathCount;
            On.Celeste.Player.Update += ModPlayerUpdate;

        }

        public override void Unload()
        {
            Logger.Log("DeathCount", "Unloaded");
            On.Celeste.Level.LoadLevel -= AddDeathCount;
            On.Celeste.Player.Update -= ModPlayerUpdate;
        }

        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            if (self.Dead)
            {
                Deaths++;
            }
        }

        public void AddDeathCount(On.Celeste.Level.orig_LoadLevel orig, Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(level, playerIntro, isFromLoader);

            // only try to add if a player exists
            // prevents crashing when loading a level without a player spawn object
            if(isFromLoader) { Deaths = 0; }
            if (level.Tracker.GetEntity<Player>() != null)
            {

                level.Add(new DeathText());

            }
        }
    }

}
