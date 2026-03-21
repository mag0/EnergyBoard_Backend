using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Infrastructure.Database.Seeders;

public class DatabaseSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IColumnRepository _columnRepository;
    private readonly ICardRepository _cardRepository;

    public DatabaseSeeder(
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IColumnRepository columnRepository,
        ICardRepository cardRepository)
    {
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _columnRepository = columnRepository;
        _cardRepository = cardRepository;
    }

    public async Task SeedAsync()
    {
        var users = await _userRepository.GetAllAsync();
        if (users.Any()) return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");

        var user = new User
        {
            Name = "Martin Guerrero",
            Email = "martin@test.com",
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var projectSeeds = GetProjectSeeds();

        foreach (var projectSeed in projectSeeds)
        {
            var project = new Project
            {
                Title = projectSeed.Title,
                Description = projectSeed.Description,
                UserId = user.Id,
                Position = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _projectRepository.AddAsync(project);

            int columnPosition = 0;

            foreach (var columnSeed in projectSeed.Columns)
            {
                var column = new Column
                {
                    Title = columnSeed.Title,
                    Description = columnSeed.Description,
                    ProjectId = project.Id,
                    Position = columnPosition++,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _columnRepository.AddAsync(column);

                int cardPosition = 0;

                foreach (var cardSeed in columnSeed.Cards)
                {
                    var card = new Card
                    {
                        Title = cardSeed.Title,
                        Description = cardSeed.Description,
                        Deadline = DateTime.UtcNow.AddDays(cardSeed.DeadlineDays),
                        ColumnId = column.Id,
                        Position = cardPosition++,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _cardRepository.AddAsync(card);
                }
            }
        }
    }

    private List<ProjectSeed> GetProjectSeeds()
    {
        return new List<ProjectSeed>
        {
            new()
            {
                Title = "EnergyBoard Development",
                Description = "Desarrollo de una aplicación Kanban estilo Trello.",
                Columns =
                [
                    new ColumnSeed
                    {
                        Title = "Backlog",
                        Description = "Ideas y tareas pendientes",
                        Cards =
                        [
                            new CardSeed("Definir arquitectura Clean Architecture","Organizar capas Domain, Application, Infrastructure",7),
                            new CardSeed("Configurar PostgreSQL","Configurar conexión a base de datos en Render",5),
                            new CardSeed("Implementar repositorios base","Crear interfaces y repositorios genéricos",8)
                        ]
                    },
                    new ColumnSeed
                    {
                        Title = "En desarrollo",
                        Description = "Tareas que se están implementando",
                        Cards =
                        [
                            new CardSeed("Crear endpoint de proyectos","CRUD completo de proyectos",6),
                            new CardSeed("Agregar autenticación JWT","Implementar login y generación de tokens",10)
                        ]
                    },
                    new ColumnSeed
                    {
                        Title = "Testing",
                        Description = "Validación de funcionalidades",
                        Cards =
                        [
                            new CardSeed("Probar endpoints en Swagger","Verificar responses y errores",4),
                            new CardSeed("Testear creación de columnas","Validar relación Project-Column",5)
                        ]
                    }
                ]
            },

            new()
            {
                Title = "Portfolio Website",
                Description = "Rediseño del portfolio personal.",
                Columns =
                [
                    new ColumnSeed
                    {
                        Title = "Ideas",
                        Description = "Conceptos iniciales",
                        Cards =
                        [
                            new CardSeed("Definir secciones del sitio","Home, proyectos, contacto",3),
                            new CardSeed("Elegir paleta de colores","Minimalista y moderna",2)
                        ]
                    },
                    new ColumnSeed
                    {
                        Title = "Diseño",
                        Description = "UI/UX",
                        Cards =
                        [
                            new CardSeed("Diseñar layout en Figma","Wireframes principales",5),
                            new CardSeed("Crear diseño responsive","Adaptación mobile y tablet",7)
                        ]
                    },
                    new ColumnSeed
                    {
                        Title = "Implementación",
                        Description = "Desarrollo frontend",
                        Cards =
                        [
                            new CardSeed("Configurar proyecto React","Estructura inicial del proyecto",4),
                            new CardSeed("Crear componente ProjectCard","Mostrar proyectos destacados",6),
                            new CardSeed("Implementar formulario de contacto","Enviar mensajes por email",8)
                        ]
                    }
                ]
            },

            new()
            {
                Title = "Vacation Planning",
                Description = "Organización del viaje de verano.",
                Columns =
                [
                    new ColumnSeed
                    {
                        Title = "Destinos",
                        Description = "Lugares posibles",
                        Cards =
                        [
                            new CardSeed("Investigar Costa del Este","Ver actividades disponibles",3),
                            new CardSeed("Buscar hoteles en Villa Gesell","Comparar precios",4)
                        ]
                    },
                    new ColumnSeed
                    {
                        Title = "Reservas",
                        Description = "Confirmaciones necesarias",
                        Cards =
                        [
                            new CardSeed("Reservar alojamiento","Elegir hotel definitivo",6),
                            new CardSeed("Comprar pasajes","Revisar opciones de transporte",5)
                        ]
                    }
                ]
            },

            new()
            {
                Title = ".NET Backend Study",
                Description = "Práctica de backend y arquitectura.",
                Columns =
                [
                    new ColumnSeed
                    {
                        Title = "Temas",
                        Description = "Conceptos para estudiar",
                        Cards =
                        [
                            new CardSeed("Dependency Injection","Entender contenedor de servicios",3),
                            new CardSeed("Middleware pipeline","Cómo funciona el pipeline HTTP",4),
                            new CardSeed("Logging","Implementar logs estructurados",5)
                        ]
                    },
                    new ColumnSeed
                    {
                        Title = "Práctica",
                        Description = "Ejercicios prácticos",
                        Cards =
                        [
                            new CardSeed("Crear API REST","Endpoints CRUD completos",6),
                            new CardSeed("Implementar autenticación","JWT con refresh tokens",7)
                        ]
                    }
                ]
            },

            new()
            {
                Title = "Personal Tasks",
                Description = "Organización personal semanal.",
                Columns =
                [
                    new ColumnSeed
                    {
                        Title = "Pendiente",
                        Description = "Tareas por hacer",
                        Cards =
                        [
                            new CardSeed("Ir al gimnasio","Entrenamiento de fuerza",2),
                            new CardSeed("Comprar supermercado","Lista de compras semanal",1)
                        ]
                    },
                    new ColumnSeed
                    {
                        Title = "Hoy",
                        Description = "Prioridades del día",
                        Cards =
                        [
                            new CardSeed("Estudiar arquitectura .NET","Revisar Clean Architecture",1),
                            new CardSeed("Avanzar proyecto EnergyBoard","Implementar seeder",2)
                        ]
                    },
                    new ColumnSeed
                    {
                        Title = "Completado",
                        Description = "Tareas finalizadas",
                        Cards =
                        [
                            new CardSeed("Configurar PostgreSQL","Base de datos funcionando",0),
                            new CardSeed("Conectar TablePlus","Acceso a base remota",0)
                        ]
                    }
                ]
            }
        };
    }
}

public record ProjectSeed
{
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public List<ColumnSeed> Columns { get; init; } = [];
}

public record ColumnSeed
{
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public List<CardSeed> Cards { get; init; } = [];
}

public record CardSeed(string Title, string Description, int DeadlineDays);