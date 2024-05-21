namespace OpenAI_APIs.GptRequest;

public class GptRequest
{
    public string model { get; set; }
    public List<GptMessage> messages { get; set; }
    public double temperature { get; set; }
    public int max_tokens { get; set; }
    public double top_p { get; set; }
    public double frequency_penalty { get; set; }
    public double presence_penalty { get; set; }

    public GptRequest()
    {
        messages = new List<GptMessage>();
    }
}

public class GptMessage
{
    public string role { get; set; }
    public string content { get; set; }
}


public class GptRequestImage : GptRequest
{
    public GptRequestImage() : base() 
    { 

    }

    public ImageFile image { get; set; }
}

public class ImageFile
{
    public string image { get; set; }
    public string mime_type { get; set; }
}

