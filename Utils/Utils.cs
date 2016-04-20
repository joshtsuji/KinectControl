using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Net;

namespace Microsoft.Samples.Kinect.SpeechBasics.Utils
{

    class Blocking
    {
        private static bool blocking = false;

        public static void StartBlocking()
        {
            blocking = true;
        }

        public static void StopBlocking()
        {
            blocking = false;
        }

        public static bool IsBlocking()
        {
            return blocking;
        }
    }
    class Speech
    {
        static SoundPlayer acknowledge = new SoundPlayer("C:\\Users\\Joshua\\Documents\\SpeechBasics-WPF\\Images\\done.wav");
        static SoundPlayer prompt = new SoundPlayer("C:\\Users\\Joshua\\Documents\\SpeechBasics-WPF\\Images\\prompt.wav");
        public static void PlayAcknowledge()
        {
            acknowledge.Play();
        }

        public static void PlayPrompt()
        {
            prompt.Play();
        }

        public static void Speak(string stringData)
        {
            string msgdata = "{\"text\":\"" + stringData + "\"}";
            int DataLength = msgdata.Length;
            Stream stdout = Console.OpenStandardOutput();
            stdout.WriteByte((byte)((DataLength >> 0) & 0xFF));
            stdout.WriteByte((byte)((DataLength >> 8) & 0xFF));
            stdout.WriteByte((byte)((DataLength >> 16) & 0xFF));
            stdout.WriteByte((byte)((DataLength >> 24) & 0xFF));

            Console.Write(msgdata);
        }
    }

    class Hue
    {
        public static void SelectScene(string scene)
        {
            var httpWebRequest = (HttpWebRequest) 
                WebRequest.Create("http://192.168.1.253/api/newdeveloper/groups/0/action");
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "PUT";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"scene\":\"" + scene + "\" }";
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                
            }
        }
    }
}
