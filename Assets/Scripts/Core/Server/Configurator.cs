using System;
using IniParser;
using IniParser.Model;
using UnityEngine;

namespace Core.Server
{
    public class Configurator : MonoBehaviour
    {
        public static FileIniDataParser parser;
        public static IniData data;
        
        private void Awake()
        {
            parser = new FileIniDataParser();
            data = parser.ReadFile($@"{Application.dataPath}\Configuration.ini");
        }
    }
}