namespace LLMChatbot.Services.Interfaces
{
    public interface ILlmService
    {
        Task<float[]> GetEmbeddingAsync(string query);
        Task IndexDocumentAsync(string fullText);
        Task<string> AskQuestionAsync(string scopeId, string question);
    }
}
