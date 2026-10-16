using System;

namespace CybersecurityAwarenessBotGUI
{
    public class ChatbotMemory
    {
        public string UserName { get; private set; } = "";
        public string LastTopic { get; set; } = "";
        private string userTopic = "";

        public void RememberName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                UserName = name;
        }

        public string RecallName()
        {
            if (string.IsNullOrEmpty(UserName))
                return "You haven't told me your name yet. What's your name?";

            return $"Your name is {UserName}.";
        }

        public void RememberTopic(string topic)
        {
            if (!string.IsNullOrWhiteSpace(topic))
                userTopic = topic;
        }

        public string RecallTopic()
        {
            if (string.IsNullOrEmpty(userTopic))
                return "I haven't learned about your interests yet. What topic interests you?";

            return $"You're interested in {userTopic}.";
        }
    }
}
