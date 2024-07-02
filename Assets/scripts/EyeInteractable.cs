using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    [field:SerializeField]public bool IsHovered { get; private set; }
    [field:SerializeField]public bool IsSelected { get; private set; }
    [SerializeField] private UnityEvent<GameObject> OnObjectHover;
    [SerializeField] private UnityEvent<GameObject> OnObjectSelected;
    [SerializeField] private Material OnHoverActiveMaterial;
    [SerializeField] private Material OnSelectActiveMaterial;
    [SerializeField] private Material onIdleMaterial;
    private MeshRenderer meshRenderer;
    private Transform originalAnchor;
    private TextMeshPro statusText;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        statusText = GetComponentInChildren<TextMeshPro>();
        originalAnchor = transform.parent;
    }

    public void Hover(bool state)
    {
        IsHovered = state;
    }

    public void Select(bool state, Transform anchor = null)
    {
        IsSelected = state;
        if(anchor)transform.SetParent(anchor);
        if(!IsSelected)transform.SetParent(originalAnchor);
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (IsHovered)
        {
            meshRenderer.material = OnHoverActiveMaterial;
            OnObjectHover?.Invoke(gameObject);
            
        }

        if (IsSelected)
        {
            OnObjectSelected?.Invoke(gameObject);
            meshRenderer.material = OnSelectActiveMaterial;
        }
        if(!IsHovered&&!IsSelected)
        {
            meshRenderer.material = onIdleMaterial;
        }
        
    }
}
