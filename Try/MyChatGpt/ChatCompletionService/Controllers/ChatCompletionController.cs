using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using ChatCompletionService.Models;

namespace ChatCompletionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatCompletionController : ControllerBase
    {
        private readonly ILogger<ChatCompletionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private decimal _temperature;

        public ChatCompletionController(ILogger<ChatCompletionController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _temperature = Convert.ToDecimal(_configuration.GetSection("ChatCompletion:temperature").Value);
            _httpClient = httpClientFactory.CreateClient("OpenAi");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.GetSection(@"ChatCompletion:API_KEY").Value);
        }

        [HttpPost(Name = "[action]")]
        public async Task<IActionResult> SendMessage(string content, CancellationToken token)
        {
            ChatCompletionRequest request = new ChatCompletionRequest(
                Model.GPT_3_5_TURBO, _temperature, new[] { new Message(Role.user.ToString(), content.ToString()) });

            var payload = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var responseMessage = await _httpClient.PostAsync("chat/completions", payload, token);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseBody = responseMessage.Content.ReadAsStringAsync().Result;
                var response = JsonSerializer.Deserialize<ChatCompletionResponse>(responseBody);
                var choice = response!.choices.Last();
                if (choice.finish_reason == "stop")
                {
                    return Ok(choice.message.content);
                }
            }

            return NotFound(string.Empty);
        }
    }
}