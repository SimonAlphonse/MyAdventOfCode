namespace ChatCompletionService.Models
{
    public record ChatCompletionRequest(string model, decimal temperature, Message[] messages);

    public record Message(string role, string content);

    public class Model
    {
        public const string GPT_3_5_TURBO = "gpt-3.5-turbo";
    }

    public enum Role
    {
        user,
        assistant,
    }
}