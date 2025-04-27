using LLMChatbot.Models;
using LLMChatbot.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LLMChatbot.Services;

public class LlmService : ILlmService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMongoCollection<BsonDocument> _collection;

    public LlmService(IOptions<MongoDbOptions> config, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        var client = new MongoClient(config.Value.ConnectionString);
        var db = client.GetDatabase(config.Value.Database);
        _collection = db.GetCollection<BsonDocument>("documentChunks");
    }

    public async Task<float[]> GetEmbeddingAsync(string query)
    {
        var client = _httpClientFactory.CreateClient("LLMClient");

        var payload = new
        {
            model = "nomic-embed-text",
            prompt = query
        };

        var response = await client.PostAsJsonAsync("embeddings", payload);
        var json = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(json);
        var vector = doc.RootElement.GetProperty("embedding").EnumerateArray()
            .Select(x => x.GetSingle()).ToArray();

        return vector;
    }

    public async Task<string> AskQuestionAsync(string question)
    {
        var relevantChunks = await SearchRelevantChunksAsync(question);

        if (!relevantChunks.Any())
            return "Sorry, I could not find any relevant information.";

        var context = string.Join("\n\n", relevantChunks);

        var answer = await GetLlama3AnswerAsync(context, question);

        return answer;
    }

    private async Task<string> GetLlama3AnswerAsync(string context, string question)
    {
        var client = _httpClientFactory.CreateClient("LLMClient");

        var prompt = $"Answer the question based on the following context:\n\n{context}\n\nQuestion: {question}";

        var request = new
        {
            model = "llama3", // or use mistral, gemma, etc.
            prompt = prompt,
            stream = false
        };

        var response = await client.PostAsJsonAsync("generate", request);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("response").GetString() ?? "No answer generated.";
    }

    public async Task IndexDocumentAsync(string fullText)
    {
        var chunks = ChunkText(fullText);
        foreach (var chunk in chunks)
        {
            var embedding = await GetEmbeddingAsync(chunk);
            var doc = new BsonDocument
            {
                { "content", chunk },
                { "embedding", new BsonArray(embedding) }
            };
            await _collection.InsertOneAsync(doc);
        }
    }

    private async Task<List<string>> SearchRelevantChunksAsync(string query, int topK = 3)
    {
        var embedding = await GetEmbeddingAsync(query);

        var pipeline = new[]
        {
            new BsonDocument("$vectorSearch", new BsonDocument
            {
                { "index", "llm-vector-searh-index" },
                { "path", "embedding" },
                { "queryVector", new BsonArray(embedding) },
                { "numCandidates", 100 },
                { "limit", topK },
                { "similarity", "cosine" }
            }),
            new BsonDocument("$project", new BsonDocument
            {
                { "content", 1 },
                { "score", new BsonDocument("$meta", "vectorSearchScore") },
                { "_id", 0 }
            })
        };
        var result = await _collection.AggregateAsync<BsonDocument>(pipeline);
        return result.ToList().Select(d => d["content"].AsString).ToList();
    }

    private static List<string> ChunkText(string text, int chunkSize = 500)
    {
        return Regex.Matches(text, $"(.|\n){{1,{chunkSize}}}").Select(m => m.Value.Trim()).ToList();
    }

}
