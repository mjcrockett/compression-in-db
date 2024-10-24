using System.Data;
using System.Data.OleDb;

var helper = new Helper();
var compressed = helper.InsertQuery();
Console.WriteLine("The id of the inserted row: " + compressed.Id);
Console.WriteLine("The compressed data: " + compressed.Value);

var uncompressed = helper.ReadQuery(compressed.Id);
Console.WriteLine("The uncompressed data:" + uncompressed.Info);

helper.DeleteQuery(compressed.Id);
Console.WriteLine("Row deleted.");

public class Helper
{
    private readonly string _connectionString = "Provider=sqloledb;Data Source=localhost\\SQLEXPRESS01;Initial Catalog=MCrockett; Trusted_Connection=yes;";

    private readonly string _insertQuery =
        @"INSERT INTO Player([Name],[Surname],[Info]) 
        OUTPUT INSERTED.Id, inserted.Info
        VALUES(?,?,COMPRESS(?))";

    private readonly string _selectQuery =
        @"SELECT [Id],[Name],[Surname],CAST(DECOMPRESS(Info) AS NVARCHAR(MAX)) AS AfterCastingDecompression 
        FROM [MCrockett].[dbo].[Player]
        WHERE [Id] = ?";

    private readonly string _deleteQuery =
        @"DELETE Player
        WHERE [Id] = ?";

    public Helper() { }
    public CompressedData InsertQuery()
    {
        using OleDbConnection connection = new(_connectionString);

        OleDbCommand cmd = new OleDbCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = _insertQuery;

        cmd.Parameters.Add(new OleDbParameter("@name", "MichaelTest"));
        cmd.Parameters.Add(new OleDbParameter("@surname", "CrockettTest"));
        cmd.Parameters.Add(new OleDbParameter("@info", "{\"sport\":\"Basketball\",\"age\": 45,\"rank\":1,\"points\":15258, turn\":17}"));

        cmd.Connection = connection;

        connection.Open();

        using OleDbDataReader reader = cmd.ExecuteReader();

        var compressedData = new CompressedData();
        while (reader.Read())
        {
            compressedData.Id = reader.GetInt32(0);
            var foo = System.Text.Encoding.UTF8.GetString((byte[])reader["Info"]);
            compressedData.Value = foo;
        }
        return compressedData;
    }

    public UncompressedData ReadQuery(int id)
    {
        using OleDbConnection connection = new(_connectionString);

        OleDbCommand cmd = new OleDbCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = _selectQuery;

        cmd.Parameters.Add(new OleDbParameter("@id", id));
        cmd.Connection = connection;

        connection.Open();

        using OleDbDataReader reader = cmd.ExecuteReader();
        var player = new UncompressedData();
        while (reader.Read())
        {
            player.Id = reader.GetInt32(0);
            player.Name = reader.GetString(1);
            player.Surname = reader.GetString(2);
            player.Info = reader.GetString(3);
        }
        return player;
    }

    public void DeleteQuery(int id)
    {
        using OleDbConnection connection = new(_connectionString);

        OleDbCommand cmd = new OleDbCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = _deleteQuery;

        cmd.Parameters.Add(new OleDbParameter("@id", id));
        cmd.Connection = connection;

        connection.Open();

        cmd.ExecuteNonQuery();
    }
}

public class UncompressedData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Info { get; set; }
}

public class CompressedData
{
    public int Id { get; set; }
    public string Value { get; set; }
}

