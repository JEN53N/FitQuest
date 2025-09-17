using System.Text.Json;

namespace FitnessApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:8000"; // http: //10.0.2.2:8000 --nospace nd in quotes replace it thats it

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5); // Long timeout for video upload http: //10.0.2.2:8000 --nospace nd in quotes replace it thats it
        }

        public async Task<AnalysisResult> AnalyzePushupsAsync(string videoPath)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Read video file
                var videoBytes = await File.ReadAllBytesAsync(videoPath);
                var videoContent = new ByteArrayContent(videoBytes);
                videoContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4");

                content.Add(videoContent, "video", Path.GetFileName(videoPath));

                // Send POST request to analysis endpoint
                var response = await _httpClient.PostAsync($"{BaseUrl}/analyze", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse>(jsonResult);

                    return new AnalysisResult
                    {
                        Success = true,
                        PushupCount = result?.pushup_count ?? 0
                    };
                }
                else
                {
                    return new AnalysisResult
                    {
                        Success = false,
                        ErrorMessage = $"Server error: {response.StatusCode}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                return new AnalysisResult
                {
                    Success = false,
                    ErrorMessage = $"Network error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new AnalysisResult
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }

    public class AnalysisResult
    {
        public bool Success { get; set; }
        public int PushupCount { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ApiResponse
    {
        public int pushup_count { get; set; }
        public string status { get; set; } = string.Empty;
    }
}