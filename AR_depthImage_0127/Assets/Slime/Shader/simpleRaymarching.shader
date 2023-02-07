Shader "Custom/simpleRaymarching"
{
	Properties
	{
		_Radius("Radius", Range(0.0,1.0)) = 0.3
		_BlurShadow("BlurShadow", Range(0.0,50.0)) = 16.0
		_Speed("Speed", Range(0.0,10.0)) = 2.0
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "LightMode" = "ForwardBase"}
		LOD 100

		Pass
		{
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fog


			#include "UnityCG.cginc"
		//���C�e�B���O�̌ŗL�֐����g������
		#include "Lighting.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 pos : POSITION1;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		//���[�J�������[���h���W�ɕϊ�
		o.pos = mul(unity_ObjectToWorld, v.vertex);
		o.uv = v.uv;
		return o;
	}

	float _Radius,_BlurShadow,_Speed;
	//float _Radius;

	//���̋����֐�
	float sphere(float3 pos)
	{
		return length(pos) - _Radius;
	}

	// ���ʂ̋����֐�
	//https://qiita.com/muripo_life/items/074f69a5f0bac74e71e6
	float plane(float3 pos)
	{
		float4 n = float4(0., 0.8, 0, 1);
		return dot(pos, n.xyz) + n.w;
	}

	float getDist(float3 pos) {
		return min(plane(pos), sphere(pos));
	}

	// �@���̎Z�o
	// https://qiita.com/edo_m18/items/3d95c2309d6ad5a6ba55
	float3 getNormal(float3 pos) {
		float d = 0.001;
		return normalize(float3(
			getDist(pos + float3(d, 0, 0)) - getDist(pos + float3(-d, 0, 0)),
			getDist(pos + float3(0, d, 0)) - getDist(pos + float3(0, -d, 0)),
			getDist(pos + float3(0, 0, d)) - getDist(pos + float3(0, 0, -d))
			));
	}

	//�\�t�g�V���h�E�̎Z�o��
	//https://wgld.org/d/glsl/g020.html
	float genShadow(float3 pos, float3 lightDir) {
		float marchingDist = 0.0;
		float c = 0.001;
		float r = 1.0;
		float shadowCoef = 0.5;
		for (float t = 0.0; t < 50.0; t++) {
			marchingDist = getDist(pos + lightDir * c);
			if (marchingDist < 0.001) {
				return shadowCoef;
			}
			r = min(r, marchingDist * _BlurShadow / c);
			c += marchingDist;
		}
		return 1.0 - shadowCoef + r * shadowCoef;
	}


	fixed4 frag(v2f i) : SV_Target
	{

		// ���C�̏����ʒu
		float3 pos = i.pos.xyz;
		// ���C�̐i�s����
		float3 rayDir = normalize(pos.xyz - _WorldSpaceCameraPos);

		int StepNum = 30;

		for (int i = 0; i < StepNum; i++) {
			//�s�i���鋗��(���Ƃ̍ŒZ������)
			float marchingDist = getDist(pos);

			//0.001�ȉ��ɂȂ�����A�s�N�Z���𔒂œh���ď����I��
			if (marchingDist < 0.001) {

				//���C�e�B���O
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 normal = getNormal(pos);
				float3 lightColor = _LightColor0;

				//�\�t�g�V���h�E
				float shadow = genShadow(pos + normal * 0.001, lightDir);

				fixed4 col = fixed4(lightColor * max(dot(normal, lightDir), 0) * max(0.5, shadow), 1.0);

				//fixed4 col = fixed4(lightColor * max(dot(normal, lightDir), 0) , 1.0);
				col.rgb += fixed3(0.2f, 0.2f, 0.2f);//����
				return col;
			}
			//���C�̕����ɍs�i����
			pos.xyz += marchingDist * rayDir.xyz;
		}

		//StepNum��s�i���Ă��Փ˔��肪�Ȃ�������A�s�N�Z���𓧖��ɂ��ď����I��
		return 0;
	}
	ENDCG
}
	}
}