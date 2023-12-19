using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;

var visionEndpoint = Environment.GetEnvironmentVariable("VISION_ENDPOINT");
var visionKey = Environment.GetEnvironmentVariable("VISION_KEY");

if (visionEndpoint is null || visionKey is null)
{
    Console.WriteLine("The VISION_ENDPOINT or VISION_KEY environment variables are not set.");
    return;
}

var serviceOptions = new VisionServiceOptions(visionEndpoint, new AzureKeyCredential(visionKey));

using var imageSource = VisionSource.FromFile("sample.jpg");

var analysisOptions = new ImageAnalysisOptions()
{
    Features = ImageAnalysisFeature.Text,
    Language = "en",
};

using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);

// 這行程式會把資料轉成 API 呼叫並且等待結果
var result = await analyzer.AnalyzeAsync();

if (result.Reason == ImageAnalysisResultReason.Analyzed)
{
    if (result.Text != null)
    {
        Console.WriteLine($" Text:");
        foreach (var line in result.Text.Lines)
        {
            string pointsToString = "{" + string.Join(',', line.BoundingPolygon.Select(pointsToString => pointsToString.ToString())) + "}";
            Console.WriteLine($"   Line: '{line.Content}', Bounding polygon {pointsToString}");

            foreach (var word in line.Words)
            {
                pointsToString = "{" + string.Join(',', word.BoundingPolygon.Select(pointsToString => pointsToString.ToString())) + "}";
                Console.WriteLine($"     Word: '{word.Content}', Bounding polygon {pointsToString}, Confidence {word.Confidence:0.0000}");
            }
        }
    }
}