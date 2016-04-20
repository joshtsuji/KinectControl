using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.SpeechBasics.Abilities
{
    public class CommuteAbility : IVoiceAbility
    {
        public Dictionary<string, string[]> GetCommandsAndPhrases()
        {
            return new Dictionary<string, string[]>() {
                { "UBER",
                    new string[] {
                    "computer, check uber",
                    "computer, check uber times",
                    "computer, check uber status"
                    }
                },

                { "HUBWAY",
                    new string[] {
                    "computer, check hubway status",
                    "computer, are there any bikes here",
                    "computer, are there any bikes nearby"
                    }
                },

                { "COMMUTE",
                    new string[] {
                    "computer, how should I get to work",
                    "computer, how should I go to work"
                    }
                },

                { "CT2",
                    new string[] {
                    "computer, when is the next bus to Kendall"
                    }
                },

                { "BUS55",
                    new string[] {
                    "computer, when is the next bus to Copley"
                    }
                },

                { "MBTA",
                    new string[] {
                    "computer, check t status",
                    "computer, check mbta status",
                    "computer, how's the t doing"
                    }
                },

                { "FLIGHT",
                    new string[] {
                    "computer, what's my flight status"
                    }
                },

                { "ALARM",
                    new string[] {
                    "computer, what times should I wake up",
                    "computer, when should I wake up"
                    }
                },
            };
        }


        async Task PromptDelay()
        {
            await Task.Delay(250);
        }

        async Task SpeechDelay()
        {
            await Task.Delay(3000);
        }


        public async void Execute(string command)
        {
            Utils.Speech.PlayPrompt();
            await PromptDelay();

            string response = "";

            switch (command)
            {
                case "HUBWAY":
                    response = "Fifteen bikes nearby, including four at Yawkey.";
                    break;
                case "UBER":
                    response = "The nearest Uber is 4 minutes away.";
                    break;
                case "MBTA":
                    response = "Green Line experiencing moderate delays in service due to a disabled train at Copley.";
                    break;
                case "COMMUTE":
                    response = "It's not expected to rain in the next hour, and there are four bikes at Yawkey. One dock is available at Ames Street, seven at Kendall, and eight at One Broadway." +
                        "Alternatively, the nearest Uber is 4 minutes away, and the Green and Red lines are operating normally.";
                    break;
                case "CT2":
                    response = "The next CT2 bus departs from Fenway Station in 12 minutes.";
                    break;
                case "BUS55":
                    response = "The next 55 bus departs from Jersey Street in 43 minutes.";
                    break;
                case "FLIGHT":
                    response = "JetBlue one three three from Boston to San Francisco is on time, and departs in 2 hours 16 minutes from gate C34";
                    break;
                case "ALARM":
                    response = "If you're going to sleep right now, recommended wake times are 8:34am, or 10:04am. Do you want to set an alarm?";
                    break;
            }

            Utils.Blocking.StartBlocking();

            Utils.Speech.Speak(response);
            await SpeechDelay();

            Utils.Blocking.StopBlocking();
        }
    }
}
