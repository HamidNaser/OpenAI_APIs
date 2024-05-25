# OpenAI APIs

This repository contains an ASP.NET Core API project that utilizes the OpenAI API to generate summaries, transcriptions, and answers from text and audio files.

## Table of Contents

- [OpenAI APIs](#openai-apis)
- [Table of Contents](#table-of-contents)
- [API Endpoints](#api-endpoints)
  - [Generate Paragraph Summary](#generate-paragraph-summary)
  - [Generate Summary for Audio Files](#generate-summary-for-audio-files)
  - [Generate Answer for Question](#generate-answer-for-question)
  - [Generate Answers for Questions in Audio Files](#generate-answers-for-questions-in-audio-files)
- [Running the Application](#running-the-application)

## API Endpoints
![Application Interface](https://github.com/HamidNaser/OpenAI_APIs/blob/main/AllAPIs.png)

### Generate Paragraph Summary

Generates a summary paragraph from the provided paragraph content.

```csharp
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
```
![Application Interface](https://github.com/HamidNaser/OpenAI_APIs/blob/main/GenerateParagraphSummary.png)

### Generate Summary for Audio Files

Generates a summary transcription from multiple audio files.

```csharp
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
```
![Application Interface](https://github.com/HamidNaser/OpenAI_APIs/blob/main/GenerateSummaryForAudioFiles.png)

### Generate Answer for Question

Generates an answer to a given question.

```csharp
[Route("API/GenerateAnswerForQuestion")]
[HttpPost]
public async Task<string> GenerateAnswerForQuestion(string question)
{
    var instructions = $"Please provide an answer to the question: {question}";

    return await _openAiService.GenerateAnswerForQuestionAsync(instructions);
}
```
![Application Interface](https://github.com/HamidNaser/OpenAI_APIs/blob/main/GenerateAnswerForQuestion.png)

### Generate Answers for Questions in Audio Files

Generates answers for questions transcribed from provided audio files.

```csharp
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
```
![Application Interface](https://github.com/HamidNaser/OpenAI_APIs/blob/main/GenerateAnswersForQuestionInAudioFiles.png)

## Running the Application


