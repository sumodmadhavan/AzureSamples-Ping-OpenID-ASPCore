namespace Microsoft.AspNetCore.Authentication
{
    public class AzurePingOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Authority { get; set; }

        public string CallbackPath { get; set; }
    }
}
