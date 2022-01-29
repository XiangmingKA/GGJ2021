using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    public KeyCode mirrorCameraButton;

    
    void Start () {
    
    
    }
    void MirrorFlipCamera(Camera _camera) {
    
        Matrix4x4 mat = _camera.projectionMatrix;
    
        mat *= Matrix4x4.Scale(new Vector3(1, -1, 1));
    
        _camera.projectionMatrix = mat;
        
        GL.invertCulling = !GL.invertCulling;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(mirrorCameraButton))
        {
            MirrorFlipCamera(this.gameObject.GetComponent<Camera>());
        }
    }

    // private Camera _camera;
    // public bool flipHorizontal;
    // void Awake () {
    //     _camera = GetComponent<Camera>();
    // }
    // void OnPreCull() {
    //     _camera.ResetWorldToCameraMatrix();
    //     _camera.ResetProjectionMatrix();
    //     Vector3 scale = new Vector3(flipHorizontal ? -1 : 1, 1, 1);
    //     _camera.projectionMatrix *= Matrix4x4.Scale(scale);
    // }
    // void OnPreRender () {
    //     GL.invertCulling = flipHorizontal;
    // }
    //  
    // void OnPostRender () {
    //     GL.invertCulling = false;
    // }
}
