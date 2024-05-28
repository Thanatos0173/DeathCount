using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void SavePosition(NpgsqlCommand checkCommand, string position)
        {
            ExecuteSQLCommand(checkCommand, $"SELECT COALESCE((SELECT id FROM Positions WHERE position = '{position}' LIMIT 1),-1);");
            // Execute the command and get the result
            var pos_id = checkCommand.ExecuteScalar();
            if (Convert.ToInt32(pos_id) == -1) // The current position does not exist
            {
                ExecuteSQLCommand(checkCommand, "SELECT COUNT(*) FROM Positions");
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                ExecuteSQLCommand(checkCommand, $"INSERT INTO Positions VALUES ({count},'{position}')");
                checkCommand.ExecuteNonQuery();
            }
        }
        public static void SaveDeaths(NpgsqlCommand checkCommand, string position, Player self, int pid)
        {
            if (self.Dead)
            {
                double percentage = 100 * (self.Position.X / 3672);
                ExecuteSQLCommand(checkCommand, $"SELECT id FROM Positions WHERE position = '{position}' LIMIT 1;");
                int posID = Convert.ToInt32(checkCommand.ExecuteScalar());
                ExecuteSQLCommand(checkCommand, $"SELECT COALESCE((SELECT occurences FROM Deaths WHERE (posid = {posID} AND pid = {pid} AND progression = {percentage})),0)  LIMIT 1;");
                int result = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (result == 0) { ExecuteSQLCommand(checkCommand, $"INSERT INTO Deaths VALUES ({pid}, {posID}, {percentage}, {1})"); checkCommand.ExecuteNonQuery(); }
                else { ExecuteSQLCommand(checkCommand, $"UPDATE Deaths SET occurences = {result} + 1 WHERE (posid = {posID} AND pid = {pid} AND progression = {percentage})"); checkCommand.ExecuteNonQuery(); }
            }
        }
        public static void SaveKeypress(NpgsqlCommand checkCommand, string position, Player self, int pid)
        {
            if (self.Position != self.PreviousPosition)
            {
                ExecuteSQLCommand(checkCommand, $"SELECT id FROM Positions WHERE position = '{position}'  LIMIT 1;");
                int posID = Convert.ToInt32(checkCommand.ExecuteScalar());
                ExecuteSQLCommand(checkCommand, $"SELECT id FROM Moves WHERE move = '{GetInputs()}'  LIMIT 1;");
                int moveID = Convert.ToInt32(checkCommand.ExecuteScalar());
                ExecuteSQLCommand(checkCommand, $"SELECT COALESCE((SELECT occurence FROM MovesSaving WHERE (moveid = {moveID} AND positionid = {posID} AND userid = {pid}) LIMIT 1),0);");
                int result = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (result == 0) { ExecuteSQLCommand(checkCommand, $"INSERT INTO MovesSaving VALUES ({moveID}, {posID}, {pid}, {1})"); checkCommand.ExecuteNonQuery(); }
                else { ExecuteSQLCommand(checkCommand, $"UPDATE MovesSaving SET occurence = {result} + 1 WHERE (moveid = {moveID} AND positionid = {posID} AND userid = {pid})"); checkCommand.ExecuteNonQuery(); }
            }
        }
    }
}
