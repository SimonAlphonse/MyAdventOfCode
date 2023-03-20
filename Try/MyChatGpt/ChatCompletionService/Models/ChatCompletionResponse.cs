using System.Reflection;

namespace ChatCompletionService.Models
{
    public record ChatCompletionResponse(string id, Choice[] choices, string model, string @object, Usage usage, double created);
    public record Choice(Message message, string finish_reason, int index);
    public record Usage(int prompt_tokens, int completion_tokens, int total_tokens);
}