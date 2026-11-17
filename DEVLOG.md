# NullRunner — Devlog

## Hedef
Octopath Traveler II referanslı **HD-2D görsel dili** + **gerçek zamanlı Action RPG** (sıra tabanlı değil). Model C: eğik 3/4 kamera, yatay dünya, billboard 8-yön karakter.

## 2026-11-17

### Faz 0 — HD-2D temel altyapı (başladı)
- [x] Mevcut proje incelendi: Unity 6000.6.9f1, URP 17.6, Input System 1.20.1.
- [x] Mevcut scriptler: AdventurerController (top-down), CameraFollow (44°, lerp), GroundSprite, SpritePreview (2.5D deneme).
- [ ] 2.5D karakter: Hormelz Knight (CC0, 8 yön, 256×256) → Player25DController.
- [ ] Kamera 3/4 eğik + yumuşak takip.
- [ ] Sprite sorting + gölge + ölçek.
