using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    public interface IVoiceAbility
    {
        Dictionary<string, string[]> GetCommandsAndPhrases();
        void Execute(string command);
    }
}
