# NullRunner — Devlog

## Hedef
Octopath Traveler II referanslı HD-2D görsel dili + gerçek zamanlı Action RPG (sıra tabanlı değil).
Model C: eğik 3/4 kamera, yatay 3D dünya, billboard pixel karakter.

## Durum özeti (2026-11-17)
- Faz 0: TAMAM (3D mekan, kamera, karakter, sorting, test).
- Faz 1: Combat Prototype — scriptler yazıldı, sahneye bağlandı; düşman AI doğrulandı, oyuncu saldırı testi sürüyor.

## Faz 0 — HD-2D temel altyapı (TAMAM)
- Unity 6000.6.9f1, URP 17.6, Input System 1.20.1.
- Kamera: CameraFollow — 35° eğik, offset (0,7,6), Player'a parent, yumuşak takip.
- 3D dünya: Ground3D (Plane 60x60) + Ground sprite (cyberpunk doku, XZ).
- 3D mekan: 4 bina, 3 duvar, 2 neon şerit, 2 Point Light (cyan/magenta); WorldColor.cs runtime renk + emission.
- Karakter: Adventurer Female (48x64, 8 kare, 6 yön: idle/walk/jump/dash/death); 3x ölçek.
- Sprite sorting: Player 100, Ground -1 (karakter zeminin arkasına geçince kaybolmasın diye).
- Rigidbody: useGravity=false, constraints=FreezePositionY; y=0.9 clamp (ayaklar zemine otursun).
- Test aracı: ScreenShot.cs (Play'de 2.5 sn sonra Logs/screen.png).
- Doğrulandı: 3D perspektif + derinlik hissi var; karakter zeminde görünüyor; geri yürürken kaybolmuyor.

## Faz 1 — Action Combat Prototype (devam)
Yazılan dosyalar (Assets/Scripts/):
- CombatController.cs (Player): J hafif saldırı (3'lü kombo, 3. vuruş 1.8x), K ağır saldırı (yüksek hasar + knockback), L dodge (i-frame 0.3 sn), I block (hasar %80 azalır) / parry (blok ilk 0.18 sn — saldırgan stagger). Hitbox: menzil + yön (dot). VFX slash. movementLocked AdventurerController'a aktarılır.
- EnemyController.cs: devriye -> kovalama -> telegraph (0.55 sn, renk magenta) -> saldırı (oyuncu hasarı) -> recover; hasar alma, knockback, stagger, ölüm.
- CombatVFX.cs: statik slash/spark efektleri (geçici primitive + URP Lit materyal).
- CombatHUD.cs: OnGUI — kontrol ipuçları + oyuncu/düşman HP barları.
- AdventurerController.cs: movementLocked desteği (saldırı/dodge sırasında hareketsiz), y clamp.
- CombatTest.cs: otomatik test (light 1-2, heavy, oyuncu hasarı) — Main Camera'da.

Test sonuçları:
- Düşman AI: [Enemy] hazir -> Chase -> Telegraph -> [Combat] Player HP 88 -> 76 ... (hasar çalışıyor).
- Ekran görüntüsünde HUD + karakter + 3D mekan doğrulandı (vision ile).
- Oyuncu saldırı logları (CombatTest) henüz kesin doğrulanmadı — test çalışması sürüyor.

## Bilinen sorunlar / notlar
- Unity_Camera_Capture URP'de "Failed to render scene preview" -> ekran ScreenCapture ile alınıyor.
- Adventurer asset'inde saldırı animasyonu YOK (sadece idle/walk/jump/dash/death) -> saldırı görseli şimdilik VFX + sprite parlaması; gerçek attack sprite'ı eklenmeli.
- Düşman görseli placeholder (mor Capsule).
- CombatTest otomatik tetikleyici debug* alanları MCP ile set edilebiliyor ama Update tüketimi tutarsız; CombatTest script'i daha güvenilir.

## Sıradaki
1. CombatTest sonucunu doğrula; gerekirse CombatController'a hata düzelt.
2. Saldırı animasyonu placeholder'ı (attack sprite veya dash sheet ile proxy).
3. Toplayıcı: kombo penceresi, hit-stop, hit reaction görseli.
4. Birkaç farklı düşman türü + mini boss.

## Ortam
- MCP: relay_win.exe --mcp; POST http://localhost:9002/mcp/call-tool; tool'lar Unity_ManageScene/GameObject/Editor/Script/Asset/GetConsoleLogs.
- Senkron: C:/Users/student/hermes-sync (nowaljin/hermes-sync) — skills/memories/SOUL push.
