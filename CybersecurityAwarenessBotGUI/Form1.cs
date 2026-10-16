using CybersecurityAwarenessBotGUI;
using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace CybersecurityAwarenessBot
{
    public class Form1 : Form
    {
        private readonly ChatbotEngine chatbot = new ChatbotEngine();

        private RichTextBox chatDisplay;
        private TextBox userInput;
        private Button sendButton;
        private Button soundButton;
        private SoundPlayer welcomePlayer;

        public Form1()
        {
            CreateInterface();
            ShowWelcome();
        }

        private void CreateInterface()
        {
            Text = "Cybersecurity Awareness Chatbot";
            Size = new Size(850, 650);
            MinimumSize = new Size(650, 500);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(15, 23, 42);
            Font = new Font("Segoe UI", 10);

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(20)
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));

            Panel header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 41, 59)
            };

            Label heading = new Label
            {
                Text = "CYBERSECURITY AWARENESS CHATBOT",
                ForeColor = Color.FromArgb(56, 189, 248),
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 45,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label subtitle = new Label
            {
                Text = "Learn how to protect yourself online",
                ForeColor = Color.WhiteSmoke,
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            header.Controls.Add(heading);
            header.Controls.Add(subtitle);

            chatDisplay = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 11),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                DetectUrls = true
            };

            TableLayoutPanel inputPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };

            inputPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            inputPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 100));
            inputPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 100));

            userInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12),
                PlaceholderText = "Type your message...",
                Margin = new Padding(0, 10, 10, 10)
            };

            userInput.KeyDown += UserInput_KeyDown;

            sendButton = new Button
            {
                Text = "Send",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(14, 165, 233),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 8, 8, 8)
            };

            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += SendButton_Click;

            soundButton = new Button
            {
                Text = "Play Voice",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 8, 0, 8)
            };

            soundButton.FlatAppearance.BorderSize = 0;
            soundButton.Click += SoundButton_Click;

            inputPanel.Controls.Add(userInput, 0, 0);
            inputPanel.Controls.Add(sendButton, 1, 0);
            inputPanel.Controls.Add(soundButton, 2, 0);

            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(chatDisplay, 0, 1);
            layout.Controls.Add(inputPanel, 0, 2);

            Controls.Add(layout);
            AcceptButton = sendButton;
        }

        private void ShowWelcome()
        {
            string asciiArt =
@"  ==================================
       CYBERSECURITY CHATBOT
            [ SECURE ]
  ==================================";

            AppendMessage("SYSTEM", asciiArt, Color.Cyan);

            AppendMessage("CHATBOT",
                "Welcome! I am your Cybersecurity Awareness Chatbot. " +
                "Ask me about passwords, phishing, scams, privacy, malware or MFA. " +
                "Type 'help' to see what I can do.",
                Color.LightGreen);

            // Play the welcome recording once, if available.
            PlayWelcomeAudio();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string message = userInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(
                    "Please enter a message.",
                    "Empty Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            AppendMessage("YOU", message, Color.LightSkyBlue);

            userInput.Clear();

            string response = chatbot.GetResponse(message);

            AppendMessage("CHATBOT", response, Color.LightGreen);

            userInput.Focus();
        }

        private void AppendMessage(
            string sender, string message, Color color)
        {
            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.SelectionLength = 0;
            chatDisplay.SelectionColor = color;

            chatDisplay.AppendText(
                sender + ":\n" + message + "\n\n");

            chatDisplay.SelectionColor = chatDisplay.ForeColor;
            chatDisplay.SelectionStart = chatDisplay.TextLength;
            chatDisplay.ScrollToCaret();
        }

        private void SoundButton_Click(object sender, EventArgs e)
        {
            PlayWelcomeAudio();
        }

        private void PlayWelcomeAudio()
        {
            string path = Path.Combine(
                AppContext.BaseDirectory, "welcome.wav");

            if (!File.Exists(path))
            {
                // Missing audio should not crash the chatbot.
                return;
            }

            try
            {
                if (welcomePlayer == null)
                    welcomePlayer = new SoundPlayer(path);

                welcomePlayer.Play();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "The welcome audio could not be played.",
                    "Audio Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}

