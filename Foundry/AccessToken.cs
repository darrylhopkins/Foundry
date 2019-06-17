namespace Foundry
{
    internal class AccessToken
    {
        string access_token { get; set; }
        string token_type { get; set; }
        int expires_in { get; set; }
        int created_at { get; set; }
    }
}