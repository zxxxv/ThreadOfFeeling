using System;
using System.Diagnostics;
using UnityEngine;

namespace PythonManagers {
    public class SelectHandsManager : MonoBehaviour {
        private static SelectHandsManager _instance;
        private static bool _isQuitting = false;

        public static SelectHandsManager Instance {
            get {
                if (_isQuitting) return null;
                if (_instance != null) return _instance;
                _instance = FindFirstObjectByType<SelectHandsManager>();
                if (_instance == null && !_isQuitting) {
                    GameObject container = new GameObject("SelectHandsManager");
                    _instance = container.AddComponent<SelectHandsManager>();
                }
                return _instance;
            }
        }

        private Process _proc;
        [SerializeField] private int _currentHand = 0;
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

        public int GetHandCode() {
            if (this == null) return 0;
            int temp = _currentHand;
            _currentHand = 0; // 값 소모(Consume)
            return temp;
        }

        private void OnPythonOutput(object sender, DataReceivedEventArgs e) {
            if (string.IsNullOrWhiteSpace(e.Data)) return;
            var data = e.Data.Trim().ToUpper();
            int newCode = 0;

            switch (data) {
                case "LEFT": newCode = 10; break;
                case "RIGHT": newCode = 20; break;
                case "BOTH": case "NONE": newCode = 0; break;
                default: return; 
            }

            if (newCode != _currentHand && newCode != 0) {
                if ((DateTime.Now - _lastUpdateTime).TotalMilliseconds > InputDebounceMillis) {
                    _currentHand = newCode;
                    _lastUpdateTime = DateTime.Now;
                    UnityEngine.Debug.Log($"<color=yellow>[Hand]</color> {data}");
                }
            }
        }

        public void StartPythonProcess() {
            if (IsRunning) return;
            string projectRoot = System.IO.Directory.GetParent(Application.dataPath).FullName;
            string pythonExePath = System.IO.Path.Combine(projectRoot, "venv", "Scripts", "python.exe");
            string scriptPath = System.IO.Path.Combine(Application.streamingAssetsPath, "python", "main_rule_based_hands_filter.py");

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
            } catch (Exception e) { UnityEngine.Debug.LogError($"Hands 실행 실패: {e.Message}"); }
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