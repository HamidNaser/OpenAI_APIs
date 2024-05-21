using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace OpenAI_APIs;

public interface IOpenAiService
{
    public Task<string> TranscribeAudioAsync(byte[] audioBytes);
    public Task<string> GenerateSummaryFromInstructions(string instructions);
    public Task<string> GenerateAnswerForQuestionAsync(string question);
    public Task<string> TranscribeImageAsync(string base64Image);
}
