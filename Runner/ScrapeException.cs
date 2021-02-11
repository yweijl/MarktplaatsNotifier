using System;

namespace Runner
{
    public class ScrapeException : Exception
    {

        public string Url { get; }
        public ScrapeException()
        {

        }
        public ScrapeException(string message) : base(message)
        {

        }

        public ScrapeException(string message, string url ,Exception inner) : base(message, inner)
        {
            Url = url;
        }
    }
}
