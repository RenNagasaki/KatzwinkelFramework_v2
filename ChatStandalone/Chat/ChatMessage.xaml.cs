using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatStandalone
{
    /// <summary>
    /// Interaction logic for ChatMessage.xaml
    /// </summary>
    public partial class ChatMessage : UserControl
    {
        public ChatMessage(Framework.ChatMessage message)
        {
            InitializeComponent();

            if (message.Message.StartsWith(Environment.NewLine))
            {
                message.Message = message.Message.Substring(2, message.Message.Length - 2);
            }

            if (message.Message.EndsWith(Environment.NewLine))
            {
                message.Message = message.Message.Substring(0, message.Message.Length - 2);
            }

            CheckCommand(message.Message);
            label_date.Content = message.Time.ToShortDateString() + " - " + message.Time.ToShortTimeString();
        }

        void Url_Click(object sender, RoutedEventArgs e)
        {
            Paragraph para = sender as Paragraph;
            System.Diagnostics.Process.Start(String.Join(String.Empty, para.Inlines.Select(line => line.ContentStart.GetTextInRun(LogicalDirection.Forward))));
        }

        void Url_Mouse_Enter(object sender, RoutedEventArgs e)
        {
            text_message.Cursor = Cursors.Hand;
        }

        void Url_Mouse_Leave(object sender, RoutedEventArgs e)
        {
            text_message.Cursor = Cursors.Arrow;
        }

        void CheckCommand(string msg)
        { 
            Match regExMatch = Regex.Match(msg, @"(ftp|https?)://[^\s]+");
            string url = regExMatch.Groups[0].Value;
            int startInd = -1;
            int endInd = -1;

            while (!String.IsNullOrEmpty(url))
            {
                startInd = msg.IndexOf(url);
                endInd = startInd + url.Length;

                string start = msg.Substring(0, startInd);
                string command = msg.Substring(startInd, endInd - startInd);
                string end = msg.Substring(endInd, msg.Length - endInd);

                if (!String.IsNullOrEmpty(start))
                {
                    Paragraph paragraphStart = new Paragraph();
                    paragraphStart.Inlines.Add(start);
                    text_message.Document.Blocks.Add(paragraphStart);
                }

                if (!String.IsNullOrEmpty(command))
                {
                    Paragraph paragraphComm = new Paragraph();
                    paragraphComm.Inlines.Add(command);
                    paragraphComm.MouseLeftButtonDown += new MouseButtonEventHandler(Url_Click);
                    paragraphComm.MouseEnter += new MouseEventHandler(Url_Mouse_Enter);
                    paragraphComm.MouseLeave += new MouseEventHandler(Url_Mouse_Leave);
                    paragraphComm.TextDecorations.Add(TextDecorations.Underline);
                    paragraphComm.TextEffects.Add(new TextEffect() { Foreground = System.Windows.Media.Brushes.Blue });
                    text_message.Document.Blocks.Add(paragraphComm);
                }

                msg = end;
                regExMatch = Regex.Match(msg, @"(ftp|https?)://[^\s]+");
                url = regExMatch.Groups[0].Value;
            }

            startInd = msg.IndexOf("[");
            endInd = msg.IndexOf("]") + 1;

            while (startInd >= 0 && endInd > startInd)
            {
                Paragraph para = new Paragraph();
                string start = msg.Substring(0, startInd);
                string command = msg.Substring(startInd, endInd - startInd);
                string end = msg.Substring(endInd, msg.Length - endInd);

                if (!String.IsNullOrEmpty(start))
                {
                    para.Inlines.Add(start);
                }
                bool res = GetSmiley(ref para, command.Replace("[", "").Replace("]", ""));

                if (!res)
                {
                    if (!String.IsNullOrEmpty(command))
                    {
                        para.Inlines.Add(command);
                    }
                }

                msg = end;
                startInd = msg.IndexOf("[");
                endInd = msg.IndexOf("]") + 1;

                text_message.Document.Blocks.Add(para);
            }

            if (!String.IsNullOrEmpty(msg))
            {
                Paragraph para = new Paragraph();
                para.Inlines.Add(msg);
                text_message.Document.Blocks.Add(para);
            }
        }

        bool GetSmiley(ref Paragraph para, string name)
        {
            if (App.Smileys.Contains(name))
            {
                BitmapImage bitmap = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Smileys\\" + name + ".png"));
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                image.Source = bitmap;
                image.Width = 30;
                para.Inlines.Add(image);

                return true;
            }

            return false;
        }
    }
}
