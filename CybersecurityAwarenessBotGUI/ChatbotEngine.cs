using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBotGUI
{
    public class ChatbotEngine
    {
        private readonly ChatbotMemory memory = new ChatbotMemory();
        private readonly SentimentAnalyzer sentiment = new SentimentAnalyzer();
        private readonly Random random = new Random();

        private readonly Dictionary<string, List<string>> tips =
            new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new List<string>
                {
                    "Use strong, unique passwords for every account.",
                    "A password manager can generate and store strong passwords.",
                    "Never share your passwords with anyone.",
                    "Use long passphrases and enable multi-factor authentication."
                },
                ["phishing"] = new List<string>
                {
                    "Always check the sender's email address before responding.",
                    "Avoid clicking unexpected links in emails or messages.",
                    "Be cautious of messages demanding urgent action.",
                    "Visit a company's official website instead of following suspicious links.",
                    "Never enter your password on a website reached through an unexpected message."
                },
                ["scam"] = new List<string>
                {
                    "Be suspicious of offers that sound too good to be true.",
                    "Never send money to strangers who promise guaranteed rewards.",
                    "Verify requests for payments through official contact channels.",
                    "Do not share OTPs or banking PINs with anyone."
                },
                ["privacy"] = new List<string>
                {
                    "Review privacy settings on your social media accounts.",
                    "Avoid sharing sensitive personal information publicly.",
                    "Only give applications permissions they genuinely need.",
                    "Use secure connections when accessing sensitive accounts."
                },
                ["malware"] = new List<string>
                {
                    "Keep your operating system and applications updated.",
                    "Avoid downloading files from untrusted websites.",
                    "Use reputable security software.",
                    "Back up important files regularly."
                },
                ["mfa"] = new List<string>
                {
                    "Enable multi-factor authentication wherever possible.",
                    "Authenticator apps or passkeys are useful security options.",
                    "Never share authentication codes with anyone.",
                    "Keep backup recovery codes in a secure location."
                }
            };

        private readonly Dictionary<string, string> explanations =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = "A password protects your account. A long, unique password makes it harder for attackers to gain access.",
                ["phishing"] = "Phishing happens when criminals pretend to be trusted organisations to trick people into revealing information.",
                ["scam"] = "An online scam is a dishonest attempt to steal money, personal information or access to accounts.",
                ["privacy"] = "Online privacy means controlling who can access your personal information and how it is used.",
                ["malware"] = "Malware is harmful software that can steal information, damage files or interfere with your device.",
                ["mfa"] = "Multi-factor authentication adds another security check beyond your password."
            };

        private string previousTip = "";

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type a question so I can help you.";

            string message = input.Trim();
            string lower = message.ToLowerInvariant();

            try
            {
                Match nameMatch = Regex.Match(message, @"\bmy name is\s+([A-Za-z]+(?:[ '-][A-Za-z]+){0,2})", RegexOptions.IgnoreCase);

                if (nameMatch.Success)
                {
                    string name = nameMatch.Groups[1].Value.Trim();
                    memory.RememberName(name);
                    return $"Nice to meet you, {name}! What cybersecurity topic would you like to discuss?";
                }

                if (Regex.IsMatch(lower, @"\b(what is my name|do you remember my name|who am i)\b"))
                    return memory.RecallName();

                if (Regex.IsMatch(lower, @"\b(what is my favourite topic|what am i interested in|remember my interest)\b"))
                    return memory.RecallTopic();

                if (lower == "help")
                    return "You can ask about passwords, phishing, scams, privacy, malware or MFA. You can also say 'another tip', 'tell me more', or 'my name is Alex'.";

                if (Regex.IsMatch(lower, @"\b(hi|hello|hey)\b") && lower.Length < 20)
                {
                    string name = memory.UserName;
                    return string.IsNullOrEmpty(name)
                        ? "Hello! Welcome to the Cybersecurity Awareness Chatbot. How can I help you?"
                        : $"Hello, {name}! What would you like to learn today?";
                }

                if (Regex.IsMatch(lower, @"\b(bye|goodbye|exit)\b"))
                    return "Goodbye! Remember to stay safe online.";

                string topic = FindTopic(lower);

                if (!string.IsNullOrEmpty(topic) && Regex.IsMatch(lower, @"\b(interested in|favourite topic is|favorite topic is|i like)\b"))
                {
                    memory.RememberTopic(topic);
                    return $"Great! I'll remember that you're interested in {topic}. {explanations[topic]}";
                }

                if (Regex.IsMatch(lower, @"\b(don't understand|do not understand|confused|what do you mean|explain more|tell me more)\b"))
                {
                    string currentTopic = !string.IsNullOrEmpty(topic) ? topic : memory.LastTopic;

                    if (!string.IsNullOrEmpty(currentTopic))
                    {
                        memory.LastTopic = currentTopic;
                        return "Let me explain it simply. " + explanations[currentTopic] + " " + GetRandomTip(currentTopic);
                    }

                    return "Of course! Tell me which cybersecurity topic you would like me to explain.";
                }

                if (Regex.IsMatch(lower, @"\b(another tip|more tips|next tip|give me more)\b"))
                {
                    string currentTopic = !string.IsNullOrEmpty(topic) ? topic : memory.LastTopic;

                    if (!string.IsNullOrEmpty(currentTopic))
                    {
                        memory.LastTopic = currentTopic;
                        return GetRandomTip(currentTopic);
                    }

                    return "Which topic would you like tips about? Try phishing, passwords, scams or privacy.";
                }

                string mood = sentiment.DetectSentiment(lower);

                if (mood == "positive" && string.IsNullOrEmpty(topic))
                    return "You're welcome! I'm glad I could help. Stay safe online!";

                if (mood == "worried" && string.IsNullOrEmpty(topic))
                    return "I understand your concern. Online safety can feel overwhelming. Tell me what worries you, and I'll help you step by step.";

                if (!string.IsNullOrEmpty(topic))
                {
                    memory.LastTopic = topic;

                    string response = explanations[topic] + " " + GetRandomTip(topic);

                    if (mood == "worried")
                        response = "It's understandable to feel concerned. " + response;

                    if (mood == "curious")
                        response = "It's great that you want to learn! " + response;

                    return Personalise(response);
                }

                return "I'm not sure I understand. Can you try rephrasing? You can ask about passwords, scams, phishing, privacy, malware or MFA.";
            }
            catch (Exception)
            {
                return "Sorry, something went wrong while processing your message. Please try again.";
            }
        }

        private string FindTopic(string message)
        {
            if (Regex.IsMatch(message, @"\b(phishing|phish|suspicious email)\b"))
                return "phishing";

            if (Regex.IsMatch(message, @"\b(password|passwords|passphrase)\b"))
                return "password";

            if (Regex.IsMatch(message, @"\b(scam|scams|fraud|scammer)\b"))
                return "scam";

            if (Regex.IsMatch(message, @"\b(privacy|private information|personal information)\b"))
                return "privacy";

            if (Regex.IsMatch(message, @"\b(malware|virus|ransomware)\b"))
                return "malware";

            if (Regex.IsMatch(message, @"\b(mfa|2fa|multi-factor|two-factor|authentication)\b"))
                return "mfa";

            return "";
        }

        private string GetRandomTip(string topic)
        {
            List<string> options = tips[topic];

            if (options.Count == 1)
                return options[0];

            string selected;
            int attempts = 0;

            do
            {
                selected = options[random.Next(options.Count)];
                attempts++;
            }
            while (selected == previousTip && attempts < 20);

            previousTip = selected;
            return selected;
        }

        private string Personalise(string response)
        {
            if (string.IsNullOrEmpty(memory.UserName))
                return response;

            return $"{memory.UserName}, {response}";
        }
    }
}

