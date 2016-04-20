using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.SpeechBasics.Abilities
{
    public class HueAbility : IVoiceAbility
    {
        private List<string[]> bedroomScenes = new List<string[]>() {
            new string[] { "a85c635bc-on-0", "turquoise" },
            new string[] { "3b75f9f64-on-0", "sunrise" },
            new string[] { "d7e9b8475-on-0", "night mode" }
        };

        private List<string[]> scenes = new List<string[]>()
        {
            new string[] { "0d472b2d2-on-0", "turquoise" },
            new string[] { "fcac600e1-on-0", "sunrise" },
            new string[] { "b75317ab9-on-0", "night mode" }
        };

        private Dictionary<string, string> commandToName = new Dictionary<string, string>();

        public Dictionary<string, string[]> GetCommandsAndPhrases()
        {
            Dictionary<string, string[]> commands = new Dictionary<string, string[]>();
            foreach (string[] scene in bedroomScenes)
            {
                String sceneId = scene[0];
                String sceneName = scene[1];

                String commandName = "HUE_" + sceneId;
                commands[commandName] = new string[]
                {
                    "computer, change lights to " + sceneName,
                    "computer, change to " + sceneName + " lighting",
                    "computer, set lights to " + sceneName,
                    "computer, set lighting to " + sceneName,
                    "computer, " + sceneName + " lights",
                    "computer, " + sceneName + " lighting",
                    "computer, " + sceneName,
                };
                commandToName.Add(commandName, sceneName);
            }

            return commands;
        }

       
        public void Execute(string command)
        {
            String sceneName = commandToName[command];
            Utils.Hue.SelectScene(command.Substring(4));
            Utils.Speech.PlayPrompt();
        }
    }
}