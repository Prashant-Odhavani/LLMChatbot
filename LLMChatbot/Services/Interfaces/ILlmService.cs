namespace LLMChatbot.Services.Interfaces
{
    public interface ILlmService
    {
        Task<float[]> GetEmbeddingAsync(string query);
        Task<string> SaveDocumentAsync(string fullText);
        Task<string> AskQuestionAsync(string scopeId, string question);
    }
}
