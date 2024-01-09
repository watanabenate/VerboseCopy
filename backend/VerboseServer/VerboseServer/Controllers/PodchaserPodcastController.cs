using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GraphQL;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Net.Http.Headers;
using GraphQL.Client.Http;
using Newtonsoft.Json;
using Polly;
using VerboseServer.Data;
using VerboseServer.Models;
using Microsoft.Extensions.Options;

namespace VerboseServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodchaserPodcastController : ControllerBase
    {
        private static String PodchaserEndpoint = "https://api.podchaser.com/graphql";
        private readonly PodchaserToken PodchaserToken;
        private String token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiI5NjBlYTIyNy1jMzljLTQ2MzgtYTRmOC1hOGM1Y2E0ODU0ZTYiLCJqdGkiOiJlNjk0NDE4MjBhYWJkNmEwYzRmNmY1NTVhMTgwZTBkZTYyNDMzZWU3OTIzY2VkNzEwNjk0OTRmN2IwYzk3OGYzYzRkMmQ5NjdjNmQ5ZTA5MCIsImlhdCI6MTY0OTg5NjI2Ny45MTg5NTksIm5iZiI6MTY0OTg5NjI2Ny45MTg5NjQsImV4cCI6MTY4MTQzMjI2Ny45MDk2MjEsInN1YiI6IiIsInNjb3BlcyI6WyIqIl19.W4ZRKypD0LAqmV2hJlVIefL18Nazfj9l08-WxcCorNa6kjSgyPmZGbv26qSmSNkfi6SdQoUcZVNoM5Fwq6lAqpdpb8hFC2mGWXsug0dn-0v15YxjF17oJfDYNCJgjgoduqvZHk7Kn8CEEIUptytKzQEjkk80Bg5A5sI-bjOAuJoM39E_ZLdbZTbVlUHhddbgYlE9GvdFsJJ5WOZNGLQfKQGLx7L-l4sHgExdRcvVTSD_KFmfhJERr7h-unCTWP92czvi9N8N5e6d36opYHM5Tr-eRITd6nO3b9T6Ot3xeQd9RbNukQVStXieUbPt7KiahvfNeFq07eOvVUkPi5S0RsMKXYQlVM9Zri6-jADr3akIVRYjv8W55LnC8CMpMfn4G-PU46M3rGl9l1d85OpJOVoWj9XGr7hPrqNB9NRFXbZN1nzeI65u_8OmYNdGYAK-tMkOk_Ydop-SLktXs4YWNJnLdBgnJE5qEo-CRSTDksYP2WXbc-3d5eaEq1QCz0VrCTRr3ieGAuJte5NkArhLPVZmDRFi6G2bNZMN2S8MeG7Bx_vIpeWtmlgeibPsm7YtS1sNY0nC4mU0jMASaFWJu9oxxFy1_0pSkNKIh4PnGub_x6jECgxz3ekSI6pR1ljrW-8c_Aygy0fKEJ1BDnW-UYNGACH1MXQAjOCLmwlUpYA"; 
        private static readonly Lazy<GraphQLHttpClient> _clientHolder = new Lazy<GraphQLHttpClient>(CreateGraphQLClient);
        private static GraphQLHttpClient Client => _clientHolder.Value;
        private readonly VerboseContext _context;
        public PodchaserPodcastController(IOptions<PodchaserToken> podchaserToken, VerboseContext context)
        {
            PodchaserToken = podchaserToken.Value ?? throw new ArgumentException(nameof(podchaserToken));
            _context = context;
        }


        /**
         * This endpoint is called when a user searches for a podcast. 
         */
        [HttpPost]
        public async Task<IActionResult> GetPodcastList([FromBody] GetPodcastListBody body)
        {
            string searchTerm = body.SearchTerm;

            var graphQlRequest = new GraphQLRequest()
            {
                Query = "query{ podcasts(searchTerm: \"" + searchTerm + "\", , first: 5) { data{ id, title, description,imageUrl,author{ name},categories {title},episodes{data{ id, title, description,imageUrl, audioUrl}}}}}"
            };

            Client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string json = (await AttemptAndRetry(() => Client.SendQueryAsync<dynamic>(graphQlRequest)).ConfigureAwait(false)).ToString();
            Rootobject obj = JsonConvert.DeserializeObject<Rootobject>(json);
            var result = ToPodcastList(obj);

            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPodcastCategories()
        {
            try
            {
                var ComedyRequest = new GraphQLRequest()
                {
                    Query = "query{ podcasts(filters: {categories: [\"Comedy\"], language: \"en\"}, first: 5 ) { data{ id, title, description,imageUrl,author{ name},categories {title},episodes{ data{ id, title, description,imageUrl, audioUrl} }}}}"
                };
                var TrueCrimeRequest = new GraphQLRequest()
                {
                    Query = "query{ podcasts(filters: {categories: [\"Crime\"], language: \"en\"}, first: 5 ) { data{ id, title, description,imageUrl,author{ name},categories {title},episodes{ data{ id, title, description,imageUrl, audioUrl} }}}}"
                };
                var TVRequest = new GraphQLRequest()
                {
                    Query = "query{ podcasts(filters: {categories: [\"TV\"], language: \"en\"}, first: 5 ) { data{ id, title, description,imageUrl,author{ name},categories {title},episodes{ data{ id, title, description,imageUrl, audioUrl} }}}}"
                };
                var SportsRequest = new GraphQLRequest()
                {
                    Query = "query{ podcasts(filters: {categories: [\"Sports\"], language: \"en\"}, first: 5 ) { data{ id, title, description,imageUrl,author{ name},categories {title},episodes{ data{ id, title, description,imageUrl, audioUrl} }}}}"
                };
                var NewsRequest = new GraphQLRequest()
                {
                    Query = "query{ podcasts(filters: {categories: [\"News\"], language: \"en\"}, first: 5 ) { data{ id, title, description,imageUrl,author{ name},categories {title},episodes{ data{ id, title, description,imageUrl, audioUrl} }}}}"
                };
                Client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string ComedyJson = (await AttemptAndRetry(() => Client.SendQueryAsync<dynamic>(ComedyRequest)).ConfigureAwait(false)).ToString();
                string TrueCrimeJson = (await AttemptAndRetry(() => Client.SendQueryAsync<dynamic>(TrueCrimeRequest)).ConfigureAwait(false)).ToString();
                string TVJson = (await AttemptAndRetry(() => Client.SendQueryAsync<dynamic>(TVRequest)).ConfigureAwait(false)).ToString();
                string SportsJson = (await AttemptAndRetry(() => Client.SendQueryAsync<dynamic>(SportsRequest)).ConfigureAwait(false)).ToString();
                string NewsJson = (await AttemptAndRetry(() => Client.SendQueryAsync<dynamic>(NewsRequest)).ConfigureAwait(false)).ToString();

                Rootobject ComedyObj = JsonConvert.DeserializeObject<Rootobject>(ComedyJson);
                Rootobject TrueCrimeObj = JsonConvert.DeserializeObject<Rootobject>(TrueCrimeJson);
                Rootobject TVObj = JsonConvert.DeserializeObject<Rootobject>(TVJson);
                Rootobject SportsObj = JsonConvert.DeserializeObject<Rootobject>(SportsJson);
                Rootobject NewsObj = JsonConvert.DeserializeObject<Rootobject>(NewsJson);

                Dictionary<String, List<VerboseServer.Models.Podcast>> result = new Dictionary<String, List<VerboseServer.Models.Podcast>>();
                result.Add("Comedy", ToPodcastList(ComedyObj));
                result.Add("TrueCrime", ToPodcastList(TrueCrimeObj));
                result.Add("TV", ToPodcastList(TVObj));
                result.Add("Sports", ToPodcastList(SportsObj));
                result.Add("News", ToPodcastList(NewsObj));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        //[HttpGet]
        //[Route("[action]")]
        //public async Task<ActionResult<Dictionary<string, List<Podcast>>>> GetSeededCategories()
        //{
        //    try
        //    {
               
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}


        /** HELPER METHODS **/

        /**
         * Creates a client with the appropiate authorization headers. 
         * */
        static GraphQLHttpClient CreateGraphQLClient()
        {
            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(PodchaserEndpoint)
            };
            var graphQLClient = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());
            return graphQLClient;
        }

        private List<VerboseServer.Models.Podcast> ToPodcastList(Rootobject o)
        {
            List<VerboseServer.Models.Podcast> podcasts = new List<VerboseServer.Models.Podcast>();
            foreach (Datum podcast in o.podcasts.data)
            {
                List<PodcastCategory> categoryList = new List<PodcastCategory>();
                foreach (Category topic in podcast.categories)
                {
                    categoryList.Add(new PodcastCategory
                    {
                        topic = topic.title
                    });
                }

                VerboseServer.Models.Podcast p = new VerboseServer.Models.Podcast
                {
                    PodcastID = int.Parse(podcast.id),
                    Creator = podcast.author.name,
                    CoverArtLink = podcast.imageUrl,
                    Title = podcast.title,
                    Description = podcast.description,
                    Categories = categoryList,
                    Episodes = AddEpisodes(podcast.episodes, int.Parse(podcast.id), podcast.author.name, podcast.imageUrl)
                };
                podcasts.Add(p);
                VerboseServer.Models.Podcast cast = _context.Podcasts
                    .Where(x => x.PodcastID == p.PodcastID)
                    .FirstOrDefault();
                if (cast == null)
                {
                    _context.Podcasts.Add(p);
                }
            }
            _context.SaveChanges();
            return podcasts;
        }

        /** HELPER METHODS **/

        private static List<Episode> AddEpisodes(Episodes ea, int PodcastID, string creator, string podcastImageUrl)
        {
            List<Episode> EpisodeList = new List<Episode>();
            foreach (Datum1 pe in ea.data)
            {
                string episodeImageURL = pe.imageUrl == "" || pe.imageUrl == null ? podcastImageUrl : pe.imageUrl;

                Episode e = new Episode
                {
                    PodchaserPodcastID = PodcastID,
                    EpisodeID = int.Parse(pe.id),
                    Description = pe.description,
                    Title = pe.title,
                    PlayLink = pe.audioUrl,
                    CoverArtLink = episodeImageURL,
                    Creator = creator,
                };
                EpisodeList.Add(e);
            }
            return EpisodeList;
        }

        /*
        * Refer to: https://github.com/brminnick/DotNetGraphQL/blob/0b95c3cfc6cf30be2d9adc92f092ec56aea5950e/Source/DotNetGraphQL.Mobile/Services/GraphQLService.cs
        */
        static async Task<T> AttemptAndRetry<T>(Func<Task<GraphQLResponse<T>>> action, int numRetries = 2)
        {
            var response = await Policy.Handle<Exception>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action).ConfigureAwait(false);

            if (response.Errors != null && response.Errors.Count() > 1)
                throw new AggregateException(response.Errors.Select(x => new Exception(x.ToString())));
            else if (response.Errors != null && response.Errors.Any())
                throw new Exception(response.Errors.First().ToString());

            return response.Data;
            static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
        }

        public class GetPodcastListBody
        {
            public string SearchTerm { get; set; }
        }

        /** RESPONSE CLASSES **/

        public class Rootobject
        {
            public Podcasts podcasts { get; set; }
        }

        public class Podcasts
        {
            public Datum[] data { get; set; }
        }

        public class Datum
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string imageUrl { get; set; }
            public Author author { get; set; }
            public Category[] categories { get; set; }
            public Episodes episodes { get; set; }
        }

        public class Author
        {
            public string name { get; set; }
        }

        public class Episodes
        {
            public Datum1[] data { get; set; }
        }

        public class Datum1
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string imageUrl { get; set; }
            public string audioUrl { get; set; }
        }

        public class Category
        {
            public string title { get; set; }
        }

    }


}
