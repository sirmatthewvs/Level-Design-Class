using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    [ExecuteInEditMode]
    public class SunSkybox : MonoBehaviour
    {
        public Material skyboxMaterial;
        int sunDirId, sunColorId;
        Light sun;

        void Awake()
        {
            if (!Application.isPlaying) return;
            sun = GetComponent<Light>();
            sunDirId = Shader.PropertyToID("_SunDirection");
            sunColorId = Shader.PropertyToID("_SunColor");
        }

        void Update()
        {
            if (!Application.isPlaying) return;
            if (skyboxMaterial)
            {
                skyboxMaterial.SetVector(sunDirId, -transform.forward.normalized);
                skyboxMaterial.SetColor(sunColorId, sun.color);
            }
        }
    }
}