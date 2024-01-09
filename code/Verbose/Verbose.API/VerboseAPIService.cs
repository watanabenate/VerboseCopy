using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Verbose.Data;
using System.Net.Http;
using Xamarin.Essentials;
using Android.Media;
using Newtonsoft.Json;
using Android.Graphics;
using Firebase.Auth;
using Android.App;

namespace Verbose.API
{
    public class VerboseAPIService : VerboseHTTPClient
    {
        #region IVerboseAPIService Implementation

        #region Private variables
        private bool _isConnected;
        private bool _isConnectedToServer;

        private Profile _userProfile;

        private List<ListenedTo> _mainFeedRecentlyListenedTo;
        private List<Post> _mainFeedPosts;

        private MediaPlayer _player;
        private bool _isPlaying;
        public PodcastEpisode _currentPlayingEpisode;

        private int _mainFeedIndex;

        private FirebaseAuth _fireAuth;

        private Dictionary<string, Bitmap> linksToBitmaps;

        private Dictionary<string, List<Podcast>> _searchPageRecommendations;

        private PublicProfile _otherUserProfile;
        public bool OtherUserInUserFollowing = false;

        public List<Podcast> searchResults;
        public ListenedTo userListenedTo;
        public Podcast PodcastFromId;

        public List<PublicProfile> friendSearchResults;

        public Dictionary<int, string> captionDict;
        #endregion Private variables

        public bool ReachedEndOfFeed = false;

        #region Constructor
        private static VerboseAPIService _instance;
        public static VerboseAPIService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VerboseAPIService();
                }
                return _instance;
            }
            private set { _instance = value; }
        }

        /// <summary>
        /// Constructor for the API
        /// This will create the HTTP Client so
        /// we can talk to the server
        /// </summary>
        private VerboseAPIService()
        {
            MainFeedIndex = 0;

            if(Client.BaseAddress == null)
            {
                Client.BaseAddress = new Uri(VerboseAPIStrings.ServerBase);
                Client.Timeout = new TimeSpan(0, 0, 6);
            }
            
            if ((Connectivity.NetworkAccess == NetworkAccess.Internet) ||
                (Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet))
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            // Setup the media player
            Player = new MediaPlayer();
            Player.Completion += PlayerFinished;

            _isConnectedToServer = false;

            linksToBitmaps = new Dictionary<string, Bitmap>();
            SearchPageRecommendations = new Dictionary<string, List<Podcast>>();
        }

        /// <summary>
        /// This method will be called whenever the player
        /// finishes playing it's media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayerFinished(object sender, EventArgs e)
        {
            _isPlaying = false;
        }


        /// <summary>
        /// If we lost connection we want to know, so
        /// change the connection here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if ((e.NetworkAccess == NetworkAccess.Internet) ||
                (e.NetworkAccess == NetworkAccess.ConstrainedInternet))
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
        }
        #endregion Constructor

        #region Tasks

        public async Task<bool> GetMainFeedAsync(string username)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("username", username);
            dataToSend.Add("index", MainFeedIndex.ToString());

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var getPostsResult = await Client.PostAsync(VerboseAPIStrings.GetMainFeedEndPoint, content);
                string postsContent = await getPostsResult.Content.ReadAsStringAsync();

                if (MainFeedPosts == null)
                {
                    MainFeedPosts = new List<Post>();
                }

                if (getPostsResult.IsSuccessStatusCode)
                {
                    List<Post> serverPosts = JsonConvert.DeserializeObject<List<Post>>(postsContent);
                    LoadMoreCount = serverPosts.Count;

                    foreach (Post sp in serverPosts)
                    {
                        if (sp != null)
                        {
                            if (sp.Comments == null) { sp.Comments = new List<Comment>(); }
                            if (sp.LikedBy == null) { sp.LikedBy = new HashSet<LikedBy>(); }
                            MainFeedPosts.Add(sp);
                        }
                    }
                    if (serverPosts.Count < 20)
                    {
                        ReachedEndOfFeed = true;
                        MainFeedIndex += serverPosts.Count;
                    }
                    else
                        MainFeedIndex += 20;
                    result = true;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public async Task<bool> GetRecentlyListenedToAsync(string username)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("userName", username);

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var getPodcastsResult = await Client.PostAsync(VerboseAPIStrings.GetRecentlyListenedToEndPoint, content);
                string podcastsContent = await getPodcastsResult.Content.ReadAsStringAsync();

                if (getPodcastsResult.IsSuccessStatusCode)
                {
                    List<ListenedTo> serverListenedTo = JsonConvert.DeserializeObject<List<ListenedTo>>(podcastsContent);
                    List<ListenedTo> mainFeed = new List<ListenedTo>();

                    foreach (ListenedTo lt in serverListenedTo)
                    {
                        if (lt.Episode != null)
                        {
                            if (lt.Episode.Comments == null) { lt.Episode.Comments = new List<Comment>(); }
                            if (lt.Episode.LikedBy == null) { lt.Episode.LikedBy = new HashSet<int>(); }

                            mainFeed.Add(lt);
                        }
                    }

                    MainFeedRecentlyListenedTo = mainFeed;
                    UserProfile.RecentlyListenedTo = mainFeed;

                    result = true;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public async Task<bool> CheckMadeAccountAsync(FirebaseAuth fireAuth)
        {
            this.fireAuth = fireAuth;

            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("email", fireAuth.CurrentUser.Email);

            // Serialize the email
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                // Post it to the server
                var postCheckMadeAccountResult = await Client.PostAsync(VerboseAPIStrings.CheckMadeAccountEndPoint, content);
                string checkMadeAccountContent = await postCheckMadeAccountResult.Content.ReadAsStringAsync();

                // See if it was a success
                if (postCheckMadeAccountResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // The server returned our wonderful profile/public profile info
                    _userProfile = JsonConvert.DeserializeObject<Profile>(checkMadeAccountContent);

                    if (_userProfile.RecentlyListenedTo == null) { _userProfile.RecentlyListenedTo = new List<ListenedTo>(); }
                    if (_userProfile.PublicProfileInfo.Posts == null) { _userProfile.PublicProfileInfo.Posts = new List<Post>(); }
                    if (_userProfile.PublicProfileInfo.Subscribed == null) { _userProfile.PublicProfileInfo.Subscribed = new List<Podcast>(); }
                    if (_userProfile.PublicProfileInfo.Following == null) { _userProfile.PublicProfileInfo.Following = new List<PublicProfile>(); }

                    result = true;
                }
                _isConnectedToServer = true;
            }
            catch (Exception e)
            {
                // There could be a lot of errors here (maybe we didn't parse correctly)
                // but I am just going to assume we couldn't connect to the server here
                _isConnectedToServer = false;
            }

            return result;
        }

        public async Task<bool> UpdateProfilePictureAsync(string link)
        {
            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("publicProfileID", _userProfile.PublicProfileInfo.PublicProfileId.ToString());
            dataToSend.Add("newPhotoLink", link);
            bool result = false; 
            
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");
            try
            {
                // post it to the server 
                var postUpdateProfilePicture = await Client.PostAsync(VerboseAPIStrings.UpdateProfilePictureEndpoint, content);
                if (postUpdateProfilePicture.IsSuccessStatusCode)

                    UserProfile.PublicProfileInfo.PictureLink = link; 

                    result = true; 

            }
            catch(Exception e)
            {

            }
            return result; 
        }

        public async Task<bool> CreateUserAsync(FirebaseAuth fireAuth, string username)
        {
            this.fireAuth = fireAuth;

            bool result = false;

            // Add data to the post request
            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("email", fireAuth.CurrentUser.Email);
            dataToSend.Add("fullName", fireAuth.CurrentUser.DisplayName);
            dataToSend.Add("pictureLink", fireAuth.CurrentUser.PhotoUrl.ToString());
            dataToSend.Add("userName", username);

            // Serialize it
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                // Post it to the server
                var postCreateAccountResult = await Client.PostAsync(VerboseAPIStrings.CreateUserEndPoint, content);
                string createAccountContent = await postCreateAccountResult.Content.ReadAsStringAsync();

                // See if it was a success
                if (postCreateAccountResult.IsSuccessStatusCode)
                {
                    // Read in the public profile
                    Profile p = JsonConvert.DeserializeObject<Profile>(createAccountContent);

                    // Set the user now to this profile
                    _userProfile = p;

                    result = true;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public async Task<bool> CheckUsernameAvailableAsync(string username)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("userName", username);

            // Serialize it
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var checkUserNameResult = await Client.PostAsync(VerboseAPIStrings.CheckUsernameAvailableEndPoint, content);
                string checkUserNameContent = await checkUserNameResult.Content.ReadAsStringAsync();

                // Status code is OK (200), if a username is available.
                if (checkUserNameResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> SubmitComment(Comment comment, PodcastEpisode episode)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("text", comment.Text);
            dataToSend.Add("date", comment.Date.ToString());
            dataToSend.Add("timestamp", comment.Timestamp.ToString());
            dataToSend.Add("podchaserID", episode.EpisodeID.ToString());
            dataToSend.Add("commenter", UserProfile.PublicProfileInfo.PublicProfileId.ToString());

            // Serialize it
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var submitCommentResult = await Client.PostAsync(VerboseAPIStrings.SubmitCommentEndPoint, content);
                string submitCommentContent = await submitCommentResult.Content.ReadAsStringAsync();

                // Status code is OK (200), if a username is available.
                if (submitCommentResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> SubmitComment(Comment comment, Post post)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("text", comment.Text);
            dataToSend.Add("date", comment.Date.ToString());
            dataToSend.Add("post", post.PostID.ToString());
            dataToSend.Add("commenter", UserProfile.PublicProfileInfo.PublicProfileId.ToString());

            // Serialize it
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var submitCommentResult = await Client.PostAsync(VerboseAPIStrings.SubmitCommentEndPoint, content);
                string submitCommentContent = await submitCommentResult.Content.ReadAsStringAsync();

                // Status code is OK (200), if a username is available.
                if (submitCommentResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> LikeOrUnlikePodcastEpisode(PodcastEpisode e, bool likeOrUnlike)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("episodeID", e.EpisodeID.ToString());
            dataToSend.Add("likeOrUnlike", likeOrUnlike.ToString());
            dataToSend.Add("publicProfile", UserProfile.PublicProfileInfo.PublicProfileId.ToString());

            // Serialize it
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var likeOrUnlikeEpisodeResult = await Client.PostAsync(VerboseAPIStrings.LikeOrUnlikeEpisodeEndPoint, content);
                string likeOrUnlikeEpisodeContent = await likeOrUnlikeEpisodeResult.Content.ReadAsStringAsync();

                // Status code is OK (200), if a username is available.
                if (likeOrUnlikeEpisodeResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> LikeOrUnlikePost(Post p, bool likeOrUnlike)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("postID", p.PostID.ToString());
            dataToSend.Add("likeOrUnlike", likeOrUnlike.ToString());
            dataToSend.Add("publicProfile", UserProfile.PublicProfileInfo.PublicProfileId.ToString());

            // Serialize it
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var likeOrUnlikePostResult = await Client.PostAsync(VerboseAPIStrings.LikeOrUnlikePostEndPoint, content);
                string likeOrUnlikePostContent = await likeOrUnlikePostResult.Content.ReadAsStringAsync();

                // Status code is OK (200), if a username is available.
                if (likeOrUnlikePostResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> SubmitPost(Post p)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            if (p.ImageURL != null)
                dataToSend.Add("imageURL", p.ImageURL);
            dataToSend.Add("title", p.Title);
            dataToSend.Add("description", p.Description);
            dataToSend.Add("date", p.Date.ToString());
            if (p.Episode != null)
            {
                dataToSend.Add("podchaserID", p.Episode.EpisodeID.ToString());
            }
                
            dataToSend.Add("posterID", p.ProfileID.ToString());


            // Serialize it
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var submitPostResult = await Client.PostAsync(VerboseAPIStrings.SubmitPostEndPoint, content);
                string submitPostContent = await submitPostResult.Content.ReadAsStringAsync();

                // Status code is OK (200), if a username is available.
                if (submitPostResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> RefreshProfilePicture()
        {
            bool result = false;
            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("publicProfileID", _userProfile.PublicProfileInfo.PublicProfileId.ToString());

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");
            try
            {
                var profResults = await Client.PostAsync(VerboseAPIStrings.GetUserProfilePictureEndpoint, content); 
                string profContent = await profResults.Content.ReadAsStringAsync();
                if (profResults.IsSuccessStatusCode)
                { 
                    _userProfile.PublicProfileInfo.PictureLink = JsonConvert.DeserializeObject<string>(profContent); 
                }
                result = true; 
            }
            catch(Exception ex)
            {

            }
            return result; 
        }

        public async Task<bool> RefreshUserPosts()
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("username", UserProfile.PublicProfileInfo.UserName);
            dataToSend.Add("index", "0");

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var getPostsResult = await Client.PostAsync(VerboseAPIStrings.RefreshUserPostsEndPoint, content);
                string postsContent = await getPostsResult.Content.ReadAsStringAsync();

                if (UserProfile.PublicProfileInfo.Posts == null)
                {
                    UserProfile.PublicProfileInfo.Posts = new List<Post>();
                }

                if (getPostsResult.IsSuccessStatusCode)
                {
                    List<Post> serverPosts = JsonConvert.DeserializeObject<List<Post>>(postsContent);

                    foreach (Post sp in serverPosts)
                    {
                        if (sp != null)
                        {
                            if (sp.Comments == null) { sp.Comments = new List<Comment>(); }
                            if (sp.LikedBy == null) { sp.LikedBy = new HashSet<LikedBy>(); }
                            UserProfile.PublicProfileInfo.Posts.Add(sp);
                        }
                    }
                    result = true;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        /// <summary>
        /// Search for a podcast by title
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public async Task<bool> SearchPodcasts(string searchQuery)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("SearchTerm", searchQuery);

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var getSearchPodcastsResult = await Client.PostAsync(VerboseAPIStrings.SearchPodcastsEndPoint, content);
                string searchPodcastsContent = await getSearchPodcastsResult.Content.ReadAsStringAsync();

                searchResults = new List<Podcast>();

                 if (getSearchPodcastsResult.IsSuccessStatusCode)
                 {
                     List<Podcast> serverPodcasts = JsonConvert.DeserializeObject<List<Podcast>>(searchPodcastsContent);

                     foreach (Podcast sp in serverPodcasts)
                     {
                         if (sp != null)
                         {
                             if (sp.Episodes == null) { sp.Episodes = new List<PodcastEpisode>(); }
                            searchResults.Add(sp);
                         }
                     }
                     result = true;
                 }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        /// <summary>
        /// Search for a user by username
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public async Task<bool> SearchUsers(string searchQuery)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("SearchTerm", searchQuery);

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var getSearchFriendsResults = await Client.PostAsync(VerboseAPIStrings.SearchFriendsEndPoint, content);
                string searchFriendsContent = await getSearchFriendsResults.Content.ReadAsStringAsync();

                friendSearchResults = new List<PublicProfile>();

                if (getSearchFriendsResults.IsSuccessStatusCode)
                {
                    List<PublicProfile> serverUsers = JsonConvert.DeserializeObject<List<PublicProfile>>(searchFriendsContent);

                    foreach (PublicProfile user in serverUsers)
                    {
                        if (user != null && user.IsPublic)
                        {
                            friendSearchResults.Add(user);
                        }
                    }
                    result = true;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public async Task<bool> GetSearchPageRecommendations()
        {
            bool result = false;

            try
            {
                var getRecommendationsPodcastsResult = await Client.GetAsync(VerboseAPIStrings.SearchRecommendationsEndPoint);
                string searchRecommendationsContent = await getRecommendationsPodcastsResult.Content.ReadAsStringAsync();

                 if (getRecommendationsPodcastsResult.IsSuccessStatusCode)
                 {
                    SearchPageRecommendations = JsonConvert.DeserializeObject<Dictionary<string, List<Podcast>>>(searchRecommendationsContent);

                    foreach(List<Podcast> categories in SearchPageRecommendations.Values)
                    {
                        foreach(Podcast podcast in categories)
                        {
                            GetImageBitmap(podcast); // Cache the podcast image
                        }
                    }

                    result = true;
                 }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public async Task<bool> SubmitRssLink(string rssLink)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            dataToSend.Add("RssLink", rssLink);
            dataToSend.Add("Email", UserProfile.Email);
            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var submitRssLinkResult = await Client.PostAsync(VerboseAPIStrings.SubmitRssLinkEndPoint, content);
                string submitRssLinkContent = await submitRssLinkResult.Content.ReadAsStringAsync();

                if (submitRssLinkResult.IsSuccessStatusCode)
                {
                    UserProfile.IsCreator = true;
                    result = true;
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

        public async Task<bool> UpdateRecentlyListenedTo(PodcastEpisode e, long timestamp)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();

            dataToSend.Add("ProfileID", UserProfile.ProfileID.ToString());
            dataToSend.Add("EpisodeID", e.EpisodeID.ToString());
            dataToSend.Add("Timestamp", timestamp.ToString());

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var updateRecentlyListenedToResult = await Client.PostAsync(VerboseAPIStrings.UpdateRecentlyListenedToEndPoint, content);
                string updateRecentlyListenedToContent = await updateRecentlyListenedToResult.Content.ReadAsStringAsync();

                if (updateRecentlyListenedToResult.IsSuccessStatusCode)
                {
                    result = true;
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

            
        public async Task<bool> GetOtherUserPublicProfile(string username)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();

            dataToSend.Add("username", username);

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var getOtherUserResult = await Client.PostAsync(VerboseAPIStrings.GetOtherUserPublicProfileEndPoint, content);
                string getOtherUserContent = await getOtherUserResult.Content.ReadAsStringAsync();

                if (getOtherUserResult.IsSuccessStatusCode)
                {
                    OtherUserProfile = JsonConvert.DeserializeObject<PublicProfile>(getOtherUserContent);
                    if(OtherUserProfile.Posts == null) { OtherUserProfile.Posts = new List<Post>(); }
                    if (OtherUserProfile.Following == null) { OtherUserProfile.Following = new List<PublicProfile>(); }
                    if (OtherUserProfile.Subscribed == null) { OtherUserProfile.Subscribed = new List<Podcast>(); }

                    if(UserProfile.PublicProfileInfo.Following.Contains(OtherUserProfile))
                    {
                        OtherUserInUserFollowing = true;
                    }
                    else
                    {
                        OtherUserInUserFollowing = false;
                    }

                    result = true;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public async Task<bool> GetUserListenedTo(PodcastEpisode e)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();

            dataToSend.Add("ProfileID", UserProfile.ProfileID.ToString());
            dataToSend.Add("EpisodeID", e.EpisodeID.ToString());

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var getUserListenedToResult = await Client.PostAsync(VerboseAPIStrings.GetUserListenedToEndPoint, content);
                string getUserListenedToContent = await getUserListenedToResult.Content.ReadAsStringAsync();

                if (getUserListenedToResult.IsSuccessStatusCode)
                {
                    userListenedTo = JsonConvert.DeserializeObject<ListenedTo>(getUserListenedToContent);
                }

                result = true;
            }

            catch (Exception)
            {

            }
            return result;
        }
        public async Task<bool> SubOrUnsubPodcast(bool subscribe, Podcast podcast)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();
            
            dataToSend.Add("username", UserProfile.PublicProfileInfo.UserName);
            dataToSend.Add("podcastID", podcast.PodcastID.ToString());
            dataToSend.Add("subOrUnsub", subscribe.ToString());

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var subUnsubPodcastResult = await Client.PostAsync(VerboseAPIStrings.SubscribeOrUnsubscribePodcastEndPoint, content);
                string subUnsubPodcastContent = await subUnsubPodcastResult.Content.ReadAsStringAsync();

                if (subUnsubPodcastResult.IsSuccessStatusCode)
                {
                    if(subscribe)
                    {
                        UserProfile.PublicProfileInfo.Subscribed.Add(podcast);
                    }
                    else
                    {
                        UserProfile.PublicProfileInfo.Subscribed.Remove(podcast);
                    }

                    result = true;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public async Task<bool> FollowOrUnfollowProfile(PublicProfile otherUser, bool follow)
        {
            bool result = false;

            var dataToSend = new Dictionary<string, string>();

            dataToSend.Add("followerID", UserProfile.PublicProfileInfo.PublicProfileId.ToString());
            dataToSend.Add("followeeID", otherUser.PublicProfileId.ToString());

            var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var followUnfollowUserResult = await Client.PostAsync(VerboseAPIStrings.FollowOrUnfollowPublicProfileEndPoint, content);
                string followUnfollowUserContent = await followUnfollowUserResult.Content.ReadAsStringAsync();

                if (followUnfollowUserResult.IsSuccessStatusCode)
                {
                    if (follow)
                    {
                        UserProfile.PublicProfileInfo.Following.Add(otherUser);
                    }
                    else
                    {
                        UserProfile.PublicProfileInfo.Following.Remove(otherUser);
                    }

                    result = true;
                }
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public async Task<bool> GetPodcastFromId(int id)
        {
            bool result = false;

            try
            {
                var getPodcastResult = await Client.GetAsync(VerboseAPIStrings.GetPodcastsEndPoint + id.ToString());
                string getPodcastContent = await getPodcastResult.Content.ReadAsStringAsync();

                if (getPodcastResult.IsSuccessStatusCode)
                {
                    PodcastFromId = JsonConvert.DeserializeObject<Podcast>(getPodcastContent);
                }

                result = true;
            }

            catch (Exception exc)
            {

            }
            return result;
        }

        public async Task<bool> SetPrivacy(int id)
        {
            bool result = false;
            try
            {
                var dataToSend = new Dictionary<string, string>();
                dataToSend.Add("PublicProfileID", UserProfile.PublicProfileInfo.PublicProfileId.ToString());

                var content = new StringContent(JsonConvert.SerializeObject(dataToSend), System.Text.Encoding.UTF8, "application/json");

                var getPrivacyResult = await Client.PostAsync(VerboseAPIStrings.SetPrivacyEndpoint, content);

                if (getPrivacyResult.IsSuccessStatusCode)
                {
                    UserProfile.PublicProfileInfo.IsPublic = !UserProfile.PublicProfileInfo.IsPublic;
                    result = true;
                }

            }
            catch (Exception exc)
            {

            }

            return result;
        }

        public async Task<bool> GetClosedCaptions()
        {
            bool result = false;

            try
            {
                var getCaptionResult = await Client.GetAsync(VerboseAPIStrings.GetClosedCaptionsEndPoint);
                string getCaptionContent = await getCaptionResult.Content.ReadAsStringAsync();

                if (getCaptionResult.IsSuccessStatusCode)
                {
                    captionDict = JsonConvert.DeserializeObject<Dictionary<int, string>>(getCaptionContent);
                }

                result = true;
            }

            catch (Exception exc)
            {

            }
            return result;
        }

        #endregion Tasks

        #region Properties
        public int LoadMoreCount { get; private set; }
        
        public Dictionary<string, List<Podcast>> SearchPageRecommendations
        {
            get { return _searchPageRecommendations; }
            private set
            {
                if(_searchPageRecommendations != value)
                {
                    _searchPageRecommendations = value;
                    NotifyPropertyChanged("SearchPageRecommendations");
                }
            }
        }

        public int MainFeedIndex
        {
            get { return _mainFeedIndex; }
            set
            {
                if (_mainFeedIndex != value)
                {
                    _mainFeedIndex = value;
                    NotifyPropertyChanged("MainFeedIndex");
                }
            }
        }

        public List<Post> MainFeedPosts
        {
            get { return _mainFeedPosts; }
            set
            {
                if (_mainFeedPosts != value)
                {
                    _mainFeedPosts = value;
                    NotifyPropertyChanged("MainFeedPosts");
                }
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    NotifyPropertyChanged("IsConnected");
                }
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    NotifyPropertyChanged("IsPlaying");
                }
            }
        }

        public Profile UserProfile
        {
            get { return _userProfile; }
            set
            {
                if (_userProfile != value)
                {
                    _userProfile = value;
                    NotifyPropertyChanged("UserProfile");
                }
            }
        }

        public PublicProfile OtherUserProfile
        {
            get { return _otherUserProfile; }
            set
            {
                if (_otherUserProfile != value)
                {
                    _otherUserProfile = value;
                    NotifyPropertyChanged("OtherUserProfile");
                }
            }
        }

        public List<ListenedTo> MainFeedRecentlyListenedTo
        {
            get { return _mainFeedRecentlyListenedTo; }
            private set
            {
                if (_mainFeedRecentlyListenedTo != value)
                {
                    _mainFeedRecentlyListenedTo = value;
                    NotifyPropertyChanged("MainFeedPodcasts");
                }
            }
        }

        public MediaPlayer Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public string CurrentPlayingMediaURL
        {
            get { return CurrentPlayingEpisode != null ? CurrentPlayingEpisode.PlayLink : ""; }
        }

        public PodcastEpisode CurrentPlayingEpisode
        {
            get { return _currentPlayingEpisode; }
            set
            {
                _currentPlayingEpisode = value;
            }
        }

        public bool IsConnectedToServer
        {
            get { return _isConnectedToServer; }
            set
            {
                if(_isConnectedToServer != value)
                {
                    _isConnectedToServer = value;
                    NotifyPropertyChanged("IsConnectedToServer");
                }
            }
        }

        public FirebaseAuth fireAuth
        {
            get { return _fireAuth; }
            set { _fireAuth = value; }
        }
        #endregion Properties

        #endregion

        #region Helper Methods
        public Bitmap GetImageBitmap(PodcastEpisode p)
        {
            try
            {
                if (p.CoverArtImage != null) { return p.CoverArtImage; }
                else if (linksToBitmaps.ContainsKey(p.CoverArtLink))
                {
                    p.CoverArtImage = linksToBitmaps[p.CoverArtLink];
                    return p.CoverArtImage;
                }

                Bitmap imageBitmap = null;

                using (var webClient = new System.Net.WebClient())
                {
                    var imageBytes = webClient.DownloadData(p.CoverArtLink);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }

                p.CoverArtImage = imageBitmap;
                linksToBitmaps.Add(p.CoverArtLink, imageBitmap);
                return imageBitmap;
            }
            catch (Exception e)
            {
                Bitmap imageBitmap = null;

                imageBitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_menu_gallery);

                return imageBitmap;
            }
        }

        public Bitmap GetImageBitmap(Podcast p)
        {
            try
            {
                if (p.CoverArtImage != null) { return p.CoverArtImage; }
                else if (linksToBitmaps.ContainsKey(p.CoverArtLink))
                {
                    p.CoverArtImage = linksToBitmaps[p.CoverArtLink];
                    return p.CoverArtImage;
                }

                Bitmap imageBitmap = null;

                using (var webClient = new System.Net.WebClient())
                {
                    var imageBytes = webClient.DownloadData(p.CoverArtLink);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }

                p.CoverArtImage = imageBitmap;
                linksToBitmaps.Add(p.CoverArtLink, imageBitmap);
                return imageBitmap;
            }
            catch (Exception e)
            {
                Bitmap imageBitmap = null;

                imageBitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_menu_gallery);

                return imageBitmap;
            }
        }

        public Bitmap GetImageBitmap(string imageURILink)
        {
            if(imageURILink == null || imageURILink == "")
            {
                Bitmap imageBitmap = null;

                imageBitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_menu_gallery);

                return imageBitmap;
            }

            if(linksToBitmaps.ContainsKey(imageURILink)) { return linksToBitmaps[imageURILink]; }
            try
            {
                Bitmap imageBitmap = null;

                using (var webClient = new System.Net.WebClient())
                {
                    var imageBytes = webClient.DownloadData(imageURILink);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }

                linksToBitmaps.Add(imageURILink, imageBitmap);
                return imageBitmap;
            }
            catch (Exception e)
            {
                Bitmap imageBitmap = null;

                imageBitmap = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_menu_gallery);

                return imageBitmap;
            }
        }

        public async void logout()
        {
            // Reset the user profile
            fireAuth.SignOut();
            if(CurrentPlayingEpisode != null)
            {
                await UpdateRecentlyListenedTo(CurrentPlayingEpisode, Player.CurrentPosition);
            }
            
            resetAPI();
        }

        private void resetAPI()
        {
            if (Player != null && Player.IsPlaying)
            {
                Player.Stop();
                Player.Release();
            }
            MainFeedPosts = null;
            MainFeedRecentlyListenedTo = null;
            SearchPageRecommendations = null;
            UserProfile = null;
            MainFeedIndex = 0;
            // Setup the media player
            Player = new MediaPlayer();
            Player.Completion += PlayerFinished;
            linksToBitmaps = new Dictionary<string, Bitmap>();
            SearchPageRecommendations = new Dictionary<string, List<Podcast>>();
            IsPlaying = false;
            CurrentPlayingEpisode = null;
        }

        /// <summary>
        /// Helper method for getting a time from milliseconds
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>
        public string getTimeString(long millis)
        {
            string buf = "";

            int hours = (int)(millis / (1000 * 60 * 60));
            int minutes = (int)((millis % (1000 * 60 * 60)) / (1000 * 60));
            int seconds = (int)(((millis % (1000 * 60 * 60)) % (1000 * 60)) / 1000);

            if (hours > 0)
            {
                buf = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
            }
            else
            {
                buf = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            }


            return buf;
        }
        #endregion Helper Methods

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// INotifyPropertyChanged Raise PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of property that changed</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
