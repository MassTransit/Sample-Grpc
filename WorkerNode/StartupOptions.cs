namespace WorkerNode
{
    using System;
    using System.Collections.Generic;


    public class StartupOptions
    {
        /// <summary>
        /// Hosts to connect when started
        /// </summary>
        public string Servers { get; set; }

        /// <summary>
        /// Port to use for the host
        /// </summary>
        public int? Port { get; set; }

        public int? Count { get; set; }

        /// <summary>
        /// Override the default host address (127.0.0.1)
        /// </summary>
        public string Host { get; set; }

        public IEnumerable<Uri> GetServers()
        {
            if (string.IsNullOrWhiteSpace(Servers))
                yield break;

            var hosts = Servers.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var host in hosts)
            {
                if (Uri.IsWellFormedUriString(host, UriKind.Absolute))
                    yield return new Uri(host);
            }
        }
    }
}