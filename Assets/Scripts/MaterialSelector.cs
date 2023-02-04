using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSelector : MonoBehaviour
{
    [SerializeField] private List<Material> _materials;
    
    private int _materialIdx = 0;
    private MeshRenderer _renderer;

    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        if(_materials.Count > 0)
            _renderer.material = _materials[_materialIdx];
    }

    public void SetMaterial(int index)
    {
        if(index < _materials.Count && index >= 0)
            _renderer.material = _materials[index];
    }
}
