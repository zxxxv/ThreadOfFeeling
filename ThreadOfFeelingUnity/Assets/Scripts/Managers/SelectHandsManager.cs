using System;
using System.Diagnostics;
using UnityEngine;

namespace Managers{
  public class SelectHandsManager : MonoBehaviour {
    public static SelectHandsManager Instance {get; private set;}

    private Process _proc;
    private int _currentHand = 0;

    private void Awake(){
      if (Instance != null && Instance != this){
        Destroy(gameObject);
        return;
      }

      Instance = this;
      DontDestroyOnLoad(gameObject);

      StartPython();
    }

    private void OnDestroy(){
      try {
        if (_proc != null && !_proc.HasExited){
          _proc.Kill();
          _proc.Dispose();
        }
      }
      catch (Exception e){
        UnityEngine.Debug.LogWarning($"SelectHandsManager: kill process failed: {e.Message}");
      }
    }

    private void StartPython(){
      var psi = new ProcessStartInfo{
        FileName = "python3",
        Arguments = "../python/main_rule_based_hands_filter.py",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

      _proc = new Process();
      _proc.StartInfo = psi;
      _proc.OutputDataReceived += OnPythonOutput;
      _proc.BeginOutputReadLine();
    }

    private void OnPythonOutput(object sender, DataReceivedEventArgs e)
    {
      if(string.IsNullOrWhiteSpace(e.Data))
        return;

      var raw = e.Data.Trim();

      if(!int.TryParse(raw, out var code))
        return;

      if(code == 10 || code == 20)
      {
        if(code != _currentHand)
        {
          _currentHand = code;
        }
      }
    }

    public int GetHandCode(){
      return _currentHand;
    }

    public bool IsLeftActive(){
      return _currentHand == 10;
    }

    public bool IsRightActive(){
      return _currentHand == 20;
    }
  }
}
        
