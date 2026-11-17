# NullRunner — Gerçek Zamanlı Action RPG: Mimari & Prototip Planı

> **For Hermes:** Use subagent-driven-development skill to implement this plan task-by-task.

**Goal:** Octopath Traveler II'nin HD-2D görsel dilini koruyan, ancak **sıra tabanlı olmayan** gerçek zamanlı bir Action RPG vertical slice'ı kurmak.

**Architecture:** Unity 6000.6.9 + URP; 2.5D dünya (yatay zemin + billboard sprite'lar + eğik perspektif kamera); oynanış kodunu veri odaklı tutan (ScriptableObject) katmanlı mimari: Input → Player StateMachine/Combat → Hitbox/Hurtbox → Enemy AI → RPG progression → World/Quest → UI. Tek geliştirici için **C modeli** seçildi (HD-2D serbest hareket); savaş/RPG katmanları A/C'den bağımsız ortak.

**Tech Stack:** Unity 6000.6.9 (URP, 2D Renderer + 2D lights), C#, Unity Input System, Cinemachine (opsiyonel), Aseprite/itch.io sprite'ları, MCP (Hermes) ile sahne/script sürümü.

---

## 0. Mevcut durum (başlangıç noktası)

Proje: `C:\Users\student\Desktop\BozkurtIlker\Unity\NullRunner`

| Var olan | Durum |
|---|---|
| 2.5D dünya | Zemin XZ'de (GroundSprite), kamera 38° eğik + `CameraFollow` (lerp) |
| Karakter | `Player` + `AdventurerController` (top-down sprite, WASD XZ, idle/walk/jump/dash/death) |
| 2.5D sprite denemesi | `TestKnight` + `SpritePreview` — Hormelz 2.5D 8-yön sheet'i (256×256 frame) |
| Çevre | Central City (cyberpunk) zemin dokusu |
| MCP köprüsü | relay + Unity AI MCP (54 tool) ile sahne/script düzenleme çalışıyor |

**Karar:** top-down karakter sprite'ları yerine **Hormelz "8 Directional Knight" (CC0, 2.5D/HD2D, 8 yön, 256×256)** kullanılacak; savaş sistemi sıfırdan real-time olarak kurulacak.

---

## 1. Üç Action RPG savaş modeli — karşılaştırma

### A) Top-down / izometrik gerçek zamanlı ARPG
- **Kamera:** Sabit orthographic/izometrik, sabit açı; takip basit.
- **Hareket:** 2 eksen (X/Y); 4/8 yön sprite; 2D fizik.
- **Animasyon:** Top-down sprite (mevcut Adventurer) — hazır; yön sayısı az.
- **Savaş:** Hitbox 2D (OverlapCircle/Box), dodge/block/parry kolay; alan bazlı.
- **Teknik zorluk:** Düşük. **Maliyet:** Düşük (1-2 haftalık prototip).
- **Octopath uyumu:** Düşük — aynı oynanışı verir, aynı görsel dili vermez.

### B) 2.5D side-scrolling ARPG
- **Kamera:** Yan görünüm, sabit Y; takip X ekseninde.
- **Hareket:** X ekseninde serbest (+ isteğe bağlı Z derinliği yok).
- **Animasyon:** Side-view sprite; platform/derinlik eklenirse kapsam büyür.
- **Savaş:** Tek eksen; mesafe/zıplama kritik; camera framing'e bağımlı.
- **Teknik zorluk:** Orta. **Maliyet:** Orta.
- **Octopath uyumu:** Kısmi — Octopath'un yan sahneleri andırır ama serbest hareket yok.

### C) HD-2D perspektifli, gerçek zamanlı, serbest hareketli ARPG ✅
- **Kamera:** Eğik 3/4 perspektif (~40-50°), FOV genişliği ile derinlik; **yumuşak takip** (mevcut `CameraFollow`).
- **Hareket:** XZ serbest hareket; 8 yönlü sprite (dir1-8: DownLeft…Down).
- **Animasyon:** 2.5D 8-yön sprite (Hormelz CC0 — hazır, 256×256); derinlik hissi; billboard.
- **Savaş:** XZ'de hitbox (`Physics.OverlapBox/Sphere`, layer'lar), dodge 8 yöne yayılır, telegraph perspektifte belirgin; **en Octopath-benzeri his.**
- **Teknik zorluk:** Orta — sprite derinlik sıralaması (`sortingOrder`), zemin/kamera dengesi, hitbox perspektif ayarsı.
- **Maliyet:** Orta (asset hazır; asıl iş savaş + AI + RPG derinliği).
- **Octopath uyumu:** Yüksek (görsel dil + oynanış bir bütün).

### Öneri (tek geliştirici)
**Model C.** Gerekçe:
1. **Hedef zaten Octopath:** C tek model hem görsel dili hem oynanışı aynı anda verir.
2. **Asset hazır:** Hormelz 8-yön 2.5D sprite'ları CC0; Central City çevre dokusu hazır.
3. **Mimari kaybı yok:** A'da kurulan savaş/RPG katmanları (hitbox, AI, skill, stats) C'de birebir çalışır; fark yalnızca **kamera + dünya düzlemi** katmanıdır. Geçici olarak A'da prototip sürmek de güvenli — C'ye taşınan şey kaybolmaz.
4. **Mevcut proje zaten C'ye yakın:** zemin XZ'de, kamera eğik; sadece sprite tasarımı (top-down→2.5D) ve savaş katmanı eksik.

---

## 2. Mimari

```
NullRunner/Assets/
  Scripts/
    Core/        GameManager.cs, EventBus.cs, SaveSystem.cs, SceneLoader.cs
    Input/       InputReader.cs (Input System → aksiyon event'leri)
    Player/      PlayerController.cs, PlayerStateMachine.cs, CombatController.cs,
                 DodgeHandler.cs, BlockParry.cs, SkillController.cs, Stats.cs
    Combat/      Hitbox.cs, Hurtbox.cs, DamageInfo.cs, AttackDataSO.cs,
                 ComboState.cs, Telegraph.cs, Knockback.cs, Stagger.cs
    Enemies/     EnemyBase.cs, EnemyAI.cs (FSM), EnemyDataSO.cs,
                 BossController.cs, TelegraphPatternSO.cs
    RPG/         LevelSystem.cs, SkillTree.cs, EquipmentSO.cs, Element.cs, Inventory.cs
    World/       Zone.cs, QuestSO.cs, NPC.cs, Interactable.cs, Spawner.cs
    Camera/      CameraFollow.cs (mevcut; genişletilecek)
    Data/        ScriptableObject'lar (weapons, skills, enemies, elements)
  Sprites/
    Knight25D/   Hormelz 8-yön animasyonları (Attack/Run/Walk/Idle/Jump/Die/…)
    CentralCity/ cyberpunk çevre
  Scenes/        VerticalSlice.unity
```

### Savaş katmanı (kalp)
- **AttackDataSO** (ScriptableObject): hasar, süre, hitbox boyutu/konumu, stagger/knockback, element, ses/efekt referansı.
- **Hitbox/Hurtbox:** `Hitbox` (saldırı anında aktif; `Physics.OverlapBox/Sphere` + layer `Enemy`/`Player`); `Hurtbox` (hasar alma; i-frame bayrağı).
- **Animasyon senkronu:** Animator event'leri → `AttackDataSO.Trigger()` → hitbox aç/kapa. Gecikme/süre SO'dan.
- **Kombo:** `ComboState` — input buffer (saldırı tuşu zaman damgalı kuyruk) + kombo penceresi; 3 vuruş zinciri, her vuruş farklı SO.
- **Dodge:** i-frame (hasar tamamen reddedilir), kısa mesafe, cooldown.
- **Block/Parry:** block = hasar azaltma (sürekli); parry = darbe anına ±100 ms pencere → düşman `Stagger`.
- **Telegraph:** `TelegraphPatternSO` — düşman saldırısından önce belirgin animasyon + efekt + bekleme süresi (oyuncu okur, kaçar/parry yapar).
- **Boss fazları:** `BossController` — sağlık eşikleri → yeni telegraph/desen + sahne efekti.

### Düşman AI
- Basit **FSM**: Idle → Patrol → Chase → Windup(Telegraph) → Attack → Recover → Stagger → Dead.
- Davranış verisi: `EnemyDataSO` (hız, menzil, saldırı desenleri, element zayıflığı).
- Yakın (melee) ve uzak (ranged) iki temel tür + 1 mini boss.

### RPG katmanı
- `Stats` (SO + hesap): level, exp, HP/MP, güç, zeka, savunma; ekipman bonusları.
- `SkillTree` (SO + UI): aktif/pasif, element, cooldown/mana.
- `Element`: ateş/buz/gök/gölge…; düşman zayıflığı çarpanı.
- Envanter + ekipman yuvaları; build çeşitliliği.

### Kayıt
- JSON dosyası (`Application.persistentDataPath`): player stats/ekipman/skill/quest durumu + checkpoint.

### UI (uGUI)
- HUD: HP bar, mana/enerji, cooldown ikonları, hedef göstergesi, mini quest tracker.
- Menüler: envanter, skill tree, görevler; context'te etkileşim promptu.

### HD-2D atmosfer (Octopath hissi)
- URP 2D Renderer + 2D light'lar; bloom/vignette post-process; sprite'lara glow.
- Kamera açısı sabit (3/4); dünya ışığı (gece neon) + karakter etrafı ışık.

---

## 3. Teknik araştırma (10 madde) — Unity çözümleri

| # | Konu | Yaklaşım / API |
|---|---|---|
| 1 | Gerçek zamanlı hareket | Unity Input System + `CharacterController` veya Rigidbody (XZ); mevcut `AdventurerController` temeli |
| 2 | Saldırı + kombo | Input buffer + `ComboState`; animasyon event'leri; `AttackDataSO` |
| 3 | Dodge / block / parry | i-frame bayrağı; parry penceresi (süre toleranslı); `Stagger` durumu |
| 4 | Animasyon state machine | Animator Controller + blend (8 yön); aksiyon durumları (idle/attack/dodge/hit/dead) |
| 5 | Hitbox / hurtbox | `Physics.OverlapBox/Sphere`, layer'lar, `OnTriggerEnter` yerine event-tabanlı tarama |
| 6 | Düşman AI + combat behavior | FSM + `EnemyDataSO`; bölge/aggro menzili |
| 7 | Boss AI + fazlar | Sağlık eşikli faz geçişi + desen listesi |
| 8 | Skill + cooldown | `SkillController` + SO; mana/enerji; cooldown timer'ları |
| 9 | Gerçek zamanlı VFX | Particle System + sprite efektleri; hit flash, knockback, telegraph glow |
| 10 | Gamepad + klavye | Input System aksiyon haritası (iki cihaz, tek kod) |

---

## 4. Vertical Slice planı (bite-sized adımlar)

> Her adım: uygula → Play ile doğrula → konsol hatasız → sonraki.

**Faze 0 — Temel (1 gün)**
1. `PlayerController`'ı 2.5D'ye çevir: `AdventurerController` yerine 8-yön Hormelz sheet'ini yükle; hareket XZ (var), yön→animasyon eşlemesi (dir1..dir8). Doğrula: 8 yönde yürü, doğru animasyon.
2. Kamera: 44° eğik + yumuşak takip (mevcut `CameraFollow` ayarı: offset (0,9,-8), rot 44, smooth 5). Doğrula: takip yumuşak.

**Faze 1 — Savaş çekirdeği (2-3 gün)**
3. `AttackDataSO` + `Hitbox`/`Hurtbox` + `DamageInfo` kur. Tek normal saldırı (sol tık): animasyon event'inde hitbox aç → oyuncu hasar alır.
4. Hit reaction: knockback + kısa stagger + hit flash + ses. Doğrula: düşman vurulunca geri sekmez/flash.
5. Kombo: 3 vuruş zinciri + input buffer. Doğrula: art arda tıklamada zincir, boşta sıfırlanır.
6. Ağır saldırı: basılı tut → charge → büyük knockback. Doğrula.

**Faze 2 — Kaçınma (1-2 gün)**
7. Dodge (tuş): i-frame + kısa dash + cooldown. Doğrula: dodge sırasında hasar almaz.
8. Block/parry: block = azaltma; parry penceresi = stagger. Doğrula: parry'de düşman sersemler.

**Faze 3 — Düşman (2-3 gün)**
9. `EnemyDataSO` + `EnemyAI` FSM (Idle/Chase/Windup/Attack/Recover/Stagger/Dead) + 1 melee düşman (telegraph'lı). Doğrula: yaklaşır, telegraph gösterir, saldırır.
10. 1 ranged düşman (telegraph + mermi). Doğrula.
11. HP bar + ölüm + exp.

**Faze 4 — Mini boss (2 gün)**
12. `BossController`: faz geçişleri + özel telegraph'lar + arena. Doğrula: 2 faz, ölüm animasyonu.

**Faze 5 — Dünya + RPG (2-3 gün)**
13. Keşif alanı: cyberpunk sokak (Central City tile'ları), çevre nesneleri, ışık. Doğrula: yürü, atmosfer.
14. NPC + görev: diyalog kutusu, quest tracker, "düşmanları temizle" görevi. Doğrula: kabul et, hedef, ödül.
15. `Stats`/`LevelSystem`: exp, level, stat artışı. Doğrula.
16. Envanter + ekipman (1-2 silah) + stat bonusu. Doğrula.
17. Skill tree (3-5 aktif/pasif) + cooldown/mana. Doğrula.

**Faze 6 — Atmosfer + polishe (2 gün)**
18. HD-2D ışık: neon ışıklar, karakter etrafı ışık, bloom/vignette. Doğrula: Octopath havası.
19. Savaş efektleri: hit spark, telegraph glow, death efekti. Doğrula.
20. HUD polishe + kayıt sistemi + başlangıç sahnesi. Doğrula: yükleniyor, kaydediliyor.

---

## 5. Riskler / açık sorular

| Risk | Önlem |
|---|---|
| Tek geliştirici kapsam | Slice'a dar: 1 melee + 1 ranged + 1 mini boss; skill tree 5 düğüm; tek bölge |
| Sprite/kamera dengesi (2.5D) | Kamera 40-50° bandında kal; `sortingOrder` ile derinlik; gerekirse kamera 45° sabitle |
| Hitbox perspektif yanılsaması | Hitbox'ları oyuncuyla aynı z düzleminde tut; debug görselleştir |
| Kayıt/yoğun SO | SO'larda minimal alan; save'de yalnızca gerekli alanlar |
| Hormelz sheet'lerinde "Attack" yön davranışı | dir1-8 eşlemesini kodda sabit tut; gerekirse yön birleştir |
| Performans | Sprite sayısı düşük (256×256); fizik_overlap sadece saldırı anında |

**Açık sorular:**
1. Kahraman modeli: knight (hazır) mu, yoksa kendi cyberpunk karakterin mi (ücretli/üretilmiş asset)?
2. Boss: mini boss tek mi, yoksa sonraki slice'da büyük boss mu?
3. Co-op/ikinci karakter şimdilik kapsam dışı mı?

---

## 6. Sıradaki adım

Kodlamaya **başlamadan önce** bu plan kesinleşsin:
- [ ] Model C onaylandı mı?
- [ ] Kahraman asset'i: Hormelz Knight (hazır) mi, yoksa yeni 2.5D cyberpunk karakter mi?
- [ ] Faze 0'a mi başlayalım (2.5D kamera + 8-yön karakter), yoksa önce savaş çekirdeği (Faze 1) mu?

Onay sonrası: **Faze 0** ile başlarız — mevcut `TestKnight` denemesini `Player`'a taşırız (8-yön 2.5D sprite + hareket), ardından Faze 1 (hitbox/kombo).

---
*Bu plan yalnızca tasarım belgesidir; kod değişikliği yapılmadı.*
