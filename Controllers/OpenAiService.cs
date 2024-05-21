using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace OpenAI_APIs;
/// <summary>
/// Service for interacting with the OpenAI API to generate chat completions.
/// </summary>    
public class OpenAiService : IOpenAiService
{
    private readonly string _openAiApiKey = "sk-proj-oeCRZe9iqh5nQTkTW90aT3BlbkFJt00NXrWfnx5dOwrvn0Fs";
    private readonly string _openAiEngine = "gpt-4";
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAiService"/> class.
    /// </summary>
    public OpenAiService()
    {
        try
        {
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
            if (string.IsNullOrEmpty(openAiApiKey))
            {
                _openAiApiKey = "sk-proj-oeCRZe9iqh5nQTkTW90aT3BlbkFJt00NXrWfnx5dOwrvn0Fs";
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An error occurred during OpenAI service registration.");
        }
    }

    /// <summary>
    /// Asynchronously sends an audio file to the OpenAI API for transcription.
    /// </summary>
    /// <param name="audioBytes">The audio file content as a byte array.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the transcription result as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown when there is an error while sending the HTTP request.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs.</exception>
    public async Task<string> GetAudioTranscriptionAsync(byte[] audioBytes)
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var formData = new MultipartFormDataContent();
            formData.Add(new ByteArrayContent(audioBytes), "file", "audio.mp3");
            formData.Add(new StringContent("whisper-1"), "model");

            var response = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", formData);
            response.EnsureSuccessStatusCode();

            var transcription = await response.Content.ReadAsStringAsync();

            return transcription;
        }
        catch (HttpRequestException httpRequestException)
        {
            Console.WriteLine($"HttpRequestException: {httpRequestException.Message}");
            return $"HttpRequestException: {httpRequestException.Message}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return $"Exception: {ex.Message}";
        }
    }


    /// <summary>
    /// Asynchronously transcribes audio data using the OpenAI API.
    /// </summary>
    /// <param name="audioBytes">The audio data to be transcribed, represented as a byte array.</param>
    /// <returns>The transcribed text as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown when there is an issue with the HTTP request to the OpenAI API.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during the transcription process.</exception>
    /// <remarks>
    /// <para>
    /// This method communicates with the OpenAI API to transcribe audio data provided in the form of a byte array.
    /// Ensure that the OpenAI API key is correctly set and the audio data is properly formatted before calling this method.
    /// </para>
    /// <para>
    /// The method uses a multipart form data content to send the audio file and the model information to the OpenAI API.
    /// </para>
    /// <para>
    /// If an error occurs during the OpenAI API call, the exception is logged to the console, and the error message is returned as a string.
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// <code>
    /// byte[] audioData = File.ReadAllBytes("path/to/audio.mp3");
    /// string transcription = await TranscribeAudioAsync(audioData);
    /// </code>
    /// </para>
    /// </remarks>
    public async Task<string> TranscribeAudioAsync(byte[] audioBytes)
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var formData = new MultipartFormDataContent();
            formData.Add(new ByteArrayContent(audioBytes), "file", "audio.mp3");
            formData.Add(new StringContent("whisper-1"), "model");

            var response = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", formData);
            response.EnsureSuccessStatusCode();

            var transcription = await response.Content.ReadAsStringAsync();

            return transcription;
        }
        catch (HttpRequestException httpRequestException)
        {
            Console.WriteLine($"HttpRequestException: {httpRequestException.Message}");
            return $"HttpRequestException: {httpRequestException.Message}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return $"Exception: {ex.Message}";
        }
    }


    /// <summary>
    /// Asynchronously generates an answer to a given question using the OpenAI API.
    /// </summary>
    /// <param name="question">The question for which an answer is to be generated.</param>
    /// <returns>A string containing the generated answer.</returns>
    /// <exception cref="HttpRequestException">Thrown when there is an issue with the HTTP request to the OpenAI API.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during the request process.</exception>
    /// <remarks>
    /// <para>
    /// This method communicates with the OpenAI API to generate an answer to the specified question.
    /// It constructs a request with the question formatted as instructions, sends it to the OpenAI API,
    /// and returns the response as a string.
    /// </para>
    /// <para>
    /// Ensure that the OpenAI API key is correctly set before calling this method.
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// <code>
    /// string question = "What is the capital of France?";
    /// string answer = await GenerateAnswerFromQuestionAsync(question);
    /// </code>
    /// </para>
    /// </remarks>
    public async Task<string> GenerateAnswerForQuestionAsync(string instructions)
    {
        try
        {
            var gptRequest = new OpenAI_APIs.GptRequest.GptRequest
            {
                model = _openAiEngine,
                messages = new List<OpenAI_APIs.GptRequest.GptMessage>
            {
                new OpenAI_APIs.GptRequest.GptMessage
                {
                    role = "user",
                    content = instructions
                }
            },
                temperature = 1,
                max_tokens = 1000,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var gptRequestJson = JsonSerializer.Serialize(gptRequest);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _openAiApiKey);
                var content = new StringContent(gptRequestJson, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
                {
                    Content = content
                };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var answer = await response.Content.ReadAsStringAsync();

                var chatCompletion = JsonSerializer.Deserialize<OpenAI_APIs.ChatCompletion.ChatCompletion>(answer);

                return chatCompletion.choices.FirstOrDefault()?.message.content ?? string.Empty;
            }
        }
        catch (HttpRequestException httpRequestException)
        {
            Console.WriteLine($"HttpRequestException: {httpRequestException.Message}");
            throw; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            throw; 
        }
    }

    /// <summary>
    /// Asynchronously generates a summary based on the provided instructions using the OpenAI API.
    /// </summary>
    /// <param name="instructions">Instructions for generating the summary.</param>
    /// <returns>Generated summary as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="_openAiClient"/> is not provided (null).</exception>
    /// <remarks>
    /// <para>
    /// This method communicates with the OpenAI API to generate a summary based on the provided instructions.
    /// Ensure that the OpenAI client is properly configured before calling this method.
    /// </para>
    /// <para>
    /// The method uses the provided <paramref name="instructions"/> to create a chat completion request.
    /// </para>
    /// <para>
    /// If an error occurs during the OpenAI API call, the exception is logged, and it is re-thrown to allow the caller to handle it.
    /// </para>
    /// <para>
    /// <b>Authorization:</b> The endpoint requires authentication. Ensure that the API key is correctly set in the headers.
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// <code>
    /// var summary = await GenerateSummaryFromInstructions("Please summarize the following content: ...");
    /// </code>
    /// </para>
    /// </remarks>
    public async Task<string> GenerateSummaryFromInstructions(string instructions)
    {
        try
        {

            OpenAI_APIs.GptRequest.GptRequest gptRequest = new OpenAI_APIs.GptRequest.GptRequest
            {
                model = _openAiEngine,
                messages = new List<OpenAI_APIs.GptRequest.GptMessage>
            {
                new OpenAI_APIs.GptRequest.GptMessage
                {
                    role = "user",
                    content = instructions
                }
            },
                temperature = 1,
                max_tokens = 1000,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var gptRequestJson = JsonSerializer.Serialize(gptRequest);

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
                request.Headers.Add("Authorization", "Bearer " + _openAiApiKey);
                var content = new StringContent(gptRequestJson, Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var answer = await response.Content.ReadAsStringAsync();

                var chatCompletion = JsonSerializer.Deserialize<OpenAI_APIs.ChatCompletion.ChatCompletion>(answer);

                return chatCompletion.choices.FirstOrDefault()?.message.content ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }


    public async Task<string> TranscribeImageAsync(string base64Image)
    {
        try
        {
            var instructions = "Extract text from image.";

            OpenAI_APIs.GptRequest.GptRequestImage gptRequestImage = new OpenAI_APIs.GptRequest.GptRequestImage
            {
                model = _openAiEngine,
                messages = new List<OpenAI_APIs.GptRequest.GptMessage>
                {
                    new OpenAI_APIs.GptRequest.GptMessage
                    {
                        role = "user",
                        content = instructions
                    }
                },
                temperature = 1,
                max_tokens = 1000,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0,
                image = new OpenAI_APIs.GptRequest.ImageFile
                {
                    image = base64Image,
                    mime_type = "image/png"
                }
            };


            string gptRequestImageJson = JsonSerializer.Serialize(gptRequestImage);

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
                request.Headers.Add("Authorization", "Bearer " + _openAiApiKey);
                var content = new StringContent(gptRequestImageJson, Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var extractedText = await response.Content.ReadAsStringAsync();

                return extractedText;
            }
        }
        catch (HttpRequestException httpRequestException)
        {
            Console.WriteLine($"HttpRequestException: {httpRequestException.Message}");
            return $"HttpRequestException: {httpRequestException.Message}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return $"Exception: {ex.Message}";
        }
    }


}

