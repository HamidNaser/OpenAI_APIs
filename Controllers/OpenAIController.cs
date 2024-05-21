using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace OpenAI_APIs;


[ApiController]
[Route("[controller]")]
public class OpenAIController : ControllerBase
{
    private readonly IOpenAiService _openAiService;
    private readonly ILogger<OpenAIController> _logger;

    private IConfiguration _configuration;

    public OpenAIController(
        ILogger<OpenAIController> logger, 
        IConfiguration configuration, 
        IOpenAiService openAiService)
    {
        _logger = logger;
        _configuration = configuration;
        _openAiService = openAiService;
    }

    /// <summary>
    /// Generates a summary paragraph from the provided paragraph content.
    /// </summary>
    /// <param name="paragraph">The paragraph content to be summarized.</param>
    /// <returns>Generated summary paragraph as a string.</returns>
    /// <remarks>
    /// <para>
    /// This API endpoint is used to generate a summary paragraph based on the content provided in the request. 
    /// The OpenAI API is utilized for generating a concise summary from the provided paragraph.
    /// </para>
    /// <para>
    /// The request body should contain a string with the paragraph content. The generated summary is based on the provided information.
    /// </para>
    /// <para>
    /// The <paramref name="paragraph"/> parameter represents the content that needs to be summarized and is included in the request body.
    /// </para>
    /// <para>
    /// The generated summary paragraph is returned as a string in the response.
    /// </para>
    /// <para>
    /// <b>Authorization:</b> The endpoint requires authentication. The client making the request must be authorized to access this endpoint.
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// <code>
    /// POST /api/openai/GenerateParagraphSummary
    /// {
    ///   "paragraph": "This is the paragraph that needs to be summarized. It contains detailed information about a specific topic, which will be condensed into a summary."
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    [Route("API/GenerateParagraphSummary")]
    [HttpPost]
    public async Task<string> GenerateParagraphSummary(string paragraph)
    {
        var instructions =
            "Below is a paragraph containing information. " +
            "Please write a summary paragraph that will inform the reader of the important information in the paragraph. " +
            "All of the information in the summary should be factual and relevant. " +
            $"Here is the paragraph content: {paragraph} ";

        return await _openAiService.GenerateSummaryFromInstructions(instructions);
    }


    /// <summary>
    /// Asynchronously generates a summary transcription from multiple audio files using the OpenAI API.
    /// </summary>
    /// <param name="audioFiles">An object containing the audio files to be transcribed.</param>
    /// <returns>A string containing the concatenated transcriptions of the provided audio files.</returns>
    /// <remarks>
    /// <para>
    /// This API endpoint accepts multiple audio files, processes each file to generate a transcription using the OpenAI API,
    /// and concatenates the resulting transcriptions into a single string.
    /// </para>
    /// <para>
    /// Each audio file is read into a memory stream and sent to the OpenAI service for transcription.
    /// Only non-empty transcriptions are appended to the final result.
    /// </para>
    /// <para>
    /// <b>Example Request:</b>
    /// <code>
    /// POST /API/GenerateSummaryFromAudioFiles
    /// Content-Type: multipart/form-data
    /// 
    /// --boundary
    /// Content-Disposition: form-data; name="audioFiles"; filename="audio1.mp3"
    /// Content-Type: audio/mpeg
    /// 
    /// [binary data]
    /// --boundary
    /// Content-Disposition: form-data; name="audioFiles"; filename="audio2.mp3"
    /// Content-Type: audio/mpeg
    /// 
    /// [binary data]
    /// --boundary--
    /// </code>
    /// </para>
    /// </remarks>
    [Route("API/GenerateSummaryForAudioFiles")]
    [HttpPost]
    public async Task<string> GenerateSummaryForAudioFiles([FromForm] MediaFiles audioFiles)
    {
        var audioTranscript = new StringBuilder();

        foreach (var file in audioFiles._mediaFiles)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var result = await _openAiService.TranscribeAudioAsync(stream.ToArray());

                if (!string.IsNullOrEmpty(result))
                {
                    audioTranscript.AppendLine(result);
                }
            }
        }

        return audioTranscript.ToString();
    }

    /// <summary>
    /// Asynchronously generates an answer to a given question using the OpenAI API.
    /// </summary>
    /// <param name="question">The question for which an answer is to be generated.</param>
    /// <returns>A string containing the generated answer.</returns>
    /// <remarks>
    /// <para>
    /// This API endpoint communicates with the OpenAI service to generate an answer for the specified question.
    /// It constructs instructions based on the question, sends them to the OpenAI service, and returns the generated answer as a string.
    /// </para>
    /// <para>
    /// Ensure that the OpenAI service is properly configured and the necessary API key is set before calling this method.
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// <code>
    /// POST /API/GenerateAnswerFromQuestion
    /// {
    ///     "question": "What is the capital of France?"
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    [Route("API/GenerateAnswerForQuestion")]
    [HttpPost]
    public async Task<string> GenerateAnswerForQuestion(string question)
    {
        var instructions = $"Please provide an answer to the question: {question}";

        return await _openAiService.GenerateAnswerForQuestionAsync(instructions);
    }

    /// <summary>
    /// Asynchronously generates answers for questions transcribed from provided audio files using the OpenAI API.
    /// </summary>
    /// <param name="audioFiles">An object containing the audio files to be transcribed and analyzed.</param>
    /// <returns>A list of strings containing the generated answers for each question found in the audio files.</returns>
    /// <remarks>
    /// <para>
    /// This API endpoint accepts multiple audio files, transcribes each file to extract questions using the OpenAI service,
    /// and then generates answers for each transcribed question.
    /// </para>
    /// <para>
    /// Each audio file is read into a memory stream, transcribed to text, and if a question is identified,
    /// an answer is generated using the OpenAI API. The resulting answers are returned as a list of strings.
    /// </para>
    /// <para>
    /// <b>Example Request:</b>
    /// <code>
    /// POST /API/GenerateAnswersForQuestionInAudio
    /// Content-Type: multipart/form-data
    /// 
    /// --boundary
    /// Content-Disposition: form-data; name="audioFiles"; filename="audio1.mp3"
    /// Content-Type: audio/mpeg
    /// 
    /// [binary data]
    /// --boundary
    /// Content-Disposition: form-data; name="audioFiles"; filename="audio2.mp3"
    /// Content-Type: audio/mpeg
    /// 
    /// [binary data]
    /// --boundary--
    /// </code>
    /// </para>
    /// </remarks>
    [Route("API/GenerateAnswerForQuestionInAudio")]
    [HttpPost]
    public async Task<List<string>> GenerateAnswersForQuestionInAudioFiles([FromForm] MediaFiles audioFiles)
    {
        var audioTranscriptAnswers = new List<string>();

        foreach (var file in audioFiles._mediaFiles)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                var question = await _openAiService.TranscribeAudioAsync(stream.ToArray());

                if (!string.IsNullOrEmpty(question))
                {
                    var instructions = $"Please provide an answer to the question: {question}";

                    var answer =  await _openAiService.GenerateAnswerForQuestionAsync(instructions);

                    audioTranscriptAnswers.Add(answer);

                }
            }
        }

        return audioTranscriptAnswers;
    }
}