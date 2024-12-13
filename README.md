# compression-for-db
The point of this proof-of-concept was to replicate the SQL compress method for saving to the database, doing this in the back-end
code to avoid additional overhead in the database, and then being able to decompress using the basic SQL Decompress method.

The following SQL Compress method has been replicated in C#:
<pre><code>INSERT INTO Player ([Name], [Surname], [Info])     
VALUES ('John', 'Smith', <b>COMPRESS</b>(N'{"sport":"Tennis","age": 28,"rank":1,"points":15258, turn":17}'))
</pre></code>
Therefore, after data has been inserted into the database, the same data can easily be decompressed in SQL Server Management Studio
using the Decompress method:
<pre><code>SELECT [Id]
  ,[Name]
  ,[Surname]
  ,[Info]
  ,CAST(<b>DECOMPRESS</b>(Info) AS NVARCHAR(MAX)) AS  AfterCastingDecompression
FROM Player
WHERE Id = 9
</pre></code>
Note, `varbinary(max)` is being used as the datatype for the compressed data in the data table.
