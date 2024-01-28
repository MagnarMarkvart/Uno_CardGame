// Uno CardGame Console Application

using DAL;
using Microsoft.EntityFrameworkCore;
using UI.ConsoleUI.ConsoleMenuSystem;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Title = ">> U N O <<";

// Database Start
var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared";
connectionString = connectionString.Replace("<%temppath%>", Path.GetTempPath());


var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;
using var db = new AppDbContext(contextOptions);

// apply all the migrations
db.Database.Migrate();
// Database End

// Interchangeable Saving Methods - JSON FILE AND DB

// IGameRepository gameRepository = new GameRepositoryFileSystem();
IGameRepository gameRepository = new GameRepositoryEf(db);

// Load MenuSystem
while (true)
{
    var menuLoader = new MenuLoader(gameRepository);
    menuLoader.Run();
}
