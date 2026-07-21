using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    /// <summary>
    /// Handles the respawn visual effect by fading in the character's alpha from 0 to 1.
    /// </summary>
    public class EllenSpawn : MonoBehaviour
    {
        [Tooltip("Duration in seconds for the fade-in effect")]
        public float effectTime = 1.0f;
        
        [Tooltip("Materials used during the respawn effect (should use URP/Unlit with Transparent surface type)")]
        public Material[] EllenRespawnMaterials;
        
        [Tooltip("Particle effect GameObject to activate during respawn")]
        public GameObject respawnParticles;
        
        protected Material[] m_EllenMaterials;
        protected MaterialPropertyBlock m_PropertyBlock;
        protected Renderer m_Renderer;
        protected float m_Timer;
        protected bool m_Started = false;

        const string k_BaseColorName = "_BaseColor";

        void Awake()
        {
            if (respawnParticles != null)
                respawnParticles.SetActive(false);
            
            m_PropertyBlock = new MaterialPropertyBlock();
            m_Renderer = GetComponentInChildren<Renderer>();
            
            if (m_Renderer != null)
            {
                // Store the original materials
                m_EllenMaterials = m_Renderer.materials;
            }

            m_Started = false;
            this.enabled = false;
        }

        void OnEnable()
        {
            m_Started = false;
            
            if (m_Renderer != null && EllenRespawnMaterials != null && EllenRespawnMaterials.Length > 0)
            {
                // Switch to respawn materials
                m_Renderer.materials = EllenRespawnMaterials;
                // Set initial alpha to 0
                SetAlpha(0f);
            }
            
            if (m_Renderer != null)
                m_Renderer.enabled = false;
        }

        /// <summary>
        /// Starts the respawn fade-in effect.
        /// </summary>
        public void StartEffect()
        {
            if (m_Renderer != null)
                m_Renderer.enabled = true;

            if (respawnParticles != null)
                respawnParticles.SetActive(true);
            
            m_Started = true;
            m_Timer = 0.0f;
        }

        void Update()
        {
            if (!m_Started || m_Renderer == null)
                return;

            // Calculate alpha from 0 to 1 over effectTime
            float alpha = Mathf.Clamp01(m_Timer / effectTime);
            SetAlpha(alpha);

            m_Timer += Time.deltaTime;

            // When fully faded in, switch back to original materials and disable this component
            if (alpha >= 1.0f)
            {
                if (m_EllenMaterials != null)
                    m_Renderer.materials = m_EllenMaterials;
                
                this.enabled = false;
            }
        }

        /// <summary>
        /// Sets the alpha value for all materials on the renderer using MaterialPropertyBlock.
        /// Preserves the original color from the material, only modifying the alpha channel.
        /// </summary>
        /// <param name="alpha">Alpha value from 0 (transparent) to 1 (opaque)</param>
        protected void SetAlpha(float alpha)
        {
            if (m_Renderer == null || m_Renderer.materials == null || m_Renderer.materials.Length == 0)
                return;

            m_Renderer.GetPropertyBlock(m_PropertyBlock);
            
            // Get the base color from the material itself (not the property block)
            // This preserves the color set in the material inspector
            Material currentMaterial = m_Renderer.materials[0];
            Color baseColor = currentMaterial != null && currentMaterial.HasProperty(k_BaseColorName)
                ? currentMaterial.GetColor(k_BaseColorName)
                : Color.white;
            
            // Only modify the alpha channel, keep RGB values from the material
            baseColor.a = alpha;
            m_PropertyBlock.SetColor(k_BaseColorName, baseColor);
            
            m_Renderer.SetPropertyBlock(m_PropertyBlock);
        }
    }
}