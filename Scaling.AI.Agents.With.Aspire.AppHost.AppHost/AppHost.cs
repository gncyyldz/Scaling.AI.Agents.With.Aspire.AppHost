var builder = DistributedApplication.CreateBuilder(args);

var redisCache = builder.AddRedis("redisCache");
var postgreDB = builder.AddPostgres("postgreDB")
                       .WithImage("postgres", "18")
                       .AddDatabase("mydb");

var rabbitMQMessaging = builder.AddRabbitMQ("rabbitMQMessaging")
                               .WithImage("rabbitmq", "latest");

var myAPI = builder.AddProject<Projects.MyAPI>("myAPI")
                   .WithReference(redisCache)
                   .WithReference(postgreDB)
                   .WithReference(rabbitMQMessaging)
                   .WithExternalHttpEndpoints();

var aiService = builder.AddPythonApp("aiService", "../ai-service", "main.py")
                       .WithReference(postgreDB)
                       .WithReference(rabbitMQMessaging)
                       .WithExternalHttpEndpoints();

var myFrontend = builder.AddJavaScriptApp("myFrontend", "../MyFrontend")
                   .WithRunScript("start")
                   .WithReference(myAPI)
                   .WithReference(aiService.GetEndpoint("http"))
                   .WithExternalHttpEndpoints();

builder.Build().Run();
