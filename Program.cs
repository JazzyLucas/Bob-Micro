var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/message", async (HttpClient httpClient) =>
{
    var response = await httpClient.GetStringAsync("http://localhost:5000/api/receive");
    return Results.Json(new { Message = $"Received response: {response}" });
}).WithName("GetMessage");

app.MapGet("/", () => Results.Content("""
    <!DOCTYPE html>
    <html>
    <head>
        <title>Success</title>
        <script>
            async function callApi() {
                const response = await fetch('/api/message');
                const data = await response.json();
                document.getElementById('result').innerText = data.Message;
            }
        </script>
    </head>
    <body>
        <h1>Success!</h1>
        <button onclick="window.location.href='/swagger'">Go to Swagger UI</button>
        <button onclick="callApi()">Call Bob-Micro API</button>
        <p id="result"></p>
    </body>
    </html>
    """, "text/html"))
   .WithName("GetRoot");

app.Run();