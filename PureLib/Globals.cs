namespace Anomaly
{
    public static class Globals
    {
        //This Class Stores strings to be used in other Functions. I store them here to keep them safe.
        public static string VersionNumber { get; set; } = "0.04#20";
        public static string ChangelogURL { get; set; } = "https://raw.githubusercontent.com/PurityWasHere/OpenBullet-Anomaly-Plugin/master/Changelog.txt";
        public static string VersionURL { get; set; } = "https://raw.githubusercontent.com/PurityWasHere/OpenBullet-Anomaly-Plugin/master/VersionNumber.txt";
        public static string LatestDownload { get; set; } = "https://github.com/PurityWasHere/OpenBullet-Anomaly-Plugin/releases/latest/download/Anomaly.zip";
        public static string LatestAPIVersion { get; set; } = "https://api.github.com/repos/PurityWasHere/OpenBullet-Anomaly-Plugin/releases/latest";
        public static string HttpProxyUrl { get; set; } = "https://api.proxyscrape.com/?request=getproxies&proxytype=http&timeout=10000&country=all&ssl=all&anonymity=all";
        public static string Socks4ProxyUrl { get; set; } = "https://api.proxyscrape.com/?request=getproxies&proxytype=socks4&timeout=10000&country=all";
        public static string Socks5ProxyUrl { get; set; } = "https://api.proxyscrape.com/?request=getproxies&proxytype=socks5&timeout=10000&country=all";
    }
}