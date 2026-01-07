using System;
using System.Diagnostics;
using UnityEngine;

namespace PythonManagers {
    public class EmotionManager : MonoBehaviour {
        private static EmotionManager _instance;
        private static bool _isQuitting = false;

        public static EmotionManager Instance {
            get {
                if (_isQuitting) return null;
                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<EmotionManager>();
                if (_instance == null && !_isQuitting) {
                    GameObject container = new GameObject("EmotionManager");
                    _instance = container.AddComponent<EmotionManager>();
                }
                return _instance;
            }
        }

        private Process _proc;
        [SerializeField] private int _currentEmotion = 0;
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private const double InputDebounceMillis = 500; 

        public bool IsRunning => _proc != null && !_proc.HasExited;

        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _isQuitting = false;
        }

        private void OnDestroy() {
            if (_instance == this) _instance = null;
            StopPythonProcess();
        }

        private void OnApplicationQuit() {
            _isQuitting = true;
            StopPythonProcess();
        }

        public int GetEmotion() {
            if (this == null) return 0;
            int temp = _currentEmotion;
            _currentEmotion = 0; // 값 소모(Consume)
            return temp;
        }

        private void OnPythonOutput(object sender, DataReceivedEventArgs e) {
            if (string.IsNullOrWhiteSpace(e.Data)) return;
            string msg = e.Data.Trim().ToUpper();
            int newCode = 0;

            switch (msg) {
                case "JOY": newCode = 10; break;
                case "SAD": newCode = 20; break;
                case "ANGER": newCode = 30; break;
                case "DISLIKE": newCode = 40; break;
                case "NEUTRAL": case "NONE": newCode = 0; break;
                default: return;
            }

            if (newCode != _currentEmotion && newCode != 0) {
                if ((DateTime.Now - _lastUpdateTime).TotalMilliseconds > InputDebounceMillis) {
                    _currentEmotion = newCode;
                    _lastUpdateTime = DateTime.Now;
                    UnityEngine.Debug.Log($"<color=cyan>[Emotion]</color> {msg}");
                }
            }
        }

        public void StartPythonProcess() {
            if (IsRunning) return;
            string projectRoot = System.IO.Directory.GetParent(Application.dataPath).FullName;
            string pythonExePath = System.IO.Path.Combine(projectRoot, "venv", "Scripts", "python.exe");
            string scriptPath = System.IO.Path.Combine(Application.streamingAssetsPath, "python", "main_rule_based_classifier.py");

            if (!System.IO.File.Exists(pythonExePath) || !System.IO.File.Exists(scriptPath)) return;

            var psi = new ProcessStartInfo {
                FileName = pythonExePath,
                Arguments = $"-u \"{scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try {
                _proc = new Process();
                _proc.StartInfo = psi;
                _proc.OutputDataReceived += OnPythonOutput;
                _proc.Start();
                _proc.BeginOutputReadLine();
                _proc.BeginErrorReadLine();
            } catch (Exception e) { UnityEngine.Debug.LogError($"Emotion 실행 실패: {e.Message}"); }
        }

        public void StopPythonProcess() {
            try {
                if (_proc != null && !_proc.HasExited) {
                    _proc.Kill();
                    _proc.Dispose();
                    _proc = null;
                }
            } catch { }
        }
    }
}