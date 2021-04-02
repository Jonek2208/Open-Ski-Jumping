using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    [ExecuteInEditMode]
    public class UISkinControllerxd : MonoBehaviour
    {
        [SerializeField] private Selectable selectable;
        [SerializeField] private SelectableSkinData skinData;
        

        protected void OnSkinUI()
        {
            selectable.colors = skinData.colors;
        }

        protected void Awake()
        {
            OnSkinUI();
        }

        protected void Update()
        {
            if (Application.isEditor)
            {
                OnSkinUI();
            }
        }
    }
}