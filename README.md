# Pirate's Passage — Zamanda Yolculuk TPS Oyunu

Unity ile geliştirilmiş, zamanda yolculuk temalı üçüncü şahıs nişancı (TPS) oyun prototipi. Oyuncu, geçmiş ve gelecek arasında yolculuk yapan bir korsan karakterini kontrol ederek gizli bir hazineyi ve gizemli bir geçidi keşfetmeye çalışır.

> Akademik proje olarak geliştirilmiştir (Mart – Haziran 2026)

---

## Hakkında

Pirate's Passage, iki farklı zaman diliminde geçen hikaye odaklı bir TPS oyunudur. Oyuncu; savaş, keşif ve bulmaca çözme mekaniklerini bir arada kullanarak anahtarlar toplar, altın biriktirir ve düşmanlarla savaşarak son portala ulaşmayı hedefler.

## Oyun Özellikleri

- **Üçüncü Şahıs Savaş Sistemi** — Nişan alma, ateş etme, raycast tabanlı atış, namlu parlaması ve hasar geri bildirimi
- **Silah Sistemi** — Silah çekme/kılıflama (Q), FOV yakınlaştırmalı nişan alma (Sağ Tık), ateş etme (Sol Tık)
- **Düşman Yapay Zekası** — NavMesh tabanlı devriye, takip ve saldırı davranışları
- **Envanter ve Toplanabilir Eşyalar** — Altın toplama, kilitli kapılar için anahtar bulma, can yenileme
- **Arayüz Sistemi** — Can barı (HUD), nişangah, envanter gösterimi, duraklat menüsü, kazanma ekranı ve animasyonlu ana menü
- **Portal ve Seviye Geçişi** — Sahne geçişleri için portal sistemi ve oyun bitiş sekansı
- **Ses Sistemi** — Arka plan müziği, silah sesi, altın toplama efekti, yumruk etkisi ve rüzgar ortam sesi
- **Grafik Ayarları** — Farklı donanımlar için oyun içi grafik kalitesi yöneticisi

## Sahneler

| Sahne | Açıklama |
|-------|----------|
| `MainMenu` | Animasyonlu ana menü, stilize butonlar ve ayarlar |
| `Level_Past` | Geçmiş dönemde geçen ana oynanış seviyesi |

## Proje Yapısı

```
Assets/
├── Animations/          # Karakter ve düşman animasyon klipleri (.fbx)
├── Audio/               # Ses efektleri ve müzikler
├── Materials/           # Özel materyaller (kum, gelecek teması)
├── Prefabs/             # Yeniden kullanılabilir oyun nesneleri
├── Scenes/              # Oyun sahneleri (MainMenu, Level_Past)
├── Scripts/
│   ├── Player/          # WeaponController, CombatSystem, PlayerDeath
│   ├── Enemy/           # EnemyAI, HealthSystem
│   ├── Systems/         # Portal, GoldManager, KeyPickup, DoorController, GameFinish, GraphicsManager
│   └── UI/              # PlayerHUD, Crosshair, PauseMenu, MainMenuManager, WinScreen, HealthPickup
├── Settings/            # URP render pipeline ayarları
├── Starter Assets/      # Unity Starter Assets (TPS kontrolcüsü, input sistemi)
└── UI/                  # Arayüz görselleri ve ikonlar
```

## Script Detayları

### Oyuncu (Player)
| Script | Görevi |
|--------|--------|
| `WeaponController.cs` | Silah çekme/kılıflama, FOV yakınlaştırmalı nişan, raycast atış ve mermi izi efekti |
| `CombatSystem.cs` | Savaş mantığı ve hasar verme |
| `PlayerDeath.cs` | Oyuncu ölüm yönetimi |
| `IDamageable.cs` | Hasar alabilir nesneler için arayüz (interface) |

### Düşman (Enemy)
| Script | Görevi |
|--------|--------|
| `EnemyAI.cs` | NavMesh tabanlı düşman davranışı (devriye, takip, saldırı) |
| `HealthSystem.cs` | Düşman canı, hasar alma ve ölüm |

### Sistemler (Systems)
| Script | Görevi |
|--------|--------|
| `Portal.cs` | Işınlanma / sahne geçiş sistemi |
| `GoldManager.cs` | Oyun genelinde toplanan altın takibi |
| `GoldPickup.cs` | Altın toplama mantığı |
| `KeyPickup.cs` | Kapı açmak için anahtar toplama |
| `DoorController.cs` | Anahtarla tetiklenen kapı açma/kapama |
| `GameFinish.cs` | Kazanma koşulu ve oyun bitiş akışı |
| `GraphicsManager.cs` | Çalışma zamanı grafik kalitesi ayarları |

### Arayüz (UI)
| Script | Görevi |
|--------|--------|
| `PlayerHUD.cs` | Can barı, altın sayacı, anahtar envanteri gösterimi |
| `CrosshairController.cs` | Dinamik nişangah |
| `PauseMenu.cs` | Oyunu durdurma/devam ettirme |
| `MainMenuManager.cs` | Ana menü navigasyonu ve sahne yükleme |
| `MenuAnimator.cs` | Menü geçiş animasyonları |
| `MenuButtonEffect.cs` | Buton hover/tıklama görsel efektleri |
| `MenuStyleApplier.cs` | Menüler arası tutarlı stil uygulama |
| `WinScreenAnimator.cs` | Zafer ekranı animasyonları |
| `HealthPickup.cs` | Can yenileme eşyası mantığı |
| `AutoHideText.cs` | Bildirimler için otomatik kaybolan yazı |

## Teknoloji

- **Motor:** Unity 6 (6000.0.68f1), Universal Render Pipeline (URP)
- **Dil:** C#
- **Girdi Sistemi:** Unity New Input System
- **Kamera:** Cinemachine (Unity.Cinemachine)
- **Navigasyon:** Unity NavMesh (düşman yol bulma)
- **Temel Kontrolcü:** Unity Starter Assets — Third Person Character Controller

## Kullanılan Asset Store Paketleri

Aşağıdaki paketler dosya boyutu nedeniyle depoya dahil edilmemiştir. Projeyi klonladıktan sonra Unity Asset Store üzerinden içe aktarın:

- **Pirate Customized** — Korsan karakter modeli ve dokuları
- **Basic Bandit** — Düşman karakter modeli
- **Stylized Pirate Ship** — Korsan gemisi çevre objesi
- **Island** — Ada arazisi ve çevre objeleri
- **4K Tiled Ground Textures p1** — Zemin dokuları
- **Rust Key** — Toplanabilir anahtar modeli
- **SciFi Office Lite** — Gelecek dönemi çevre varlıkları
- **Sci-Fi Styled Modular Pack** — Gelecek dönemi modüler yapı parçaları
- **LiquidFire Package 4** — Görsel efektler (VFX)
- **Sprite Muzzle Flashes** — Namlu parlaması spriteları
- **SlimUI** — Arayüz çerçevesi
- **TextMesh Pro** — Metin oluşturma (genellikle Unity ile birlikte gelir)

## Kurulum

1. Depoyu klonlayın:
   ```bash
   git clone https://github.com/KeremUUnal/tps_game_project.git
   ```

2. Projeyi **Unity 6 (6000.0.68f1)** veya üzeri sürümle açın

3. Yukarıda listelenen Asset Store paketlerini içe aktarın

4. `Assets/Scenes/MainMenu.unity` veya `Assets/Scenes/Level_Past.unity` sahnesini açın

5. **Play** tuşuna basın

## Kontroller

| Tuş | Eylem |
|-----|-------|
| `WASD` | Hareket |
| `Fare` | Etrafına bakınma |
| `Space` | Zıplama |
| `Left Shift` | Koşma |
| `Q` | Silah çekme / kılıflama |
| `Sağ Tık` | Nişan alma (yakınlaştırma) |
| `Sol Tık` | Ateş etme |
| `Escape` | Duraklat menüsü |

## Lisans

Bu proje akademik amaçlarla geliştirilmiştir.

---

*Unity 6 & URP ile geliştirilmiştir*