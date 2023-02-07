Shader "Unlit/Slime"
{
    Properties
    {
        _SmoothScale("SmoothScale", Range(1, 50)) = 4.0

    }
        SubShader
    {
        Tags
        {
            "Queue" = "Transparent" // 透過できるようにする
        }

        Pass
        {
            ZWrite On // 深度を書き込む
            Blend SrcAlpha OneMinusSrcAlpha // 透過できるようにする

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _SmoothScale;

        // 入力データ用の構造体
        struct input
        {
            float4 vertex : POSITION; // 頂点座標
        };

    // vertで計算してfragに渡す用の構造体
    struct v2f
    {
        float4 pos : POSITION1; // ピクセルワールド座標
        float3 viewDir: TEXCOORD1; //視線ベクトル
        float4 vertex : SV_POSITION; // 頂点座標
    };

    // 出力データ用の構造体
    struct output
    {
        float4 col: SV_Target; // ピクセル色
        float depth : SV_Depth; // 深度
    };

    // 入力 -> v2f
    v2f vert(const input v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        float4x4 modelMatrix = unity_ObjectToWorld;
        o.viewDir = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);
        o.pos = mul(unity_ObjectToWorld, v.vertex); // ローカル座標をワールド座標に変換
        return o;
    }

    // 球の距離関数
    float4 sphereDistanceFunction(float4 sphere, float3 pos)
    {
        return length(sphere.xyz - pos) - sphere.w;
    }

    // 深度計算
    inline float getDepth(float3 pos)
    {
        const float4 vpPos = mul(UNITY_MATRIX_VP, float4(pos, 1.0));

        float z = vpPos.z / vpPos.w;
        #if defined(SHADER_API_GLCORE) || \
                    defined(SHADER_API_OPENGL) || \
                    defined(SHADER_API_GLES) || \
                    defined(SHADER_API_GLES3)
                return z * 0.5 + 0.5;
                #else
                return z;
                #endif
            }

    #define MAX_SPHERE_COUNT 256 // 最大の球の個数
    float4 _Spheres[MAX_SPHERE_COUNT]; // 球の座標・半径を格納した配列
    int _SphereCount; // 処理する球の個数


    float smoothMin(float x1, float x2, float k)
    {
        return -log(exp(-k * x1) + exp(-k * x2)) / k;
    }

    // いずれかの球との最短距離を返す
    float getDistance(float3 pos)
    {
        float dist = 100000;
        for (int i = 0; i < _SphereCount; i++)
        {
            dist = smoothMin(dist, sphereDistanceFunction(_Spheres[i], pos), _SmoothScale);
        }
        return dist;
    }

    fixed3 _Colors[MAX_SPHERE_COUNT];

    fixed3 getColor(const float3 pos)
    {
        fixed3 color = fixed3(0, 0, 0);
        float weight = 0.01;
        for (int i = 0; i < _SphereCount; i++)
        {
            const float distinctness = 0.7;
            const float4 sphere = _Spheres[i];
            const float x = clamp((length(sphere.xyz - pos) - sphere.w) * distinctness, 0, 1);
            const float t = 1.0 - x * x * (3.0 - 2.0 * x);
            color += t * _Colors[i];
            weight += t;
        }
        color /= weight;
        return float4(color, 1);

    }

    float3 getNormal(const float3 pos)
    {
        float d = 0.001;
        return normalize(float3(
            getDistance(pos + float3(d, 0.0, 0.0)) - getDistance(pos + float3(-d, 0.0, 0.0)),
            getDistance(pos + float3(0.0, d, 0.0)) - getDistance(pos + float3(0.0, -d, 0.0)),
            getDistance(pos + float3(0.0, 0.0, d)) - getDistance(pos + float3(0.0, 0.0, -d))
            ));
    }

    // v2f -> 出力
    output frag(const v2f i)
    {
        output o;

        float3 pos = i.pos.xyz; // レイの座標（ピクセルのワールド座標で初期化）
        const float3 rayDir = normalize(pos.xyz - _WorldSpaceCameraPos); // レイの進行方向
        const half3 halfDir = normalize(_WorldSpaceLightPos0.xyz - rayDir); // ハーフベクトル



        for (int i = 0; i < 30; i++)
        {
             // posといずれかの球との最短距離
             float dist = getDistance(pos);

             // 距離が0.001以下になったら、色と深度を書き込んで処理終了
             if (dist < 0.01)
             {
                 //o.col = fixed4(0, 1, 0, 0.5); // 塗りつぶし
                 //fixed3 color = getColor(pos);
                 ////fixed3 color = getNormal(pos);

                 //half rim = 1.0 - abs(dot(i.viewDir, getNormal(pos)));
                 //fixed3 emission = pow(rim, 3) * 3;
                 //color.rgb += emission;


                 /*
                 解答例
                 */
                 fixed3 norm = getNormal(pos);
                 fixed3 baseColor = getColor(pos);

                 const float rimPower = 2;
                 const float rimRate = pow(1 - abs(dot(norm, rayDir)), rimPower);
                 const fixed3 rimColor = fixed3(1.5, 1.5, 1.5);

                 //fixed3 color = clamp(lerp(baseColor, rimColor, rimRate), 0, 1);
                 //float alpha = clamp(lerp(0.2, 4, rimRate), 0, 1);

                 float highlight = dot(norm, halfDir) > 0.99 ? 1 : 0;
                 fixed3 color = clamp(lerp(baseColor, rimColor, rimRate)+ highlight, 0, 1);
                 float alpha = clamp(lerp(0.2, 4, rimRate)+highlight, 0, 1);

                 o.col = fixed4(color, alpha);
                 o.depth = getDepth(pos); // 深度書き込み
                 return o;
             }

             // レイの方向に行進
             pos += dist * rayDir;
        }

             // 衝突判定がなかったら透明にする
             o.col = 0;
             o.depth = 0;
             return o;
    }
    ENDCG
        }
    }
}