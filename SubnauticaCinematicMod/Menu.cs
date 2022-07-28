using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace SubnauticaCinematicMod
{
    public class CameraMenu : uGUI_InputGroup
    {
        private bool _showInventory = false;
        private static CameraMenu _instance;
 
        public static CameraMenu Instance
        {
            get
            {
                if (_instance != null) return _instance;
                GameObject go = new GameObject();
                go.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(go);
                go.AddComponent<SceneCleanerPreserve>();
                _instance = go.AddComponent<CameraMenu>();
                return _instance;
            }
        }

        private void OnGUI()
        {
            if (!_showInventory) return;
            GUI gui = new GUI();
        }
        
        override void Update()
        {
            if( Input.GetKeyDown( KeyCode.P ) && Player.main)
            {
                _showInventory = !_showInventory;
            }
        }
    }
}