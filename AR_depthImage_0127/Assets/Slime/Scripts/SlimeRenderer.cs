
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // 再生していない間も座標と半径が変化するように
public class SlimeRenderer : MonoBehaviour
{
    [SerializeField] private Material material; // スライム用のマテリアル
    public int slimeNum;
    public GameObject slime;

    private const int MaxSphereCount = 600; // 球の最大個数（シェーダー側と合わせる）
    private readonly Vector4[] _spheres = new Vector4[MaxSphereCount];
    private SphereCollider[] _colliders;
    private Vector4[] _colors = new Vector4[MaxSphereCount];

    private void Start()
    {
        GenerateSlime(slimeNum);

        // 子のSphereColliderをすべて取得
        _colliders = GetComponentsInChildren<SphereCollider>();

        // シェーダー側の _SphereCount を更新
        material.SetInt("_SphereCount", _colliders.Length);

        for (var i=0; i<_colors.Length; i++)
        {
            _colors[i] = (Vector4)Random.ColorHSV(0, 1, 1, 1, 1, 1);
        }

        material.SetVectorArray("_Colors", _colors);
    }

    private void Update()
    {
        // 子のSphereColliderの分だけ、_spheres に中心座標と半径を入れていく
        for (var i = 0; i < _colliders.Length; i++)
        {
            var col = _colliders[i];
            if(col != null)
            {
                var t = col.transform;
                var center = t.position;
                var radius = t.lossyScale.x * col.radius;
                // 中心座標と半径を格納
                _spheres[i] = new Vector4(center.x, center.y, center.z, radius);
            }
        }

        // シェーダー側の _Spheres を更新
        material.SetVectorArray("_Spheres", _spheres);
    }

    private void GenerateSlime(int slimeNum)
    {
        for(int i=0; i<slimeNum; i++)
        {
            GameObject obj = Instantiate(slime, new Vector3(Random.Range(-3, 3), Random.Range(0, 5), Random.Range(-3, 3)), Quaternion.identity);
            obj.transform.parent = this.transform;
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Run stopped");
        foreach (Transform n in this.transform)
        {
            Destroy(gameObject.transform.Find("Slime(Clone)").gameObject);
        }
    }
}