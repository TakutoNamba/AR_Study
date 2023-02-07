
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // �Đ����Ă��Ȃ��Ԃ����W�Ɣ��a���ω�����悤��
public class SlimeRenderer : MonoBehaviour
{
    [SerializeField] private Material material; // �X���C���p�̃}�e���A��
    public int slimeNum;
    public GameObject slime;

    private const int MaxSphereCount = 600; // ���̍ő���i�V�F�[�_�[���ƍ��킹��j
    private readonly Vector4[] _spheres = new Vector4[MaxSphereCount];
    private SphereCollider[] _colliders;
    private Vector4[] _colors = new Vector4[MaxSphereCount];

    private void Start()
    {
        GenerateSlime(slimeNum);

        // �q��SphereCollider�����ׂĎ擾
        _colliders = GetComponentsInChildren<SphereCollider>();

        // �V�F�[�_�[���� _SphereCount ���X�V
        material.SetInt("_SphereCount", _colliders.Length);

        for (var i=0; i<_colors.Length; i++)
        {
            _colors[i] = (Vector4)Random.ColorHSV(0, 1, 1, 1, 1, 1);
        }

        material.SetVectorArray("_Colors", _colors);
    }

    private void Update()
    {
        // �q��SphereCollider�̕������A_spheres �ɒ��S���W�Ɣ��a�����Ă���
        for (var i = 0; i < _colliders.Length; i++)
        {
            var col = _colliders[i];
            if(col != null)
            {
                var t = col.transform;
                var center = t.position;
                var radius = t.lossyScale.x * col.radius;
                // ���S���W�Ɣ��a���i�[
                _spheres[i] = new Vector4(center.x, center.y, center.z, radius);
            }
        }

        // �V�F�[�_�[���� _Spheres ���X�V
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