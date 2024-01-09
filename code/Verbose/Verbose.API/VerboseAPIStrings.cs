using Amazon;

namespace Verbose.API
{
    public class VerboseAPIStrings
    {
        /// <summary>
        /// Different server base will go here
        /// </summary>
        //public static string ServerBase = "http://192.168.1.150:2999";
        //public static string ServerBase = "http://localhost:2999";
        //public static string ServerBase = "http://10.17.218.127:2999";
        //public static string ServerBase = "http://10.0.2.2:2999";
        //public static string ServerBase = "http://10.0.0.54:2999";
        public static string ServerBase = "http://ec2-34-233-37-52.compute-1.amazonaws.com/";


        public static string LoginEndPoint = "/login";
        public static string GetMainFeedEndPoint = "/api/Feed/GetMainFeedPosts";
        public static string GetRecentlyListenedToEndPoint = "/api/Feed/GetRecentlyListenedTo";
        public static string TestEndPoint = "/";
        public static string RefreshUserPostsEndPoint = "/api/Feed/RefreshUserPosts";

        public static string CheckMadeAccountEndPoint = "/api/Login/CheckAccountCreated";
        public static string CreateUserEndPoint = "/api/Login/CreateNewUser";
        public static string CheckUsernameAvailableEndPoint = "/api/Login/CheckUserNameAvailable";

        public static string SubmitCommentEndPoint = "/api/Comments/SubmitComment";

        public static string LikeOrUnlikeEpisodeEndPoint = "/api/Episodes/LikeOrUnlike";
        public static string UpdateRecentlyListenedToEndPoint = "/api/Episodes/ListenedTo";
        public static string GetUserListenedToEndPoint = "/api/Episodes/Listen";

        public static string GetPodcastsEndPoint = "/api/Podcasts/GetPodcastFromEpisode?podcastId=";

        public static string SubmitPostEndPoint = "/api/Posts/SubmitPost";
        public static string LikeOrUnlikePostEndPoint = "/api/Posts/LikeOrUnlike";

        public static string SearchPodcastsEndPoint = "/api/PodchaserPodcast";
        public static string SearchRecommendationsEndPoint = "/api/CategorySeed";

        public static string SearchFriendsEndPoint = "/api/PublicProfiles/SearchUser";

        public static string SubmitRssLinkEndPoint = "/api/Profiles/VerifyCreator";

        public static string GetOtherUserPublicProfileEndPoint = "/api/PublicProfiles/GetOtherUser";
        public static string SubscribeOrUnsubscribePodcastEndPoint = "/api/PublicProfiles/SubOrUnsubPodcast";
        public static string FollowOrUnfollowPublicProfileEndPoint = "/api/PublicProfiles/FollowOrUnfollow";
        public static string SetPrivacyEndpoint = "/api/PublicProfiles/SetPrivacy";
        public static string UpdateProfilePictureEndpoint = "/api/PublicProfiles/UpdateProfilePicture";
        public static string GetUserProfilePictureEndpoint = "/api/PublicProfiles/GetPublicProfilePicture"; 

        public static string GetClosedCaptionsEndPoint = "/api/Captions";

        // AWS Region Endpoints
        public static RegionEndpoint CognitoRegion = RegionEndpoint.USWest1;
        public static RegionEndpoint SnsRegion = RegionEndpoint.USWest1;
    }
}
