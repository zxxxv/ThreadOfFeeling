using System;
//using System.Diagonstics;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Managers {
    public class EmotionManager : MonoBehaviour {
        public static EmotionManager Instance {get; private set; }

        private Process _proc;
        private int _currentEmotion = 0;

        private void Awake()
        {
            if(Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartPython();

        }

        private void StartPython()
        {
            var psi = new ProcessStartInfo()
            {
                FileName = "python3",
                Arguments = "../python/main_rule_based_classifier.py",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _proc = new Process();
            _proc.StartInfo = psi;
            _proc.OutputDataReceived += OnPythonOutput;
            _proc.Start();
            _proc.BeginOutputReadLine();
        }

        private void OnPythonOutput(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data)) return;

            string msg = e.Data.Trim();
            int next = EmotionStringToNumber(msg);

            if(next != _currentEmotion)
            {
                _currentEmotion = next;
            }
        }

        private int EmotionStringToNumber(string s)
        {
            return s switch
            {
                "JOY" => 1,
                "SAD" => 2,
                "ANGER" => 3,
                "DISLIKE" => 4,

                "NEUTRAL" => _currentEmotion,
                "NONE" => _currentEmotion,
                "UNKNOWN" => _currentEmotion,
                "HOLD" => _currentEmotion,

                _ => _currentEmotion
            };
        }

        public int GetEmotion()
        {
            return _currentEmotion;
        }
    }
}

