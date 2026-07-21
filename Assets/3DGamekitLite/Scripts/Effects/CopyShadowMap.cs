using UnityEngine;
using UnityEngine.Rendering;

namespace Gamekit3D
{
    [ExecuteInEditMode]
    public class CopyShadowMap : MonoBehaviour
    {
        CommandBuffer cb = null;

        void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            var light = GetComponent<Light>();
            if (light)
            {
                cb = new CommandBuffer();
                cb.name = "CopyShadowMap";
                cb.SetGlobalTexture("_DirectionalShadowMask", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
                light.AddCommandBuffer(UnityEngine.Rendering.LightEvent.AfterScreenspaceMask, cb);
            }
        }

        void OnDisable()
        {
            if (!Application.isPlaying)
                return;

            var light = GetComponent<Light>();
            if (light)
            {
                light.RemoveCommandBuffer(UnityEngine.Rendering.LightEvent.AfterScreenspaceMask, cb);
            }
        }

    }
}