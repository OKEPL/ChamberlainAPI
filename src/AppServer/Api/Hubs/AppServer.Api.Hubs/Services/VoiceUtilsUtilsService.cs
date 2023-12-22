using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Chamberlain.AppServer.Api.Hubs.Contracts.Data_Structures;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.Common.Contracts.Enums;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.Database.Persistency.Model.Enums;
using Chamberlain.Prototypes.TextToSpeech;
using Plugins.VoiceCommandPlugin;
using Plugins.GoogleSpeechRecognition;
using VoiceApp.Contracts.Commands;


namespace Chamberlain.AppServer.Api.Hubs.Services
{
    public class VoiceUtilsUtilsService : IVoiceUtilsService
    {
        private readonly Polly _polly;
        private readonly IGoogleSpeechRecognition _googleSpeech;
        private Dictionary<int, PartialDataContainer> _dataSlicesByHash;

        private const int DataSliceLenght = 16000;

        public VoiceUtilsUtilsService(IGoogleSpeechRecognition googleSpeech)
        {
            _polly = new Polly();
            _googleSpeech = googleSpeech;
            _dataSlicesByHash = new Dictionary<int, PartialDataContainer>();
        }

        public byte[] GetBytesAndSave(string text)
        {
            return _polly.GetBytesAndSave(text, "x");
        }

        public void ReceiveSoundDownloadRequest(string text, IActorRef sender, string voiceUnitId)

        {
            var bytes = GetBytesAndSave(text);
            var slices = (int)Math.Ceiling(bytes.Length / (decimal)DataSliceLenght);

            var hash = bytes.GetHashCode();
            
            for (int i = 0; i < slices - 1; i++)
            {
                var chunk = bytes.Skip(i * DataSliceLenght).Take(DataSliceLenght).ToArray();
                var response = sender.Ask(new SynthesizerCommand.SoundDownloadPartialResponsePart(text, hash, slices, i, chunk, voiceUnitId),TimeSpan.FromSeconds(30)).Result;
            }
            var chunk2 = bytes.Skip((slices - 1) * DataSliceLenght).ToArray();
            var response2 = sender.Ask(new SynthesizerCommand.SoundDownloadPartialResponsePart(text ,hash, slices, slices - 1, chunk2, voiceUnitId), TimeSpan.FromSeconds(30)).Result;
        }

        public void GetDefaultSoundsList(string language, IActorRef sender, string requestId, string voiceUnitId)
        {
            switch (language)
            {
                case "english":
                    var list = new[]
                    {
                        "connection with the central unit has been lost"
                    };

                    sender.Tell(new SynthesizerCommand.DefaultSoundsList(list, requestId ,voiceUnitId));
                    break;
                default:
                    sender.Tell(new string[0]);
                    break;
            }
            
        }

        public void SendGrammarUpdate(IActorRef sender, string userName , string voiceUnitId)
        {
            using (var db = new Entities())
            {
                var customer = db.Customers.Single(x => x.Username == userName);

                var devices = db.Things.Where(x => x.CustomerId == customer.Id && x.State == ThingStates.Active);

                var grammarBuilder = new VoiceGrammarBuilder();

                var profiles = db.FaceProfiles.Where(x => x.CustomerId == customer.Id).Select(x => x.Name).ToList();
                grammarBuilder.InitilizeGrammar(devices.ToList(), profiles);

                sender.Tell(new GrammarUpdateCommand.NewGrammar(grammarBuilder.GetGrammarString(), voiceUnitId));
                sender.Tell(new GrammarUpdateCommand.NewKeyphrases(grammarBuilder.GetKeyphrasesString(), voiceUnitId));
                sender.Tell(new GrammarUpdateCommand.NewDictionary(grammarBuilder.GetDictionaryString(), voiceUnitId));
            }
        }

        public void ReceivePartialRequestPart(IActorRef sender, int requestHash, int slicesCount, int sliceId, byte[] data, string voiceUnitId, string requestId, GoogleRecognitionContext context)
        {
            if(!_dataSlicesByHash.ContainsKey(requestHash))
                _dataSlicesByHash[requestHash] = new PartialDataContainer(slicesCount);

            _dataSlicesByHash[requestHash].InsertSlice(sliceId, data);
            sender.Tell(new RecognitionCommand.PartReceived(voiceUnitId, requestId));
            if(_dataSlicesByHash[requestHash].IsComplete())
            { 
                var recognizedText = _googleSpeech.RecognizeFromBytes(_dataSlicesByHash[requestHash].GetCompleteData(), context);

                var response = new RecognitionCommand.GoogleSpeechResponse(recognizedText, voiceUnitId, requestHash);
                sender.Tell(response);

                _dataSlicesByHash.Remove(requestHash);
            }
        }
    }
}

