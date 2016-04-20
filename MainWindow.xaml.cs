//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SpeechBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using Microsoft.Speech.AudioFormat;
    using Microsoft.Speech.Recognition;
    using System.Net;
    using Speech.Synthesis;
    using System.Windows.Controls;
    using Abilities;
    using System.Media;
    public partial class MainWindow : Window
    {

        IVoiceAbility[] abilities = {
            new CommuteAbility(),
            new HueAbility()
        };

        private KinectSensor kinectSensor = null;
        private KinectAudioStream convertStream = null;
        private SpeechRecognitionEngine speechEngine = null;
        private List<Span> recognitionSpans;
        
        private Choices commands = new Choices();
        private Dictionary<string, IVoiceAbility> commandToVoiceAbility = new Dictionary<string, IVoiceAbility>();
        private List<String> allValidPhrases = new List<String>();
        private Dictionary<string, string> phraseToCommand = new Dictionary<string, string>();

        public MainWindow()
        {
            this.InitializeComponent();
            foreach (IVoiceAbility ability in abilities)
            {
                Dictionary<string, string[]> commandsAndPhrases = ability.GetCommandsAndPhrases();
                foreach (String command in commandsAndPhrases.Keys)
                {
                    string[] phrases = commandsAndPhrases[command];
                    foreach (String phrase in phrases)
                    {
                        commands.Add(new SemanticResultValue(phrase, command));
                        allValidPhrases.Add(phrase);
                        phraseToCommand.Add(phrase, command);
                    }

                    commandToVoiceAbility.Add(command, ability);
                }
            }
        }

        

        public async void HandleCommandString(string command)
        {
            if (!Utils.Blocking.IsBlocking())
            {
                IVoiceAbility relevantAbility = commandToVoiceAbility[command];
                relevantAbility.Execute(command);
            }
            else
            {

            }
        }

        private static RecognizerInfo TryGetKinectRecognizer()
        {
            IEnumerable<RecognizerInfo> recognizers;
            
            try
            {
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            }
            catch (COMException)
            {
                return null;
            }

            foreach (RecognizerInfo recognizer in recognizers)
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }
        
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Only one sensor is supported
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                // open the sensor
                this.kinectSensor.Open();

                // grab the audio stream
                IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                // create the convert stream
                this.convertStream = new KinectAudioStream(audioStream);
            }
            else
            {
                // on failure, set the status text
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
                return;
            }

            RecognizerInfo ri = TryGetKinectRecognizer();

            if (null != ri)
            {


                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                
                
                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(commands);
                var g = new Grammar(gb);
                this.speechEngine.LoadGrammar(g);

                this.speechEngine.SpeechRecognized += this.SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected += this.SpeechRejected;
                this.speechEngine.SpeechHypothesized += this.SpeechHypothesized;

                // let the convertStream know speech is going active
                this.convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                this.speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.convertStream)
            {
                this.convertStream.SpeechActive = false;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= this.SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= this.SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }

            if (null != this.kinectSensor)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        bool alreadyHandled = false;
        SoundPlayer player = new SoundPlayer("C:\\Users\\Joshua\\Documents\\SpeechBasics-WPF\\Images\\startlisten.wav");
        private void SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {

            LogText.Text = e.Result.Confidence + ": " + e.Result.Text + "\n" + LogText.Text;

            if (e.Result.Confidence < 0.25)
                return;

            



            if (!alreadyHandled &&
                allValidPhrases.Contains(e.Result.Text.Trim()))
            {
                LogText.Text = "Handling..." + "\n" + LogText.Text;
                if (e.Result.Semantics.Value != null)
                    HandleCommandString(e.Result.Semantics.Value.ToString());
                else
                    HandleCommandString(phraseToCommand[e.Result.Text]);
                alreadyHandled = true;
            }
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            LogText.Text = "> " + e.Result.Confidence + ": " + e.Result.Text + "\n" + LogText.Text;
            if (!alreadyHandled) {
                if (e.Result.Confidence > 0.25)
                {
                    LogText.Text = "> Handling..." + "\n" + LogText.Text;
                    HandleCommandString(e.Result.Semantics.Value.ToString());
                }
            }

            alreadyHandled = false;
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            
        }

       
    }
}