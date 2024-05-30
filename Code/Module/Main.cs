using System;
using System.Data;
using Npgsql;
using System.Net.NetworkInformation;
using System.Linq;
using Monocle;
using SDL2;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Diagnostics.Metrics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.MiaInfoGetter
{
    public class Main : EverestModule
    {

        public static Main Instance;

        public Main()
        {
            Instance = this;
        }

        int pid = -1;

        static string ip = "158.178.196.26";
        static string db_name = "postgres";
        static string user = "postgres";
        static string pwd = "b6bfHDvAEwGkpVmrwPFbVD9898VJt5";

        static List<string> l = new List<string>();
        public override void Load()
        {
            Logger.Log("Mia_InfoGetter", "Loaded");
            On.Celeste.Level.LoadLevel += LoadLevel;
            On.Celeste.Player.Update += PlayerUpdate;
            Everest.Events.Level.OnExit += ExitLevel;
            On.Celeste.Celeste.OnExiting += Exiting;

        }

        public override void Unload()
        {
            Logger.Log("Mia_InfoGetter", "Unloaded");
            On.Celeste.Level.LoadLevel -= LoadLevel;
            On.Celeste.Player.Update -= PlayerUpdate;
            Everest.Events.Level.OnExit -= ExitLevel;
            On.Celeste.Celeste.OnExiting -= Exiting;

        }
        static bool doesUpdate = false;


        [Command("level", "Select level of the player")]
        public static void LevelCommand(string level)
        {
            if (!doesUpdate)
            {
                var macAddr = (
                            from nic in NetworkInterface.GetAllNetworkInterfaces()
                            where nic.OperationalStatus == OperationalStatus.Up
                            select nic.GetPhysicalAddress().ToString()
                        ).FirstOrDefault();

                if (macAddr == null)
                {
                    Console.WriteLine("No MAC address found.");
                    return;
                }

                var hashedMacAddr = BitConverter.ToUInt32(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(macAddr.ToLower())), 0);
                var connectionString = $"Host={ip};Username={user};Password={pwd};Database={db_name}";
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    NpgsqlCommand checkCommand = connection.CreateCommand();
                    Utils.Utils.ExecuteSQLCommand(checkCommand, $"INSERT INTO Levels VALUES ((SELECT pid FROM Users WHERE ip_address = {hashedMacAddr}),{Int32.Parse(level)})");
                    int result = Convert.ToInt32(checkCommand.ExecuteNonQuery());

                };
                doesUpdate = true;

            }
        }


        private void Exiting(On.Celeste.Celeste.orig_OnExiting orig, Celeste self, object sender, EventArgs args)
        {
            var connectionString = $"Host={ip};Username={user};Password={pwd};Database={db_name}";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                foreach (string s in l)
                {
                    NpgsqlCommand checkCommand = connection.CreateCommand();
                    Utils.Utils.ExecuteSQLCommand(checkCommand, s);
                    checkCommand.ExecuteNonQuery();

                }

            };
            orig(self, sender, args);
        }

        private void ExitLevel(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow)
        {
            var connectionString = $"Host={ip};Username={user};Password={pwd};Database={db_name}";
            using (var connection = new NpgsqlConnection(connectionString))
            {

                connection.Open();


                foreach (string s in l)
                {
                    NpgsqlCommand checkCommand = connection.CreateCommand();
                    Utils.Utils.ExecuteSQLCommand(checkCommand, s);
                    checkCommand.ExecuteNonQuery();

                }

            };

        }

        private void LoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);
            var macAddr = (
                            from nic in NetworkInterface.GetAllNetworkInterfaces()
                            where nic.OperationalStatus == OperationalStatus.Up
                            select nic.GetPhysicalAddress().ToString()
                        ).FirstOrDefault();

            if (macAddr == null)
            {
                Console.WriteLine("No MAC address found.");
                return;
            }

            var hashedMacAddr = BitConverter.ToUInt32(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(macAddr.ToLower())), 0);
            Console.WriteLine(hashedMacAddr);
            
            var connectionString = $"Host={ip};Username={user};Password={pwd};Database={db_name}";
            if (isFromLoader && self.Session.Area.SID == "mia3")
            {
                Task.Run(async () =>
                {
                    try
                    {

                        var dataSource = NpgsqlDataSource.Create(connectionString);

                        var checkCommand = dataSource.CreateCommand($"SELECT COUNT(*) FROM Users WHERE ip_address = {hashedMacAddr}");
                        var result = await checkCommand.ExecuteScalarAsync();

                        if (result != null && Convert.ToInt32(result) == 0)
                        {
                            pid = Convert.ToInt32(await dataSource.CreateCommand("SELECT COALESCE(MAX(pid)+1,0) FROM Users").ExecuteScalarAsync());
                            var insertCommand = dataSource.CreateCommand($"INSERT INTO Users VALUES ({pid},{hashedMacAddr})");
                            await insertCommand.ExecuteNonQueryAsync();
                        }
                        else
                        {
                            var pidCommand = dataSource.CreateCommand($"SELECT pid FROM Users WHERE ip_address = {hashedMacAddr}");
                            var pidResult = await pidCommand.ExecuteScalarAsync();
                            pid = Convert.ToInt32(pidResult);
                            Console.WriteLine($"The MAC address is already saved as pid {pid}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred when loading the level: {ex.ToString()}");
                    }
                });

            }
            using (var connection = new NpgsqlConnection(connectionString))
            {
                if (isFromLoader)
                {
                    connection.Open();
                    NpgsqlCommand checkCommand = connection.CreateCommand();
                    Utils.Utils.ExecuteSQLCommand(checkCommand, "SELECT COUNT(*) FROM(SELECT Users.ip_address, Levels.level FROM Levels JOIN Users ON Users.pid = Levels.id)");
                    int result = Convert.ToInt32(checkCommand.ExecuteScalar());
                    doesUpdate = (result != 0);
                    if (result == 0 && isFromLoader && self.Session.Area.SID == "mia3" && !doesUpdate)
                    {
                        Textbox textbox = new Textbox("LEVEL_ALERT_1");
                        self.Add(textbox);
                    }
                }

            };


        }

        private void PlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            if (doesUpdate)
            {
                orig(self);
                if (Engine.Scene is Level level && pid != -1)
                {
                    
                    var position = Utils.Utils.ConvertToString(TileManager.TileManager.FusedArrays(level, level.SolidsData.ToArray(), self));


                    Utils.Utils.SavePosition(position, l);
                    Utils.Utils.SaveKeypress(position, self, pid, l);
                    Utils.Utils.SaveDeaths(position, self, pid, l);


                }
            }
        }

    }
}
