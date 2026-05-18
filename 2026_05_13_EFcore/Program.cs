using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

await RunAsync();

static async Task RunAsync()
{
    using var db = new AppDbContext();
    await db.Database.EnsureCreatedAsync();

    Console.WriteLine("EF Core Console Demo - Projects & TodoItems");

    bool exit = false;
    while (!exit)
    {
        Console.WriteLine();
        Console.WriteLine("1) Liste Projekte");
        Console.WriteLine("2) Neues Projekt anlegen");
        Console.WriteLine("3) Liste Todos");
        Console.WriteLine("4) Neues Todo anlegen");
        Console.WriteLine("5) Todo umschalten (done/undone)");
        Console.WriteLine("6) Todo löschen");
        Console.WriteLine("7) Sample-Daten anlegen");
        Console.WriteLine("0) Beenden");
        Console.Write("Auswahl: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await ListProjects(db);
                break;
            case "2":
                await CreateProject(db);
                break;
            case "3":
                await ListTodos(db);
                break;
            case "4":
                await CreateTodo(db);
                break;
            case "5":
                await ToggleTodo(db);
                break;
            case "6":
                await DeleteTodo(db);
                break;
            case "7":
                await SeedSample(db);
                break;
            case "0":
                exit = true;
                break;
            default:
                Console.WriteLine("Ungültige Auswahl");
                break;
        }
    }
}

static async Task ListProjects(AppDbContext db)
{
    var projects = await db.Projects.Include(p => p.TodoItems).ToListAsync();
    if (!projects.Any())
    {
        Console.WriteLine("Keine Projekte vorhanden.");
        return;
    }
    foreach (var p in projects)
    {
        Console.WriteLine($"[{p.Id}] {p.Name} - Todos: {p.TodoItems.Count}");
    }
}

static async Task CreateProject(AppDbContext db)
{
    Console.Write("Projektname: ");
    var name = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name darf nicht leer sein.");
        return;
    }
    var p = new Project { Name = name.Trim() };
    db.Projects.Add(p);
    await db.SaveChangesAsync();
    Console.WriteLine($"Projekt angelegt mit Id {p.Id}");
}

static async Task ListTodos(AppDbContext db)
{
    var todos = await db.TodoItems.Include(t => t.Project).ToListAsync();
    if (!todos.Any())
    {
        Console.WriteLine("Keine Todos vorhanden.");
        return;
    }
    foreach (var t in todos)
    {
        Console.WriteLine(
            $"[{t.Id}] {(t.IsDone ? "x" : " ")} {t.Title} (Projekt: {t.Project?.Name ?? "-"})"
        );
    }
}

static async Task CreateTodo(AppDbContext db)
{
    Console.Write("Todo Titel: ");
    var title = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Titel darf nicht leer sein.");
        return;
    }
    var projects = await db.Projects.ToListAsync();
    int? projectId = null;
    if (projects.Any())
    {
        Console.WriteLine("Projekt auswählen (leer = kein Projekt):");
        foreach (var p in projects)
            Console.WriteLine($"[{p.Id}] {p.Name}");
        Console.Write("ProjektId: ");
        var pid = Console.ReadLine();
        if (int.TryParse(pid, out var pidv))
            projectId = pidv;
    }
    var todo = new TodoItem
    {
        Title = title.Trim(),
        IsDone = false,
        ProjectId = projectId,
    };
    db.TodoItems.Add(todo);
    await db.SaveChangesAsync();
    Console.WriteLine($"Todo angelegt mit Id {todo.Id}");
}

static async Task ToggleTodo(AppDbContext db)
{
    Console.Write("Todo Id: ");
    var s = Console.ReadLine();
    if (!int.TryParse(s, out var id))
    {
        Console.WriteLine("Ungültige Id");
        return;
    }
    var todo = await db.TodoItems.FindAsync(id);
    if (todo == null)
    {
        Console.WriteLine("Todo nicht gefunden");
        return;
    }
    todo.IsDone = !todo.IsDone;
    await db.SaveChangesAsync();
    Console.WriteLine($"Todo {id} ist jetzt {(todo.IsDone ? "fertig" : "offen")}.");
}

static async Task DeleteTodo(AppDbContext db)
{
    Console.Write("Todo Id zu löschen: ");
    var s = Console.ReadLine();
    if (!int.TryParse(s, out var id))
    {
        Console.WriteLine("Ungültige Id");
        return;
    }
    var todo = await db.TodoItems.FindAsync(id);
    if (todo == null)
    {
        Console.WriteLine("Todo nicht gefunden");
        return;
    }
    db.TodoItems.Remove(todo);
    await db.SaveChangesAsync();
    Console.WriteLine($"Todo {id} gelöscht.");
}

static async Task SeedSample(AppDbContext db)
{
    if (await db.Projects.AnyAsync() || await db.TodoItems.AnyAsync())
    {
        Console.WriteLine("Daten bereits vorhanden. Seed übersprungen.");
        return;
    }
    var p1 = new Project { Name = "Website" };
    var p2 = new Project { Name = "Hausaufgaben" };
    db.Projects.AddRange(p1, p2);
    await db.SaveChangesAsync();

    var t1 = new TodoItem { Title = "Startseite erstellen", ProjectId = p1.Id };
    var t2 = new TodoItem { Title = "Kontaktformular", ProjectId = p1.Id };
    var t3 = new TodoItem { Title = "Mathe: Übungsblatt", ProjectId = p2.Id };
    db.TodoItems.AddRange(t1, t2, t3);
    await db.SaveChangesAsync();
    Console.WriteLine("Sample-Daten angelegt.");
}

// Domain-Modelle und DbContext
public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<TodoItem> TodoItems { get; set; } = new();
}

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsDone { get; set; }
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // SQLite DB in project folder
        optionsBuilder.UseSqlite("Data Source=app.db");
    }
}
