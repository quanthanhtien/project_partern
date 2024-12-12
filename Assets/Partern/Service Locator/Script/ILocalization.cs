using System.Collections.Generic;
using UnityEngine;

namespace Partern.Service_Locator.Script
{
    public interface ILocalization
    {
        string GetLocalizeWord(string key);
    }

    public class MockLocalization
    {
        readonly List<string> _words = new List<string>() { "hund", "katt", "fisk", "bil", "hus" };
        private readonly System.Random _random = new System.Random();

        public string GetLocalizedWord()
        {
            return _words[_random.Next(_words.Count)];
        }
    }

    public interface ISerializer
    {
        void Serializer();
    }

    public class MockSerializer : ISerializer
    {
        public void Serializer()
        {
            Debug.Log("Serializer");
        }
    }

    public interface IAudioService
    {
        void Play();
    }

    public interface IGameService
    {
        void Start();
    }

    public class MockGameService : IGameService
    {
        public void Start()
        {
            Debug.Log("MockGameService.StartGame");
        }
    }

    public class MockMapService : IGameService
    {
        public void Start()
        {
            Debug.Log("MockMapService.StartGame");
        }
    }
}
