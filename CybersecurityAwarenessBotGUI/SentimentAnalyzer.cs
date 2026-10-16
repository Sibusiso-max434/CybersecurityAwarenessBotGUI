using System;

namespace CybersecurityAwarenessBotGUI
{
    public class SentimentAnalyzer
    {
        public string DetectSentiment(string message)
        {
            message = message.ToLowerInvariant();

            if (message.Contains("thank") || message.Contains("thanks") ||
                message.Contains("great") || message.Contains("good") ||
                message.Contains("helpful") || message.Contains("appreciate"))
                return "positive";

            if (message.Contains("worried") || message.Contains("scared") ||
                message.Contains("afraid") || message.Contains("concerned") ||
                message.Contains("anxious") || message.Contains("dangerous") ||
                message.Contains("danger") || message.Contains("help"))
                return "worried";

            if (message.Contains("how") || message.Contains("why") ||
                message.Contains("what") || message.Contains("curious") ||
                message.Contains("learn") || message.Contains("tell me"))
                return "curious";

            return "neutral";
        }
    }
}

