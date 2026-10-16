# 꿀벌 서바이벌 유니티 완성 가이드

이 문서는 제공된 14개 스크립트와 `게임_시스템_기획분석_보고서.docx`를 바탕으로, 비전공자도 유니티에서 게임을 완성할 수 있도록 순서대로 정리한 제작 안내서입니다.

완성 목표는 “꿀벌이 화면 가장자리에서 나타나는 개구리의 혀 공격을 피하면서 최대한 오래 생존하는 2D 탑뷰 회피 게임”입니다.

---

## 0. 먼저 이해하기

게임의 흐름은 아래와 같습니다.

1. 게임이 시작되면 꿀벌을 키보드 방향키 또는 WASD로 움직입니다.
2. 화면 바깥에서 개구리들이 나타납니다.
3. 개구리는 화면 안쪽 가장자리로 들어온 뒤 꿀벌을 향해 혀를 발사합니다.
4. 꿀벌이 혀에 맞으면 체력이 1 줄어듭니다.
5. 체력이 남아 있으면 잠깐 깜빡이며 무적 상태가 됩니다.
6. 꿀 아이템을 먹으면 체력이 회복됩니다.
7. 꽃 아이템 안에 들어가 있으면 안전지대처럼 무적 상태가 됩니다.
8. 체력이 0이 되면 게임오버가 되고, 생존 시간이 랭킹에 저장됩니다.
9. 스페이스, 엔터, 또는 재시작 버튼으로 다시 시작할 수 있습니다.

---

## 1. 스크립트 14개 역할 요약

| 스크립트 | 유니티에서 붙일 대상 | 역할 |
|---|---|---|
| `GameFlow.cs` | 빈 오브젝트 `GameManager` | 게임 시작, 게임오버, 재시작을 관리합니다. |
| `RankingManager.cs` | 보통 `GameManager`에 자동 추가됨 | 생존 시간 TOP 5를 저장합니다. |
| `GameTimer.cs` | 빈 오브젝트 `GameManager` 또는 `UIManager` | 생존 시간을 측정하고 UI에 표시합니다. |
| `CameraBounds2D.cs` | `Main Camera` | 카메라 화면의 월드 좌표 범위를 계산합니다. |
| `BeeController.cs` | 플레이어 꿀벌 | 꿀벌 이동과 혀 피격 감지를 담당합니다. |
| `BeeHealth.cs` | 플레이어 꿀벌 | 체력, 무적 시간, 깜빡임 효과를 담당합니다. |
| `HealthUI.cs` | 체력 UI 오브젝트 | 하트 UI를 체력에 맞게 켜고 끕니다. |
| `FrogWaveManager.cs` | 빈 오브젝트 `EnemyManager` | 개구리 웨이브를 생성합니다. |
| `FrogController.cs` | 개구리 프리팹 | 개구리 등장, 조준, 혀 발사를 담당합니다. |
| `TongueController.cs` | 개구리의 자식 오브젝트 `Tongue` | 혀의 선, 충돌 판정, 늘어남/줄어듦을 담당합니다. |
| `ItemSpawner.cs` | 빈 오브젝트 `ItemManager` | 꿀과 꽃 아이템을 랜덤 위치에 생성합니다. |
| `HoneyItem.cs` | 꿀 프리팹 | 꿀을 먹으면 체력을 회복합니다. |
| `FlowerItem.cs` | 꽃 프리팹 | 꽃 범위 안에 있으면 무적 상태가 됩니다. |
| `GameOverUI.cs` | 게임오버 UI 패널 관리자 | 최종 시간, 랭킹, 재시작 버튼을 표시합니다. |

주의: 사용자가 제공한 파일명 중 일부는 클래스명과 다를 수 있습니다.

| 현재 파일명 | 내부 클래스명 | 권장 수정 |
|---|---|---|
| `CameraBound2D.cs` | `CameraBounds2D` | 파일명을 `CameraBounds2D.cs`로 바꿉니다. |
| `HeartUI.cs` | `HealthUI` | 파일명을 `HealthUI.cs`로 바꾸거나, 클래스명을 `HeartUI`로 바꿉니다. 이 가이드에서는 `HealthUI.cs`를 기준으로 설명합니다. |

유니티는 `MonoBehaviour` 스크립트의 파일명과 클래스명이 다르면 컴포넌트를 제대로 붙이지 못할 수 있습니다.

---

## 2. 필요한 에셋 준비

아래 에셋은 직접 그려도 되고, 무료 에셋을 사용해도 됩니다.

### 필수 이미지 에셋

| 에셋 | 용도 | 권장 형태 |
|---|---|---|
| 꿀벌 스프라이트 | 플레이어 캐릭터 | 정면 또는 위에서 본 벌 이미지 |
| 개구리 스프라이트 | 적 캐릭터 | 위에서 본 개구리 이미지 |
| 꿀 스프라이트 | 회복 아이템 | 작은 꿀단지 또는 꿀방울 |
| 꽃 스프라이트 | 안전지대 아이템 | 꽃 또는 빛나는 꽃 |
| 하트 스프라이트 | 체력 UI | 빨간 하트 아이콘 3개 |
| 배경 이미지 | 플레이 화면 | 풀밭, 연못, 정원 등 |

### 선택 효과 에셋

| 에셋 | 용도 |
|---|---|
| 꿀 획득 파티클 | 꿀을 먹었을 때 반짝이는 효과 |
| 꽃 오라 이미지 | 꽃 주변의 안전지대 표시 |
| 혀 머티리얼 | `LineRenderer`로 그려지는 혀 색상 |
| 피격 사운드 | 꿀벌이 맞았을 때 |
| 회복 사운드 | 꿀을 먹었을 때 |
| 게임오버 사운드 | 체력 0이 되었을 때 |
| 배경음악 | 플레이 중 반복 재생 |

현재 스크립트에는 사운드 재생 코드가 없습니다. 사운드는 없어도 게임은 완성됩니다. 사운드까지 넣고 싶다면 별도 `AudioSource`와 재생 스크립트를 추가해야 합니다.

---

## 3. 유니티 프로젝트 기본 설정

1. 유니티에서 2D 프로젝트를 엽니다.
2. `Assets` 폴더 안에 다음 폴더를 만듭니다.

```text
Assets
├─ Scripts
├─ Sprites
├─ Prefabs
├─ Materials
├─ Effects
├─ Scenes
└─ Audio
```

3. 제공된 14개 스크립트를 `Assets/Scripts` 폴더에 넣습니다.
4. 파일명과 클래스명이 다른 스크립트를 먼저 고칩니다.
   - `CameraBound2D.cs` → `CameraBounds2D.cs`
   - `HeartUI.cs` → `HealthUI.cs`
5. 유니티 하단 `Console` 창에 빨간 오류가 없는지 확인합니다.

만약 `Rigidbody2D`에 `linearVelocity`가 없다는 오류가 뜨면 유니티 버전 차이입니다. `BeeController.cs`의 아래 줄을:

```csharp
rb.linearVelocity = input * moveSpeed;
```

아래처럼 바꿉니다.

```csharp
rb.velocity = input * moveSpeed;
```

---

## 4. 태그 만들기

스크립트가 태그를 사용하므로 반드시 만들어야 합니다.

1. 상단 메뉴에서 `Edit > Project Settings > Tags and Layers`로 들어갑니다.
2. `Tags`에 아래 태그를 추가합니다.

| 태그 이름 | 붙일 대상 |
|---|---|
| `Player` | 꿀벌 오브젝트 |
| `Tongue` | 혀 오브젝트. 단, `TongueController`가 자동으로 붙여주므로 직접 붙이지 않아도 됩니다. |

가장 중요한 것은 꿀벌에 `Player` 태그를 붙이는 것입니다. 꿀과 꽃 아이템이 `Player` 태그를 보고 꿀벌인지 판단합니다.

---

## 5. 카메라 만들기

`Main Camera`를 선택하고 아래처럼 설정합니다.

| 항목 | 값 |
|---|---|
| Projection | `Orthographic` |
| Size | `5` 정도로 시작 |
| Position | `X 0, Y 0, Z -10` |
| Background | 원하는 배경색 |

그리고 `Main Camera`에 `CameraBounds2D` 스크립트를 붙입니다.

이 스크립트는 화면의 왼쪽, 오른쪽, 위, 아래 끝을 계산합니다. 꿀벌이 화면 밖으로 나가지 못하게 하고, 개구리와 아이템이 화면 기준으로 생성되게 하는 핵심 기준점입니다.

---

## 6. 게임 관리자 만들기

빈 오브젝트를 만들고 이름을 `GameManager`로 바꿉니다.

`GameManager`에 아래 스크립트를 붙입니다.

1. `GameFlow`
2. `GameTimer`

`GameFlow`는 게임 전체의 상태를 관리합니다. 게임이 진행 중인지, 게임오버인지, 재시작해야 하는지를 판단합니다.

`GameTimer`는 생존 시간을 측정합니다. 나중에 UI 텍스트를 연결해야 합니다.

`RankingManager`는 직접 붙이지 않아도 됩니다. `GameFlow`가 시작될 때 `GameManager`에 `RankingManager`가 없으면 자동으로 추가합니다. 그래도 눈으로 확인하고 싶다면 직접 붙여도 괜찮습니다.

---

## 7. 플레이어 꿀벌 만들기

### 7-1. 꿀벌 오브젝트 만들기

1. Hierarchy 창에서 `2D Object > Sprite`를 만듭니다.
2. 이름을 `Bee`로 바꿉니다.
3. 꿀벌 스프라이트를 `Sprite Renderer`의 `Sprite` 칸에 넣습니다.
4. `Tag`를 `Player`로 바꿉니다.
5. 위치를 `X 0, Y 0, Z 0`으로 둡니다.

### 7-2. 꿀벌에 필요한 컴포넌트

`Bee`에 아래 컴포넌트를 붙입니다.

| 컴포넌트 | 설정 |
|---|---|
| `Rigidbody2D` | Gravity Scale `0`, Collision Detection `Continuous` 권장 |
| `CircleCollider2D` 또는 `CapsuleCollider2D` | `Is Trigger`는 꺼둡니다. 크기는 벌 몸통에 맞춥니다. |
| `BeeController` | 이동 담당 |
| `BeeHealth` | 체력 담당 |

`BeeController`는 `Rigidbody2D`를 자동으로 요구합니다. 그래도 직접 확인해두면 좋습니다.

### 7-3. BeeController 인스펙터 설정

| 항목 | 추천값 | 설명 |
|---|---:|---|
| Move Speed | `6` | 꿀벌 이동 속도입니다. 너무 빠르면 혀를 쉽게 피합니다. |
| Screen Margin | `0.25` | 화면 끝에서 조금 안쪽까지만 이동하게 하는 여백입니다. |

### 7-4. BeeHealth 인스펙터 설정

| 항목 | 추천값 | 설명 |
|---|---:|---|
| Max Health | `3` | 시작 체력입니다. 하트 3개와 맞춥니다. |
| Invincibility Duration | `1.5` | 맞은 뒤 무적 시간입니다. |
| Sprite Renderer | `Bee`의 Sprite Renderer | 피격 후 깜빡임에 필요합니다. 비워도 자동으로 찾습니다. |
| Blink Interval | `0.1` | 깜빡이는 속도입니다. |
| On Health Changed | 비워도 됨 | `HealthUI`가 코드로 자동 연결합니다. |
| On Death | 비워도 됨 | 체력이 0이면 코드에서 `GameFlow.GameOver()`를 호출합니다. |

주의: 현재 `BeeHealth.Heal()`은 최대 체력을 넘지 않게 막는 코드가 없습니다. 꿀을 많이 먹으면 내부 체력은 4 이상이 될 수 있지만, 하트 UI는 3개까지만 표시됩니다. 과제용으로는 크게 문제 없지만, 완성도를 높이려면 `Heal()` 안에서 `maxHealth`를 넘지 않게 고치는 것이 좋습니다.

---

## 8. 혀 오브젝트 만들기

혀는 개구리 프리팹의 자식으로 들어갑니다.

### 8-1. 혀 머티리얼 만들기

1. `Assets/Materials` 폴더에서 `Create > Material`을 선택합니다.
2. 이름을 `TongueMaterial`로 바꿉니다.
3. 색상을 분홍색 또는 빨간색 계열로 설정합니다.

2D 프로젝트에서 `LineRenderer`가 잘 안 보이면 머티리얼 Shader를 `Sprites/Default` 또는 유니티 버전에 맞는 2D용 Unlit Shader로 바꿔보세요.

### 8-2. Tongue 오브젝트 만들기

1. 빈 오브젝트를 만들고 이름을 `Tongue`로 바꿉니다.
2. 아래 컴포넌트를 붙입니다.

| 컴포넌트 | 설정 |
|---|---|
| `LineRenderer` | Position Count `2`, Use World Space 체크 |
| `EdgeCollider2D` | `Is Trigger` 체크. 코드에서도 자동 체크됩니다. |
| `Rigidbody2D` | Body Type `Kinematic` |
| `TongueController` | 혀 동작 담당 |

`LineRenderer` 설정 추천값:

| 항목 | 추천값 |
|---|---|
| Material | `TongueMaterial` |
| Sorting Layer | 캐릭터보다 위에 보이는 레이어 |
| Order in Layer | `5` 정도 |
| Start Color / End Color | 분홍색 또는 빨간색 |

혀의 실제 두께는 `FrogController`의 `Tongue Thickness` 값으로 조절됩니다.

---

## 9. 개구리 프리팹 만들기

### 9-1. 개구리 오브젝트 만들기

1. Hierarchy 창에서 `2D Object > Sprite`를 만듭니다.
2. 이름을 `Frog`로 바꿉니다.
3. 개구리 스프라이트를 넣습니다.
4. `FrogController` 스크립트를 붙입니다.

### 9-2. 개구리 자식으로 Tongue 넣기

1. 방금 만든 `Tongue` 오브젝트를 `Frog`의 자식으로 넣습니다.
2. `Tongue`의 위치는 `X 0, Y 0, Z 0`으로 둡니다.

### 9-3. 입 위치 만들기

더 정확하게 혀가 나가는 위치를 만들고 싶다면:

1. `Frog`의 자식으로 빈 오브젝트를 만듭니다.
2. 이름을 `Mouth`로 바꿉니다.
3. 개구리 입 위치에 맞게 살짝 이동합니다.

예를 들어 개구리 이미지가 위쪽을 보고 있다면 `Mouth`를 개구리 머리 앞쪽에 둡니다. 단, 이 게임에서는 개구리가 사방에서 나오므로 스프라이트 방향이 중요하지 않을 수도 있습니다.

### 9-4. FrogController 인스펙터 설정

| 항목 | 연결/추천값 | 설명 |
|---|---:|---|
| Tongue | 자식 `Tongue` | 비워도 자동으로 찾지만 직접 넣는 것이 안전합니다. |
| Mouth | 자식 `Mouth` 또는 `Frog` 자신 | 혀가 시작되는 위치입니다. |
| Extend Speed | `18` | 혀가 뻗는 속도입니다. |
| Retract Speed | `24` | 혀가 돌아오는 속도입니다. |
| Tongue Thickness | `0.18` | 혀 두께입니다. |
| Extra Past Screen | `1.5` | 혀가 화면 밖까지 더 나가게 하는 거리입니다. |

개구리가 혀를 쏘고 혀가 다시 줄어들면 `FrogController`가 개구리 오브젝트를 삭제합니다. 이 구조 덕분에 웨이브 매니저가 “이번 웨이브의 개구리가 모두 사라졌는지” 확인할 수 있습니다.

### 9-5. 프리팹으로 저장

`Frog` 오브젝트를 `Assets/Prefabs` 폴더로 드래그해서 `Frog.prefab`으로 만듭니다.

프리팹으로 만든 뒤 Hierarchy에 남아 있는 `Frog`는 삭제해도 됩니다. 실제 게임에서는 `FrogWaveManager`가 자동 생성합니다.

---

## 10. 개구리 웨이브 관리자 만들기

빈 오브젝트를 만들고 이름을 `EnemyManager`로 바꿉니다.

`EnemyManager`에 `FrogWaveManager` 스크립트를 붙입니다.

### FrogWaveManager 인스펙터 설정

| 항목 | 추천값 | 설명 |
|---|---:|---|
| Start Frogs | `3` | 첫 웨이브의 개구리 수입니다. |
| Frog Increment Per Wave | `1` | 웨이브가 지날 때마다 늘어나는 개구리 수입니다. |
| Wave Start Delay | `0.2` | 게임 시작 후 첫 웨이브까지 대기 시간입니다. |
| Inter Wave Delay | `0.3` | 웨이브 사이 쉬는 시간입니다. |
| Wait Before Enter | `0.3` | 개구리가 생성된 뒤 들어오기 전 대기 시간입니다. |
| Enter Duration | `0.25` | 화면 가장자리로 슬라이드하는 시간입니다. |
| Inner Margin | `0.5` | 개구리가 화면 안쪽으로 얼마나 들어올지 정합니다. |
| Fire Delay After Enter | `0` | 들어온 뒤 혀를 쏘기 전 대기 시간입니다. |
| Frog Prefab | `Frog.prefab` | 반드시 연결해야 합니다. |
| Edge Offset | `1` | 개구리가 화면 밖 어느 정도 거리에서 생성될지 정합니다. |
| Min Frog Spacing | `2` | 개구리끼리 너무 붙지 않게 하는 최소 거리입니다. |
| Place Attempts Per Frog | `30` | 위치 찾기 재시도 횟수입니다. |

난이도를 낮추고 싶다면:

- `Start Frogs`를 `1` 또는 `2`로 낮춥니다.
- `Frog Increment Per Wave`를 `0`으로 둡니다.
- `Extend Speed`를 낮춥니다.
- `Fire Delay After Enter`를 `0.3` 정도로 올립니다.

난이도를 높이고 싶다면:

- `Start Frogs`를 올립니다.
- `Frog Increment Per Wave`를 올립니다.
- `Extend Speed`를 올립니다.
- `Inter Wave Delay`를 줄입니다.

---

## 11. 꿀 회복 아이템 만들기

### 11-1. Honey 오브젝트 만들기

1. `2D Object > Sprite`를 만듭니다.
2. 이름을 `Honey`로 바꿉니다.
3. 꿀 스프라이트를 넣습니다.
4. `CircleCollider2D`를 붙입니다.
5. `CircleCollider2D`의 `Is Trigger`를 체크합니다.
6. `HoneyItem` 스크립트를 붙입니다.

### 11-2. HoneyItem 인스펙터 설정

| 항목 | 추천값 | 설명 |
|---|---:|---|
| Heal Amount | `1` | 먹었을 때 회복되는 체력입니다. |
| Rotation Speed | `90` | 꿀이 빙글빙글 도는 속도입니다. |
| Pickup Effect | 선택 | 먹었을 때 생성할 파티클 프리팹입니다. 없어도 됩니다. |

### 11-3. 꿀 획득 파티클 만들기

선택 사항입니다.

1. `Effects > Particle System`을 만듭니다.
2. 이름을 `HoneyPickupEffect`로 바꿉니다.
3. Duration은 `0.5`, Looping은 끕니다.
4. Start Lifetime은 `0.3`, Start Speed는 `1~2` 정도로 둡니다.
5. 노란색 계열로 설정합니다.
6. 프리팹으로 만든 뒤 `HoneyItem`의 `Pickup Effect`에 연결합니다.

### 11-4. 프리팹 저장

`Honey` 오브젝트를 `Assets/Prefabs` 폴더로 드래그해서 `Honey.prefab`으로 만듭니다.

---

## 12. 꽃 안전지대 아이템 만들기

### 12-1. Flower 오브젝트 만들기

1. `2D Object > Sprite`를 만듭니다.
2. 이름을 `Flower`로 바꿉니다.
3. 꽃 스프라이트를 넣습니다.
4. `CircleCollider2D`를 붙입니다.
5. `CircleCollider2D`의 `Is Trigger`를 체크합니다.
6. 콜라이더 크기를 꽃 주변의 안전지대 크기만큼 넓힙니다.
7. `FlowerItem` 스크립트를 붙입니다.

### 12-2. 꽃 오라 만들기

꽃이 “안전지대”라는 것을 플레이어가 알아보기 쉽게 하려면 오라를 추가하는 것이 좋습니다.

방법:

1. `Flower`의 자식으로 `2D Object > Sprite`를 만듭니다.
2. 이름을 `Aura`로 바꿉니다.
3. 원형 이미지나 흐릿한 빛 이미지를 넣습니다.
4. 꽃보다 크게 스케일을 키웁니다.
5. 색상 알파를 낮춰 반투명하게 만듭니다.
6. `FlowerItem`의 `Aura Renderer`에 이 `Aura`의 `SpriteRenderer`를 연결합니다.

### 12-3. FlowerItem 인스펙터 설정

| 항목 | 추천값 | 설명 |
|---|---:|---|
| Bob Speed | `2` | 위아래로 움직이는 속도입니다. |
| Bob Height | `0.2` | 위아래 움직임의 높이입니다. |
| Safe Color | 연노랑, 알파 낮게 | 오라 색입니다. |
| Aura Renderer | 자식 `Aura`의 SpriteRenderer | 안전지대 표시용입니다. |

플레이어가 꽃 범위 안에 있으면 `BeeHealth.ActivateInvincibility(999f)`가 호출됩니다. 즉, 꽃 안에 머무는 동안 거의 무한 무적입니다.

플레이어가 꽃에서 나가면 `BeeHealth.DeactivateInvincibility()`가 호출되어 무적이 바로 풀립니다.

### 12-4. 프리팹 저장

`Flower` 오브젝트를 `Assets/Prefabs` 폴더로 드래그해서 `Flower.prefab`으로 만듭니다.

---

## 13. 아이템 스포너 만들기

빈 오브젝트를 만들고 이름을 `ItemManager`로 바꿉니다.

`ItemManager`에 `ItemSpawner` 스크립트를 붙입니다.

### ItemSpawner 인스펙터 설정

| 항목 | 추천값 | 설명 |
|---|---:|---|
| Honey Prefab | `Honey.prefab` | 반드시 연결합니다. |
| Flower Prefab | `Flower.prefab` | 반드시 연결합니다. |
| Honey Spawn Interval | `15` | 꿀 기본 생성 간격입니다. |
| Honey Spawn Random Range | `5` | 꿀 생성 간격에 랜덤성을 줍니다. |
| Flower Spawn Interval | `20` | 꽃 기본 생성 간격입니다. |
| Flower Spawn Random Range | `8` | 꽃 생성 간격에 랜덤성을 줍니다. |
| Flower Lifetime | `15` | 꽃이 유지되는 시간입니다. |
| Spawn Margin | `0.5` | 화면 끝에서 어느 정도 안쪽에 생성할지 정합니다. |

예를 들어 꿀은 `15 ± 5초`, 꽃은 `20 ± 8초`마다 화면 안 랜덤 위치에 생성됩니다.

---

## 14. UI 만들기

UI는 크게 세 가지가 필요합니다.

1. 생존 시간 텍스트
2. 체력 하트 3개
3. 게임오버 패널과 랭킹 표시

TextMeshPro를 처음 쓰는 프로젝트라면, TMP 텍스트를 만들 때 유니티가 TMP Essential Resources를 가져오라고 할 수 있습니다. 안내창이 뜨면 Import를 누르면 됩니다.

### 14-1. Canvas 만들기

1. Hierarchy에서 `UI > Canvas`를 만듭니다.
2. Canvas 이름은 `Canvas`로 둡니다.
3. `Canvas Scaler`를 아래처럼 설정합니다.

| 항목 | 값 |
|---|---|
| UI Scale Mode | `Scale With Screen Size` |
| Reference Resolution | `1920 x 1080` |
| Match | `0.5` |

4. `EventSystem`이 자동 생성되었는지 확인합니다.

---

## 15. 생존 시간 UI 만들기

1. Canvas 안에 `UI > Text - TextMeshPro`를 만듭니다.
2. 이름을 `TimerText`로 바꿉니다.
3. 화면 상단 중앙 또는 왼쪽 상단에 배치합니다.
4. 텍스트 예시는 `00:00:00`으로 둡니다.
5. 글자 크기는 `40~60` 정도를 추천합니다.

그 다음 `GameManager`의 `GameTimer` 컴포넌트에서:

| 항목 | 연결 |
|---|---|
| Timer Text | `TimerText` |

이 연결을 빼먹으면 시간은 내부적으로 증가하지만 화면에 보이지 않습니다.

---

## 16. 체력 하트 UI 만들기

1. Canvas 안에 빈 오브젝트를 만들고 이름을 `HealthUI`로 바꿉니다.
2. `HealthUI`에 `Horizontal Layout Group`을 붙이면 하트 정렬이 편합니다.
3. `HealthUI` 자식으로 `UI > Image`를 3개 만듭니다.
4. 이름을 각각 `Heart1`, `Heart2`, `Heart3`으로 바꿉니다.
5. 각 Image의 Source Image에 하트 스프라이트를 넣습니다.
6. `HealthUI` 오브젝트에 `HealthUI` 스크립트를 붙입니다.

`HealthUI` 스크립트의 인스펙터에서:

| 배열 칸 | 연결 |
|---|---|
| Hp Images 0 | `Heart1` Image |
| Hp Images 1 | `Heart2` Image |
| Hp Images 2 | `Heart3` Image |

게임이 시작되면 `HealthUI`가 `BeeHealth.I`를 찾아서 체력 변경 이벤트를 자동으로 구독합니다.

---

## 17. 게임오버 UI 만들기

### 17-1. 패널 만들기

1. Canvas 안에 `UI > Panel`을 만듭니다.
2. 이름을 `HighscorePanel`로 바꿉니다.
3. 화면 중앙에 크게 배치합니다.
4. 배경은 반투명 검정 또는 어두운 색으로 설정합니다.

중요: `GameOverUI`는 이름으로 패널을 자동 탐색합니다. 아래 이름 중 하나를 사용하면 자동으로 찾기 쉽습니다.

- `highscorePanel`
- `HighscorePanel`
- `HighScorePanel`

가이드에서는 `HighscorePanel`을 사용합니다.

### 17-2. 최종 시간 텍스트 만들기

`HighscorePanel` 안에 TMP 텍스트를 만들고 이름을 `FinalTimeText`로 바꿉니다.

텍스트 예시:

```text
TIME  00:00:00
```

`GameOverUI`는 오브젝트 이름에 `time`이 들어간 TMP 텍스트를 최종 시간 텍스트로 자동 인식할 수 있습니다.

### 17-3. 랭킹 텍스트 5개 만들기

`HighscorePanel` 안에 TMP 텍스트를 5개 만듭니다.

이름은 아래처럼 합니다.

```text
Rank1Text
Rank2Text
Rank3Text
Rank4Text
Rank5Text
```

텍스트 예시는 아래처럼 넣어둡니다.

```text
Rank #1 : 00:00:00
Rank #2 : 00:00:00
Rank #3 : 00:00:00
Rank #4 : 00:00:00
Rank #5 : 00:00:00
```

`GameOverUI`는 이름이 `rank`로 시작하고 숫자가 들어간 텍스트를 자동으로 찾아 배열에 넣습니다.

### 17-4. 재시작 버튼 만들기

1. `HighscorePanel` 안에 `UI > Button - TextMeshPro`를 만듭니다.
2. 이름을 `RestartButton`으로 바꿉니다.
3. 버튼 글자는 `RESTART` 또는 `다시 시작`으로 바꿉니다.

버튼의 `On Click()`에 아래를 연결합니다.

1. `GameOverUI`가 붙은 오브젝트를 드래그합니다.
2. 함수는 `GameOverUI > RestartGame()`을 선택합니다.

### 17-5. GameOverUI 스크립트 붙이기

Canvas 또는 빈 오브젝트 `UIManager`를 만들고 `GameOverUI` 스크립트를 붙입니다.

인스펙터에 직접 연결하면 가장 안전합니다.

| 항목 | 연결 |
|---|---|
| Highscore Panel | `HighscorePanel` |
| Final Time Text | `FinalTimeText` |
| Rank Texts 0 | `Rank1Text` |
| Rank Texts 1 | `Rank2Text` |
| Rank Texts 2 | `Rank3Text` |
| Rank Texts 3 | `Rank4Text` |
| Rank Texts 4 | `Rank5Text` |
| Restart Button | `RestartButton` |

직접 연결하지 않아도 이름을 잘 맞추면 자동으로 찾지만, 처음 만드는 사람은 직접 연결하는 편이 실수가 적습니다.

---

## 18. 배경 만들기

간단한 방법:

1. `2D Object > Sprite`를 만듭니다.
2. 이름을 `Background`로 바꿉니다.
3. 풀밭, 정원, 연못 같은 배경 이미지를 넣습니다.
4. 위치는 `X 0, Y 0, Z 1` 또는 카메라 뒤쪽으로 둡니다.
5. `Sprite Renderer`의 `Order in Layer`를 `-10` 정도로 낮춥니다.
6. 화면 전체를 덮도록 스케일을 키웁니다.

배경에는 콜라이더가 필요 없습니다.

---

## 19. 물리 충돌 설정 확인

이 게임에서 중요한 충돌은 세 가지입니다.

| 충돌 | 필요한 설정 |
|---|---|
| 꿀벌과 혀 | 혀의 `EdgeCollider2D`가 `Is Trigger`, 꿀벌에는 `Rigidbody2D`와 2D Collider 필요 |
| 꿀벌과 꿀 | 꿀의 Collider가 `Is Trigger`, 꿀벌 태그가 `Player` |
| 꿀벌과 꽃 | 꽃의 Collider가 `Is Trigger`, 꿀벌 태그가 `Player` |

혀는 `TongueController`가 자동으로 `gameObject.tag = "Tongue"`를 실행합니다. 따라서 `Tongue` 태그가 프로젝트에 존재해야 합니다.

꿀벌은 반드시 `Player` 태그여야 합니다. 이걸 빼먹으면 꿀과 꽃이 작동하지 않습니다.

---

## 20. 씬 오브젝트 최종 구조 예시

Hierarchy는 대략 아래처럼 되면 좋습니다.

```text
Main Camera
  - CameraBounds2D

GameManager
  - GameFlow
  - GameTimer
  - RankingManager 또는 자동 추가

EnemyManager
  - FrogWaveManager

ItemManager
  - ItemSpawner

Bee
  - SpriteRenderer
  - Rigidbody2D
  - CircleCollider2D
  - BeeController
  - BeeHealth

Canvas
  - TimerText
  - HealthUI
    - Heart1
    - Heart2
    - Heart3
  - HighscorePanel
    - FinalTimeText
    - Rank1Text
    - Rank2Text
    - Rank3Text
    - Rank4Text
    - Rank5Text
    - RestartButton

EventSystem
Background
```

프리팹 폴더에는 아래가 있어야 합니다.

```text
Assets/Prefabs
├─ Frog.prefab
├─ Honey.prefab
└─ Flower.prefab
```

---

## 21. 꼭 연결해야 하는 인스펙터 항목 체크리스트

게임이 안 돌아갈 때 대부분 아래 연결이 빠져 있습니다.

### Main Camera

- `CameraBounds2D`가 붙어 있는가?
- 카메라 Projection이 `Orthographic`인가?

### GameManager

- `GameFlow`가 붙어 있는가?
- `GameTimer`가 붙어 있는가?
- `GameTimer > Timer Text`에 `TimerText`가 연결되어 있는가?

### Bee

- 태그가 `Player`인가?
- `Rigidbody2D`가 있는가?
- Gravity Scale이 `0`인가?
- 2D Collider가 있는가?
- `BeeController`가 있는가?
- `BeeHealth`가 있는가?
- `BeeHealth > Sprite Renderer`가 연결되어 있거나 같은 오브젝트에 `SpriteRenderer`가 있는가?

### EnemyManager

- `FrogWaveManager`가 붙어 있는가?
- `Frog Prefab`에 `Frog.prefab`이 연결되어 있는가?

### Frog.prefab

- `FrogController`가 붙어 있는가?
- 자식 `Tongue`가 있는가?
- `Tongue`에 `TongueController`, `LineRenderer`, `EdgeCollider2D`, `Rigidbody2D`가 있는가?
- `FrogController > Tongue`에 자식 `Tongue`가 연결되어 있는가?
- `FrogController > Mouth`가 연결되어 있는가? 비워도 되지만 연결 권장입니다.

### ItemManager

- `ItemSpawner`가 붙어 있는가?
- `Honey Prefab`이 연결되어 있는가?
- `Flower Prefab`이 연결되어 있는가?

### Honey.prefab

- `HoneyItem`이 붙어 있는가?
- Collider의 `Is Trigger`가 켜져 있는가?

### Flower.prefab

- `FlowerItem`이 붙어 있는가?
- Collider의 `Is Trigger`가 켜져 있는가?
- 안전지대 범위가 충분히 넓은가?
- `Aura Renderer`가 있으면 연결되어 있는가?

### UI

- `HealthUI`의 `Hp Images` 3칸에 하트 이미지가 연결되어 있는가?
- `GameOverUI`의 패널, 시간 텍스트, 랭킹 텍스트, 재시작 버튼이 연결되어 있는가?
- 재시작 버튼 `On Click()`에 `GameOverUI.RestartGame()`이 연결되어 있는가?

---

## 22. 플레이 테스트 순서

처음부터 모든 걸 한 번에 테스트하지 말고 아래 순서대로 확인하세요.

### 1단계: 꿀벌 이동 확인

1. 재생 버튼을 누릅니다.
2. 방향키 또는 WASD로 꿀벌이 움직이는지 확인합니다.
3. 화면 밖으로 나가지 않는지 확인합니다.

안 움직이면:

- `GameFlow`가 씬에 있는지 확인합니다.
- `Bee`에 `Rigidbody2D`가 있는지 확인합니다.
- Console에 오류가 있는지 확인합니다.

### 2단계: 타이머 확인

1. 게임 시작 후 시간이 올라가는지 확인합니다.
2. `00:00:00` 형식으로 보이는지 확인합니다.

안 보이면:

- `GameTimer > Timer Text` 연결을 확인합니다.

### 3단계: 개구리 생성 확인

1. 게임 시작 후 화면 바깥에서 개구리가 생기는지 봅니다.
2. 개구리가 화면 가장자리로 들어오는지 봅니다.
3. 혀가 꿀벌 방향으로 나가는지 봅니다.

안 나오면:

- `EnemyManager`가 있는지 확인합니다.
- `FrogWaveManager > Frog Prefab` 연결을 확인합니다.
- `Main Camera`에 `CameraBounds2D`가 붙어 있는지 확인합니다.

### 4단계: 피격 확인

1. 일부러 혀에 맞아봅니다.
2. 하트가 1개 줄어드는지 봅니다.
3. 꿀벌이 잠깐 깜빡이는지 봅니다.
4. 체력이 0이 되면 게임오버 패널이 뜨는지 봅니다.

안 맞으면:

- `Tongue` 태그가 프로젝트에 있는지 확인합니다.
- `Tongue`에 `EdgeCollider2D`가 있는지 확인합니다.
- `EdgeCollider2D > Is Trigger`가 켜져 있는지 확인합니다.
- `Bee`에 Collider와 `Rigidbody2D`가 있는지 확인합니다.

### 5단계: 아이템 확인

1. 일정 시간이 지나면 꿀과 꽃이 생기는지 봅니다.
2. 꿀을 먹으면 하트가 회복되는지 봅니다.
3. 꽃 안에 들어가면 혀에 맞아도 피해를 입지 않는지 봅니다.

안 생기면:

- `ItemManager`가 있는지 확인합니다.
- `ItemSpawner > Honey Prefab`, `Flower Prefab` 연결을 확인합니다.
- `Main Camera`에 `CameraBounds2D`가 있는지 확인합니다.

### 6단계: 게임오버와 랭킹 확인

1. 체력을 모두 잃어 게임오버를 만듭니다.
2. 최종 시간이 표시되는지 확인합니다.
3. 랭킹 1~5위가 표시되는지 확인합니다.
4. 재시작 버튼을 눌러 씬이 다시 시작되는지 확인합니다.
5. 스페이스 또는 엔터로도 재시작되는지 확인합니다.

---

## 23. 추천 밸런스 값

처음 완성할 때는 아래 값으로 시작하면 무난합니다.

### 쉬운 버전

| 항목 | 값 |
|---|---:|
| Bee Move Speed | `6.5` |
| Start Frogs | `1` |
| Frog Increment Per Wave | `1` |
| Extend Speed | `14` |
| Retract Speed | `20` |
| Fire Delay After Enter | `0.3` |
| Honey Spawn Interval | `12` |
| Flower Spawn Interval | `18` |

### 기본 버전

| 항목 | 값 |
|---|---:|
| Bee Move Speed | `6` |
| Start Frogs | `3` |
| Frog Increment Per Wave | `1` |
| Extend Speed | `18` |
| Retract Speed | `24` |
| Fire Delay After Enter | `0` |
| Honey Spawn Interval | `15` |
| Flower Spawn Interval | `20` |

### 어려운 버전

| 항목 | 값 |
|---|---:|
| Bee Move Speed | `5.5` |
| Start Frogs | `4` |
| Frog Increment Per Wave | `2` |
| Extend Speed | `22` |
| Retract Speed | `28` |
| Fire Delay After Enter | `0` |
| Honey Spawn Interval | `20` |
| Flower Spawn Interval | `25` |

---

## 24. 완성도를 올리는 연출 아이디어

스크립트를 크게 바꾸지 않고도 아래 요소를 추가하면 훨씬 게임처럼 보입니다.

### 꿀벌

- 피격 시 깜빡임은 이미 `BeeHealth`에 있습니다.
- 이동 방향에 따라 스프라이트를 살짝 기울이거나 뒤집으면 더 생동감이 납니다.
- 날갯짓 애니메이션 스프라이트를 추가하면 좋습니다.

### 개구리

- 개구리가 등장할 때 작은 점프 애니메이션을 추가합니다.
- 혀를 쏘기 직전에 입이 벌어지는 스프라이트를 넣으면 공격 예고가 됩니다.
- 개구리마다 색을 조금 다르게 하면 웨이브가 풍성해 보입니다.

### 혀

- `LineRenderer`의 색을 시작은 진한 빨강, 끝은 연한 분홍으로 만들 수 있습니다.
- 혀가 나갈 때 짧은 사운드를 추가하면 피드백이 좋아집니다.
- 혀가 너무 얇으면 `Tongue Thickness`를 올립니다.

### 꿀

- `HoneyItem`은 이미 회전합니다.
- 먹었을 때 파티클을 연결하면 보상이 더 명확해집니다.
- 회복 사운드를 추가하면 좋습니다.

### 꽃

- 오라 이미지를 반투명하게 깔면 안전지대라는 의미가 바로 전달됩니다.
- 꽃이 위아래로 움직이는 효과는 이미 들어 있습니다.
- 꽃이 사라지기 3초 전 깜빡이게 만들면 더 친절합니다. 현재 스크립트에는 없는 기능입니다.

### UI

- 게임오버 패널에 반투명 배경을 사용합니다.
- 현재 기록이 TOP 5에 들어갔을 때 해당 줄 색을 다르게 표시하면 좋습니다. 현재 스크립트에는 없는 기능입니다.
- 시작 화면과 간단한 조작 안내 화면을 추가하면 제출 완성도가 올라갑니다.

---

## 25. 자주 생기는 문제와 해결법

### Console에 “script class cannot be found” 같은 오류가 뜹니다

파일명과 클래스명이 다른 경우가 많습니다.

- `CameraBound2D.cs`를 `CameraBounds2D.cs`로 바꿉니다.
- `HeartUI.cs`를 `HealthUI.cs`로 바꿉니다.

### 꿀벌이 안 움직입니다

- `GameManager`에 `GameFlow`가 있는지 확인합니다.
- `GameFlow`가 없으면 `BeeController`가 게임 진행 중인지 판단하지 못합니다.
- `Bee`에 `Rigidbody2D`가 있는지 확인합니다.
- Console 오류를 먼저 해결합니다.

### 개구리가 안 나옵니다

- `EnemyManager`에 `FrogWaveManager`가 있는지 확인합니다.
- `Frog Prefab`이 비어 있지 않은지 확인합니다.
- `Main Camera`에 `CameraBounds2D`가 있는지 확인합니다.

### 혀가 안 보입니다

- `Tongue`에 `LineRenderer`가 있는지 확인합니다.
- `LineRenderer`에 Material이 있는지 확인합니다.
- Sorting Layer 또는 Order in Layer가 너무 낮지 않은지 확인합니다.
- `Tongue Thickness`를 `0.18`보다 크게 올려봅니다.

### 혀에 맞아도 체력이 안 줄어듭니다

- `Tongue` 태그가 프로젝트에 존재하는지 확인합니다.
- `TongueController`가 실행되면 자동으로 태그를 `Tongue`로 바꿉니다.
- `Bee`에 `BeeHealth`가 있는지 확인합니다.
- `Bee`에 2D Collider와 `Rigidbody2D`가 있는지 확인합니다.
- `Tongue`에 `EdgeCollider2D`가 있고 `Is Trigger`가 켜져 있는지 확인합니다.

### 꿀을 먹어도 회복되지 않습니다

- 꿀 프리팹에 `HoneyItem`이 있는지 확인합니다.
- 꿀 Collider의 `Is Trigger`가 켜져 있는지 확인합니다.
- 꿀벌 태그가 `Player`인지 확인합니다.
- 꿀벌에 `BeeHealth`가 있는지 확인합니다.

### 꽃 안에 들어가도 무적이 안 됩니다

- 꽃 프리팹에 `FlowerItem`이 있는지 확인합니다.
- 꽃 Collider의 `Is Trigger`가 켜져 있는지 확인합니다.
- 꽃 Collider 크기가 충분히 큰지 확인합니다.
- 꿀벌 태그가 `Player`인지 확인합니다.

### 게임오버 패널이 처음부터 보입니다

- `GameOverUI`가 `Awake()`에서 자동으로 숨기지만, 연결이 꼬였을 수 있습니다.
- `HighscorePanel`을 직접 `GameOverUI > Highscore Panel`에 연결합니다.
- 패널 이름을 `HighscorePanel`로 맞춥니다.

### 재시작 버튼이 작동하지 않습니다

- 버튼의 `On Click()`에 `GameOverUI.RestartGame()`이 연결되어 있는지 확인합니다.
- 씬이 Build Settings에 등록되어 있는지 확인합니다.
  - `File > Build Settings > Add Open Scenes`

---

## 26. 코드상 알아두면 좋은 주의점

### 1. `BeeHealth.Heal()`은 최대 체력 제한이 없습니다

현재 코드:

```csharp
currentHealth += amount;
OnHealthChanged?.Invoke(currentHealth);
```

더 안정적으로 만들려면:

```csharp
currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
OnHealthChanged?.Invoke(currentHealth);
```

### 2. 꽃 안전지대는 무적을 999초로 켭니다

꽃에 들어가면 거의 무한 무적을 켜고, 나가면 무적을 끕니다. 구조는 단순하고 과제용으로 좋습니다.

다만 피격 직후 무적 상태로 꽃에 들어갔다가 나올 때 기존 피격 무적까지 꺼질 수 있습니다. 복잡한 밸런스가 필요한 게임이라면 “피격 무적”과 “꽃 무적”을 분리하는 방식이 더 좋습니다.

### 3. 개구리 혀는 예측 사격이 아닙니다

혀는 발사 순간의 꿀벌 위치를 향합니다. 꿀벌의 미래 위치를 예측하지 않습니다.

그래서 난이도는 주로 개구리 수, 혀 속도, 웨이브 간격으로 조절합니다.

### 4. 랭킹은 로컬 저장입니다

`RankingManager`는 `PlayerPrefs`를 사용합니다. 즉, 같은 컴퓨터의 같은 게임에서만 저장됩니다. 온라인 랭킹이 아닙니다.

### 5. 모바일 입력은 없습니다

`BeeController`는 `Input.GetAxisRaw("Horizontal")`, `Input.GetAxisRaw("Vertical")`을 사용합니다. 기본적으로 키보드 방향키와 WASD에 적합합니다. 모바일 조이스틱을 쓰려면 별도 입력 코드를 추가해야 합니다.

---

## 27. 최종 제출 전 체크리스트

- [ ] 14개 스크립트가 `Assets/Scripts`에 들어가 있다.
- [ ] `CameraBounds2D.cs`, `HealthUI.cs` 파일명이 클래스명과 맞다.
- [ ] Console에 빨간 오류가 없다.
- [ ] `Player`, `Tongue` 태그가 존재한다.
- [ ] 꿀벌 오브젝트에 `Player` 태그가 붙어 있다.
- [ ] `Main Camera`에 `CameraBounds2D`가 붙어 있다.
- [ ] `GameManager`에 `GameFlow`, `GameTimer`가 붙어 있다.
- [ ] `EnemyManager`의 `FrogWaveManager`에 `Frog.prefab`이 연결되어 있다.
- [ ] `ItemManager`의 `ItemSpawner`에 `Honey.prefab`, `Flower.prefab`이 연결되어 있다.
- [ ] `Frog.prefab` 안에 `Tongue` 자식 오브젝트가 있다.
- [ ] `Tongue`에 `LineRenderer`, `EdgeCollider2D`, `Rigidbody2D`, `TongueController`가 있다.
- [ ] `Honey.prefab`의 Collider는 `Is Trigger`가 켜져 있다.
- [ ] `Flower.prefab`의 Collider는 `Is Trigger`가 켜져 있다.
- [ ] `GameTimer`에 `TimerText`가 연결되어 있다.
- [ ] `HealthUI`에 하트 이미지 3개가 연결되어 있다.
- [ ] `GameOverUI`에 게임오버 패널, 최종 시간 텍스트, 랭킹 텍스트, 재시작 버튼이 연결되어 있다.
- [ ] 재시작 버튼의 `On Click()`에 `RestartGame()`이 연결되어 있다.
- [ ] 씬이 `Build Settings`에 등록되어 있다.
- [ ] 플레이어 이동, 혀 피격, 체력 감소, 꿀 회복, 꽃 무적, 게임오버, 랭킹, 재시작을 모두 테스트했다.

---

## 28. 추천 제작 순서 한 줄 요약

1. 스크립트를 `Assets/Scripts`에 넣고 파일명 오류를 고칩니다.
2. 카메라에 `CameraBounds2D`를 붙입니다.
3. `GameManager`를 만들고 `GameFlow`, `GameTimer`를 붙입니다.
4. 꿀벌을 만들고 `BeeController`, `BeeHealth`를 붙입니다.
5. 개구리와 혀를 만든 뒤 `Frog.prefab`으로 저장합니다.
6. `EnemyManager`를 만들고 `FrogWaveManager`에 개구리 프리팹을 연결합니다.
7. 꿀과 꽃을 만든 뒤 각각 프리팹으로 저장합니다.
8. `ItemManager`를 만들고 `ItemSpawner`에 꿀/꽃 프리팹을 연결합니다.
9. 타이머, 하트, 게임오버 패널 UI를 만듭니다.
10. 인스펙터 연결을 모두 확인하고 플레이 테스트합니다.

이 순서대로 진행하면 “움직이는 꿀벌 → 공격하는 개구리 → 피격과 체력 → 아이템 → UI와 랭킹” 순서로 기능이 하나씩 완성됩니다.
