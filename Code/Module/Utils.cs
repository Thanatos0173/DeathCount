using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Celeste.Mod.MiaInfoGetter.Utils
{
    internal class Utils
    {
        public static string GetInputs()
        {
            int[] inputs = new int[7];
            
            if (Input.MoveX.Value == -1) inputs[0] = 1;
            if (Input.MoveX.Value == 1) inputs[1] = 1;
            if (Input.MoveY.Value == 1) inputs[2] = 1;
            if (Input.MoveY.Value == -1) inputs[3] = 1;
            if (Input.Grab.Check) inputs[4] = 1;
            if (Input.DashPressed) inputs[5] = 1;
            if (Input.Jump.Check) inputs[6] = 1;
            return $"{inputs[0]}{inputs[1]}{inputs[2]}{inputs[3]}{inputs[4]}{inputs[5]}{inputs[6]}";

        }

        public static bool IsConnectedToInternet()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable() &&
                       NetworkInterface.GetAllNetworkInterfaces()
                           .Where(n => n.OperationalStatus == OperationalStatus.Up)
                           .Any(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                     n.NetworkInterfaceType != NetworkInterfaceType.Tunnel);
            }
            catch
            {
                return false;
            }
        }

        public static string ConvertToString(int[,] array)
        {
            string s = "";
            foreach(int o in array) s += o.ToString();
            return s;

        }
        public static void ExecuteSQLCommand(NpgsqlCommand checkCommand, string command)
        {
            checkCommand.CommandText = command;
        }
        public static void SavePosition(string position, List<string> l)
        {
            l.Add($"DO $$ DECLARE x INT; c INT; BEGIN SELECT COALESCE((SELECT id FROM Positions WHERE position = '{position}' LIMIT 1),-1) INTO x; IF x = -1 THEN SELECT COUNT(x) INTO c FROM Positions; INSERT INTO Positions VALUES (c,'{position}'); END IF; END $$;");


        }
        public static void SaveDeaths(string position, Player self, int pid, List<string> l)
        {
            if (self.Dead)
            {
                l.Add($"DO $$ DECLARE percentage FLOAT := 100 * {self.Position.X} / 3672; x INT; y INT; pos INT; BEGIN CREATE TEMP TABLE temporary AS SELECT d.pid, d.progression, d.occurences, p.position, d.posid FROM deaths as d JOIN positions as p ON d.posid = p.id; SELECT occurences,posid INTO x,pos FROM temporary WHERE pid = {pid} AND position = '{position}' AND progression = percentage; IF x IS NULL THEN y := 0; ELSE y := x; END IF; IF y = 0 THEN INSERT INTO deaths VALUES ({pid}, pos, percentage, 1); ELSE UPDATE deaths SET occurences = y + 1 WHERE posid = pos AND pid = {pid} AND progression = percentage; END IF; END$$;");
            }
        }
        public static void SaveKeypress(string position, Player self, int pid, List<string> l)
        {
            if (self.Position != self.PreviousPosition)
            {
                l.Add($"DO $$ DECLARE keypress VARCHAR(7) := '{GetInputs()}'; currpos VARCHAR(400) := '{position}'; moveidvar INT; posidvar INT; occvar INT; BEGIN SELECT id INTO moveidvar FROM moves WHERE move = keypress; SELECT id INTO posidvar FROM positions WHERE position = currpos; SELECT occurence INTO occvar FROM movessaving WHERE moveid = moveidvar AND positionid = posidvar AND userid = {pid}; IF occvar IS NULL THEN INSERT INTO movessaving VALUES (moveidvar, posidvar, {pid}, 1); ELSE UPDATE movessaving SET occurence = occvar + 1 WHERE moveid = moveidvar AND positionid = posidvar AND userid = {pid} AND occurence = occvar; END IF; END$$;"

);
            }
        }
    }
}
