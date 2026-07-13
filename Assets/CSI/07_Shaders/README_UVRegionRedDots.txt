UV 영역 붉은 점 효과 (Shader + Shader Graph로 만드는 방법)
==========================================================

이 문서에서 제공하는 것
- 선택한 UV 영역 내부에 붉은 점이 나타나는 동작하는 Unlit 셰이더(CSI/UVRegionRedDots).
  - 영역은 마스크 텍스처의 색(RGB)로 정의합니다. 예: 한 구역은 순수 빨강(1,0,0), 다른 구역은 초록(0,1,0) 등으로 칠합니다.
  - 부동 소수(float) 파라미터 "_Density"(0..1)를 올릴수록 선택한 영역에 붉은 점이 더 많이 나타납니다.
- 런타임에서 머티리얼 속성을 쉽게 제어하는 보조 컴포넌트 RegionDotsController.cs.
- 순수 Shader Graph로 동일한 결과를 만드는 방법 단계.

머티리얼 프로퍼티
- _MainTex (Texture2D): 색 마스크 텍스처. 각 영역은 RGB의 고유한 색을 사용합니다.
- _RegionColor (Color): 활성화할 영역의 색(예: 마스크 텍스처의 빨강 영역이면 (1,0,0)).
- _Tolerance (0..1): 영역 색과의 허용 오차(0 = 완전 일치만 허용).
- _BackgroundColor (Color): 배경색.
- _DotColor (Color): 점의 색(기본 빨강).
- _Density (0..1): 점의 밀도. 값이 클수록 더 많은 셀에서 점이 활성화됩니다.
- _DotSize: 각 셀 내부에서 점의 반지름.
- _Tiling (x,y): UV 기준 셀 수. 값이 클수록 더 많은(더 작은) 점이 생깁니다.
- _Seed: 점 분포를 섞는 시드 값.

빠른 사용 방법
1) 셰이더 CSI/UVRegionRedDots로 머티리얼을 만듭니다.
2) _MainTex 에 영역 마스크 텍스처를 할당합니다. 마스크는 영역별로 순색을 사용하는 것이 좋습니다.
3) _RegionColor 를 활성화하려는 영역의 색으로 설정합니다(예: 순수 빨강 1,0,0).
4) _Density 를 올리면 해당 영역에 점이 더 많이 나타납니다.
5) 필요에 따라 _DotSize, _Tiling, _Seed, _Tolerance 를 조정합니다.

RegionDotsController (선택 사항)
- 임의의 Renderer(예: MeshRenderer, 또는 SpriteRenderer의 머티리얼 오버라이드)에 RegionDotsController를 추가합니다.
- 인스펙터에서 파라미터를 조정하세요. 스크립트는 MaterialPropertyBlock을 사용하여(인스턴스 안전) 값을 씁니다.
- 코드에서 SetDensity(value) 를 호출하여 밀도를 시간에 따라 애니메이션할 수 있습니다.

Shader Graph로 동일하게 제작하기(URP/HDRP/내장 파이프라인 SG)
1) Unlit Shader Graph를 생성합니다.
2) 아래 프로퍼티를 추가합니다:
   - MainTex (Texture2D)
   - RegionColor (Color)
   - Tolerance (Float 0..1)
   - BackgroundColor (Color)
   - DotColor (Color)
   - Density (Float 0..1)
   - DotSize (Float 0.001..0.2)
   - Tiling (Vector2)
   - Seed (Float)
3) 노드/로직:
   A) 영역 마스크
      - UV로 MainTex 를 샘플합니다.
      - 샘플한 RGB에서 RegionColor 를 빼고 길이(거리)를 구합니다.
      - 비교: Step 노드에서 Edge = Tolerance, In = Distance 를 사용하면 영역 내부는 1, 외부는 0이 됩니다(노드 구성에 따라 필요 시 반전).
   B) 점 그리드 & 셀별 랜덤
      - UV 에 Tiling(Vector2)을 곱합니다.
      - Seed 를 더합니다.
      - Floor()로 셀 좌표(Vector2)를 얻고, Frac()으로 셀 로컬 UV(0..1)를 얻습니다.
      - 랜덤/해시:
        • Simple Noise 또는 White Noise의 HLSL Custom Function을 쓰거나, 간단한 방법으로 대체할 수 있습니다.
          - Voronoi 노드를 사용하고, Cell Density를 Tiling 평균에 맞춘 뒤, 셀 ID의 노이즈 출력을 셀별 난수로 사용합니다.
        • 또는 Custom Function(HLSL)으로 frac(sin(dot(cell, float2(12.9898,78.233))) * 43758.5453) 를 사용합니다.
      - 밀도 게이트:
        • 난수와 Density 를 비교하여, 난수 > Density 이면 0(비활성), 그 외엔 1(활성). Step/Branch 노드로 게이트합니다.
   C) 점 형태
      - 셀 내부의 임의의 중심을 만듭니다(같은 셀 좌표에 대한 다른 해시로 0..1 범위의 2D 중심 생성).
      - 셀 로컬 UV와 중심 간의 거리 계산.
      - 원형 마스크: smoothstep(DotSize, DotSize*0.8, -distance + DotSize) 또는 step(distance, DotSize).
   D) 결합
      - DotMask = 활성 셀(밀도 게이트 결과) * 원형 마스크.
      - FinalMask = DotMask * RegionMask.
      - Lerp 의 T(Alpha)에 FinalMask 를 넣어 BackgroundColor 와 DotColor 사이를 보간합니다.
4) RegionDotsController 가 사용하는 프로퍼티 이름(_Density, _RegionColor, _Tolerance, _DotSize, _Tiling, _Seed)을 동일하게 노출하면, 이 스크립트를 Shader Graph 머티리얼에서도 그대로 사용할 수 있습니다.
5) Shader Graph 를 저장하고 머티리얼을 만들어 Renderer에 할당합니다.

팁
- 마스크의 영역 색이 완전히 순색이 아니라면 Tolerance 를 약간 올리세요(예: 0.05 ~ 0.15).
- 여러 영역을 동시에 쓰고 싶다면 머티리얼을 복제하거나, 타겟 색 배열을 받아 마스크를 합산하도록 그래프를 확장하세요.
- SpriteRenderer 를 사용할 경우, 스프라이트 지원과 렌더 큐가 적절한 머티리얼인지 확인하세요.
- URP/HDRP에서는 Shader Graph Target을 해당 파이프라인으로 설정하고 Unlit 마스터를 쓰면 일관된 결과를 얻을 수 있습니다.

참고
- Unity .shadergraph(JSON) 자산은 버전 간 호환성 문제가 잦습니다. HLSL 셰이더는 즉시 동작하며, 여기 제시한 Shader Graph 단계는 프로젝트에서 동일하게 만들 수 있는 노드 구성과 1:1로 대응됩니다.



==============================
Shader Graph 배선 가이드 (한국어, Sample Texture 2D 기준)
==============================
이 절차는 Built-in(Unlit Master), URP(Unlit), HDRP(Unlit)에서 동일하게 통합니다. RegionDotsController 스크립트와 연동하려면 Reference 이름을 정확히 일치시켜야 합니다: _MainTex, _RegionColor, _Tolerance, _BackgroundColor, _DotColor, _Density, _DotSize, _Tiling, _Seed.

A) 프로퍼티 (블랙보드에 생성)
- Texture2D: MainTex   (Reference: _MainTex)
- Color: RegionColor   (Reference: _RegionColor)
- Float: Tolerance     (Reference: _Tolerance, Range 0..1)
- Color: BackgroundColor (Reference: _BackgroundColor)
- Color: DotColor      (Reference: _DotColor)
- Float: Density       (Reference: _Density, Range 0..1)
- Float: DotSize       (Reference: _DotSize, Range 0.001..0.2)
- Vector2: Tiling      (Reference: _Tiling)
- Float: Seed          (Reference: _Seed)

B) 공통 입력 노드
- UV 노드(Input > Geometry > UV) 사용: UV0를 씁니다.
- Sample Texture 2D 노드: Texture 포트에 MainTex(블랙보드), UV 포트에 UV 노드를 연결합니다. 이 노드의 RGB 출력이 마스크 색입니다.

C) 영역 마스크 만들기
1) Subtract(Vector3) 노드: A ← Sample Texture 2D.RGB, B ← RegionColor.RGB.
2) Length 노드: Input ← Subtract 출력. 결과를 Distance로 둡니다.
3) Step 노드: Edge ← Tolerance, In ← Distance. 출력이 RegionMask(영역 내부 1, 외부 0)입니다. 만약 반대로 나오면 Edge와 In을 바꾸고 One Minus(1-값)로 보정하세요.

D) 점 그리드 좌표와 셀 데이터
1) Multiply(Vector2): A ← UV, B ← Tiling.
2) Add(Vector2): A ← 위 Multiply 출력, B ← Seed를 Vector2(Seed, Seed)로 변환해 연결. 결과를 GridUV로 명명.
3) Floor(Vector2): Input ← GridUV. 출력 = Cell(Vector2).
4) Fraction(Vector2): Input ← GridUV. 출력 = LocalUV(각 셀 내부 0..1 좌표).

E) 셀별 난수와 임의 중심(두 가지 방법)
방법 1) Custom Function(문자열) 사용(권장, 간단/빠름)
- Custom Function 노드 이름: Rand01
  • Input: cell(Vector2) / Output: r(Float)
  • Code:
    r = frac(sin(dot(cell, float2(12.9898,78.233))) * 43758.5453);
- Custom Function 노드 이름: Hash22
  • Input: cell(Vector2) / Output: center(Vector2)
  • Code:
    {
        float3 p3 = frac(float3(cell.x, cell.y, cell.x) * 0.1031);
        p3 += dot(p3, p3.yzx + 33.33);
        center = frac((p3.xx + p3.yz) * (p3.zy + p3.xz));
    }
- 연결: Rand01.cell ← Add(Cell, Seed) 결과. 출력 r → Random01.
- 연결: Hash22.cell ← 동일 입력(Add 결과). 출력 center → Center(0..1 범위의 임의 중심).

방법 2) Voronoi로 대체 (정확한 배선)
- Voronoi 노드(Procedural > Voronoi, Sampling: Euclidean)
  • UV ← GridUV
  • Cell Density ← (Tiling.x + Tiling.y) * 0.5   // Add(Vector1)로 더한 뒤 Multiply(0.5)
  • Angle Offset ← 0 (기본값)
- 무작위 값(Random01) 만들기:
  • ID(Output) → Fraction 노드 → Saturate → Random01(0..1)
    - 설명: Voronoi의 ID는 정수/연속 값일 수 있으므로 Fraction으로 0..1 범위 난수처럼 사용합니다.
- 셀 중심(Center) 선택:
  • Unity의 Voronoi 노드는 셀 로컬 UV를 직접 제공하지 않습니다. 따라서 Center는 아래 중 하나를 사용하세요.
    1) Hash22(cell) 방법(권장): D)에서 만든 Cell(= floor(GridUV))을 Hash22(Custom Function)에 넣어 0..1 범위의 2D Center를 얻습니다. Voronoi는 난수(ID)만 사용합니다.
    2) ID 기반 해시 대체: ID를 두 번 다른 상수로 해시해 2D를 만들 수도 있습니다(예: center = frac(float2(sin(ID*12.9898), sin(ID*78.233))*43758.5453);). 환경에 따라 밴딩이 보이면 방법 1을 쓰세요.

F) 밀도 게이트(셀 활성/비활성, 더 정확한 비교)
- 권장(부동소수 안전, 항상 0/1 실수 출력): Step 노드 사용
  • Step: Edge ← Random01, In ← Density
  • 출력 → ActiveCell (Density ≥ Random01 일 때 1, 그 외 0)
- 대안(비교 노드 사용): Less Or Equal(비교) 노드
  • A ← Random01, B ← Density  → 출력(Boolean)
  • Boolean을 실수 0/1로 변환하려면 Branch 노드로 True=1, False=0(또는 Multiply(Boolean,1))을 사용하세요. 일부 버전에선 Boolean이 색/수치 연산에 바로 쓰이지 않을 수 있습니다.
- 팁: Density는 0..1로 Clamp, Random01은 Fraction→Saturate 후 사용하면 가장 예측 가능한 결과를 얻습니다.

G) 셀 내부 점 형태
1) Center로 Hash22 출력(center)을 사용.
2) Distance 노드: A ← LocalUV, B ← Center. 출력 → Dist.
3) 부드러운 원형 마스크:
   - Multiply: A ← DotSize, B ← 0.8(상수). 이름: DotSizeSoft.
   - Negate: Input ← Dist. 이후 Add: A ← Negate 출력, B ← DotSize. 결과를 NegDistPlusSize로 둡니다.
   - Smoothstep: Edge1 ← DotSize, Edge2 ← DotSizeSoft, In ← NegDistPlusSize. 출력 → CircleMask(0..1).
   - 더 단순한 대안: Step(Edge ← Dist, In ← DotSize)로 하드 엣지 원을 만들 수도 있습니다.
4) Multiply: A ← ActiveCell, B ← CircleMask. 출력 → DotMask.

H) 영역 마스크와 색 결합
1) Multiply: A ← DotMask, B ← RegionMask. 출력 → FinalMask.
2) Lerp: A ← BackgroundColor.RGB, B ← DotColor.RGB, T ← FinalMask. 출력 → FinalColor(RGB).

I) 출력 노드 연결(파이프라인별)
- Built-in(Unlit Master):
  • Unlit Master.Color ← FinalColor
  • Unlit Master.Alpha ← 1 또는 FinalMask(투명 사용 시 Graph Inspector에서 Surface를 Transparent로)
  • Surface: Opaque 기본, 필요 시 Two-Sided.
- URP(Unlit):
  • Unlit Output.BaseColor ← FinalColor
  • Unlit Output.Alpha ← 1 또는 FinalMask(투명 사용 시 Graph Inspector에서 Surface = Transparent)
  • Surface: Opaque 기본, Two Sided는 필요 시.
- HDRP(Unlit):
  • Unlit Master.Color(버전에 따라 BaseColor) ← FinalColor
  • Unlit Master.Alpha/Opacity ← 1 또는 FinalMask(투명 사용 시 Surface Type = Transparent)
  • 필요하면 Emission에 FinalColor를 넣고 HDR 색/강도로 발광 느낌을 줄 수 있습니다.

J) 참고/팁
- Reference 이름의 언더스코어를 정확히 맞추어야 RegionDotsController가 런타임에서 값을 제어합니다.
- Seed가 Float일 때 Vector2/UV에 더하려면 (Seed, Seed) Vector2를 만들어 사용하세요.
- Tiling(x,y)이 커질수록 셀 수가 늘어 점은 더 작고 많아집니다.
- BackgroundColor, DotColor를 HDR로 설정하면 URP/HDRP에서 블룸을 얻을 수 있습니다.
- URP에서 SpriteRenderer를 쓸 때는 Sprite Unlit 마스터 또는 적합한 Render Queue/샘플링을 사용하세요.
