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
var response = await httpClient.GetStringAsync("http://joemicro:5246/api/receive");
    return Results.Json(new { Message = $"Received from Joe: {response}" });
}).WithName("GetMessage");

// TODO: put this in a proper file
app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html>
<head>
    <title>Bob Micro</title>
    <script>
        async function callJoeMicro() {
            try {
                const response = await fetch('/api/message');
                const data = await response.json();
                document.getElementById('result').innerText = data.Message;
            } catch (error) {
                document.getElementById('result').innerText = `Error: ${error.message}`;
            }
        }
    </script>
</head>
<body>
    <h1>Bob Micro Interface</h1>
    <button onclick="window.location.href='/swagger'">Go to Swagger UI</button>
    <button onclick="callJoeMicro()">Call Joe-Micro</button>
    <p id="result"></p>
</body>
</html>
""", "text/html")).WithName("GetRoot");

app.Run();