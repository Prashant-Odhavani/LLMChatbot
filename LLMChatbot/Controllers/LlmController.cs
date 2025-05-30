using LLMChatbot.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LLMChatbot.Controllers
{
    [ApiController]
    [Route("api/llm")]
    public class LlmController : ControllerBase
    {
        private readonly ILlmService _llmService;

        public LlmController(ILlmService llmService)
        {
            _llmService = llmService;
        }

        [HttpPost("documents")]
        public async Task<IActionResult> SaveDocument([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Text cannot be empty.");

            var scopeId = await _llmService.SaveDocumentAsync(text);
            return Ok(scopeId);
        }

        [HttpPost("questions/{scopeId}")]
        public async Task<IActionResult> AskQuestion(string scopeId, [FromBody] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Question cannot be empty.");

            var answer = await _llmService.AskQuestionAsync(scopeId, question);
            return Ok(new { answer });
        }
    }
}
