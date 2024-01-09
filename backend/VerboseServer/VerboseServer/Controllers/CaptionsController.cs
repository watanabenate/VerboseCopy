using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Speech.V1;
using Google.LongRunning;
using System.IO;
using Google.Protobuf;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Protobuf.Collections;

namespace VerboseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptionsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCaptions()
        {
            try
            {
                // key = timestamp, value = text
                var result = new Dictionary<int, string>();

                SpeechClientBuilder clientBuilder = new SpeechClientBuilder();
                clientBuilder.CredentialsPath = "loyal-optics-341119-6d3780c5e3c9.json";
                SpeechClient client = await clientBuilder.BuildAsync();

                RecognitionConfig config = new RecognitionConfig();
                config.Encoding = RecognitionConfig.Types.AudioEncoding.Linear16;
                config.LanguageCode = "en-US";
                config.EnableAutomaticPunctuation = true;
                config.SampleRateHertz = 8000;

                RecognitionAudio audio = new RecognitionAudio();

                audio.Content = ByteString.FromBase64(System.IO.File.ReadAllText("MockData/audio.txt"));

                RecognizeResponse response = await client.RecognizeAsync(config, audio);
                RepeatedField<SpeechRecognitionResult> results = response.Results;

                foreach (SpeechRecognitionResult str in results)
                {
                    // There can be several alternative transcripts for a given chunk of speech. Just use the
                    // first (most likely) one here.
                    SpeechRecognitionAlternative alternative = str.Alternatives[0];
                    Google.Protobuf.WellKnownTypes.Duration endTime = str.ResultEndTime;

                    result.Add((int) endTime.ToTimeSpan().TotalMilliseconds, alternative.Transcript);
                    
                    //Console.WriteLine("Transcription: %s%n", alternative.Transcript);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
