using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.UI
{
    public class UIData
    {
        private Canvas _joystickCanvas;
        private Canvas _loadingCanvas;
        private Canvas _lockCanvas;
        private Canvas _winCanvas;
        
        private Button _attackBtn;
        public Button GrabBtn { get; }
        public Button RestartBtn { get; }
        
        private Image _loadingProgressBar;

        public UIData(
            Canvas joystickCanvas,
            Canvas loadingCanvas,
            Canvas lockCanvas,
            Canvas winCanvas,
            Button attackBtn,
            Button grabBtn,
            Button restartBtn,
            Image loadingProgressBar)
        {
            _joystickCanvas = joystickCanvas;
            _loadingCanvas = loadingCanvas;
            _lockCanvas = lockCanvas;
            _winCanvas = winCanvas;
            
            _attackBtn = attackBtn;
            GrabBtn = grabBtn;
            RestartBtn = restartBtn;
            
            _loadingProgressBar = loadingProgressBar;
        }
        
        public void UpdateProgress(float progress) => _loadingProgressBar.fillAmount = progress;
    
        public void ShowLoadingCanvas() => _loadingCanvas.enabled = true;

        public void HideLoadingCanvas() => _loadingCanvas.enabled = false;
    
        public void ShowJoystickCanvas() => _joystickCanvas.enabled = true;

        public void HideJoystickCanvas() => _joystickCanvas.enabled = false;
    
        public void ShowLockCanvas() => _lockCanvas.enabled = true;

        public void HideLockCanvas() => _lockCanvas.enabled = false;
    
        public void ShowWinCanvas() => _winCanvas.enabled = true;

        public void HideWinCanvas() => _winCanvas.enabled = false;
    
        public void ShowAttackControls() => _attackBtn.gameObject.SetActive(true);

        public void HideAttackControls() => _attackBtn.gameObject.SetActive(false);
    
        public void ShowGrabControls() => GrabBtn.gameObject.SetActive(true);

        public void HideGrabControls() => GrabBtn.gameObject.SetActive(false);
    
        public void EnableGrabControls() => GrabBtn.interactable = true;

        public void DisableGrabControls() => GrabBtn.interactable = false;
    }
}
