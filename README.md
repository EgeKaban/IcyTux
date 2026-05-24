# Game Design Document

## Proje ve Ekip Bilgileri

* **Takım Adı:** Icy Tux
* **GitHub Bağlantısı:** https://github.com/EgeKaban/IcyTux

**Üyeler ve Roller:**

| İsim | Rol |
| :--- | :--- |
| Yiğit Dutar | Game Artist / Sound |
| Ege Kaban | Programmer / Sound |
| Efe Cem Köseoğlu | Game Designer / Sound |
| Ömer Efe Harbili | Game Designer / Level Designer / Sound |

## 1. Özet

* **Oyunun Adı:** Katana Game
* **Tür:** 2D Action-Puzzle
* **Elevator Pitch:** Oyuncu, zamanın sadece kendisi hareket ettiğinde aktığı bir dünyada katanalı bir dövüşçüyü kontrol eder. Her hamle, sınırlı bir "atılma" (dash) hakkı ile stratejik bir bulmacaya dönüşür. Yanlış bir hamle zamanı geri sarmayı gerektirirken, doğru zamanlama ve düşman kullanımı oyuncuyu durdurulamaz bir ölüm makinesine dönüştürür.

## 2. Oynanış

* **Zaman Mekaniği:** Oyun dünyasında zaman durağandır. Zamanın akması tamamen karakterin hareketine bağlıdır. Karakter durduğunda her şey donar; bu, oyuncuya bir sonraki "slice-dash" yörüngesini planlaması için sınırsız düşünme süresi tanır.
* **İlerleme ve Öğretici:** Oyuncunun mekaniklere alışması için kademeli bir yapı öngörülmüştür:
  * **Yürüme:** Hareket kısıtlı olacak, 1-2 adım maksimum şeklinde ve hareket etmediği süre boyunca zaman tamamıyla duruyor olacak.
  * **2 Kısa Bölüm:** Temel kontroller ve zaman mekaniğinin öğretilmesi.
  * **1 Uzun Bölüm:** Öğrenilen tüm mekaniklerin (dash, zaman ve düşman etkileşimi) birleştirildiği kapsamlı bir test alanı.
  * **Toplam 4 bölüm**
* **Kazanma/Kaybetme:** Sınırlı dash ile tüm hedefleri temizlemek zaferdir. Boşa atılan bir dash veya hakların bitmesi "Zamanı Geri Sar" (Rewind) komutunu tetikler.
* **Temel Döngü (Core Loop):**
  1. Düşmanların yerleşimini ve bölüm tasarımını (level layout) gözlemle.
  2. Atılma (dash) yörüngesini planla.
  3. Düşmanları kesmek için atılmayı (slice-dash) gerçekleştir.
  4. Bölümü atılma limiti sınırları altında tamamla.

## 3. Mekanikler

* **Kontroller:**
  * **Klave Tuşları (WASD/Yön Okları):** Atılmanın (dash) yönünü nişan almayı sağlar.
  * **Sol Shift Tıklaması:** Kesik-atılma (slice-dash) hareketini gerçekleştirir.
  * **WASD / Yön Tuşları:** Atılmalar arasında, sınırlı bir menzil içinde ihtiyaç duyulursa temel hareket etmeyi sağlar.
  * **R Tuşu:** Bölümü yeniden başlatır (Restart level) veya zamanı geri sarar (Rewind time).
* **Özel Düşman Etkileşimi:** Seviyelerde bulunan "Özel Düşmanlar" (Special Enemies) öldürüldüğünde, karakterin hareket alanını (dash menzilini) kalıcı olmayan, bir kereye mahsus +1 birim artırır. Bu özellik stratejik olarak en uzak hedeflere ulaşmak için kullanılmalıdır. (tahmini bir sayı 1 olarak dedim)

## 4. Oyun Unsurları

* **Düşman Varyasyonları:**
  * **Standart Düşman:** Tek dash ile kesilen hedefler.
  * **Ateş Eden Düşman:** Tek dash ile kesilen ve oyuncuya eteş eden hedefler.
  * **Özel Düşman:** Katledildiğinde oyuncuya ekstra hareket alanı (bonus range) sağlayan stratejik hedefler.
* **Bölüm Tasarımı:** Zamanın durma özelliğini zorunlu kılan, mermilerin veya hareketli engellerin olduğu "bulmaca odaları". Düşman yerleşimlerinin, oyuncunun yörünge planlamasını test edeceği ve "puzzle" yapısını oluşturduğu stratejik alanlar.
* **Görsel Sunum:** Karakter, çevredeki gri tonların aksine (belki kıyafetindeki bir detay veya kılıç parlamasıyla) oyuncunun gözlemini kolaylaştıracak şekilde tasarlanmıştır. Belki kontrast bir renk tonu.
* **Hassas Konumlanma:** Dash hamleleri arasında, oyuncunun bir sonraki saldırı açısını mükemmelleştirmesi için kısıtlı bir alanda yürüme yeteneği.
* **Atmosferik Dünya:** Oyun, gri ve monokrom altın tonların hakim olduğu, terk edilmiş ve yosun tutmuş antik taş bloklardan oluşan bir evrende geçer.
* **İzleme Hattı:** Fare ile nişan alırken görüken hayali dash çizgisi oyuncunun planlama aşaması için.
* **Zaman Manipülasyonu Görseli:** Geri sarma esnasında devreye giren ekran efektleri.
* **Giriş Ekranı:** Menü tuşları solda olacak şekilde 3 tuş olacak: Oyna - Ayarlar - Çıkış. Ana menüdeki tuşların sağ tarafında karakterin silüeti ya da kendisi nefes alıp verme ya da rüzgardan saçılan saçları vs gibi ana menü girişi.

## 5. Varlıklar

* **Görsel Sanat Tarzı:** 2D Pixel Art
* **Renk Paleti ve Atmosfer:** Oyun genel olarak karanlık, soğuk ve monokromatik (gri, siyah ve beyaz tonları) bir renk paleti kullanmaktadır. Bu renksiz ve kasvetli arka plan, "slice-dash" sırasında çıkacak kan, kılıç izi (trail) veya düşman indikatörleri gibi parlak renkli elementlerin oyuncunun gözünde anında öne çıkmasını sağlayacaktır. Renk paletimiz daha altın tonu olacak şekilde gölgelendirmeler bırakan bir yapı.

### Renk Paleti (Aseprite)

Palet toplam **48 renk**, 16×3 grid formatında düzenlenmiştir.

#### 🖤 Koyu & Nötr Tonlar
Arka plan, gölge ve zemin dokuları için kullanılan koyu tonlar.

| Renk | Hex | Renk | Hex | Renk | Hex | Renk | Hex |
|:----:|:---:|:----:|:---:|:----:|:---:|:----:|:---:|
| ![#090E0A](https://placehold.co/20x20/090E0A/090E0A.png) | `#090E0A` | ![#0C110B](https://placehold.co/20x20/0C110B/0C110B.png) | `#0C110B` | ![#181A19](https://placehold.co/20x20/181A19/181A19.png) | `#181A19` | ![#24232B](https://placehold.co/20x20/24232B/24232B.png) | `#24232B` |
| ![#28252E](https://placehold.co/20x20/28252E/28252E.png) | `#28252E` | ![#353432](https://placehold.co/20x20/353432/353432.png) | `#353432` | ![#494B4A](https://placehold.co/20x20/494B4A/494B4A.png) | `#494B4A` | ![#504C4B](https://placehold.co/20x20/504C4B/504C4B.png) | `#504C4B` |
| ![#685F60](https://placehold.co/20x20/685F60/685F60.png) | `#685F60` | ![#737462](https://placehold.co/20x20/737462/737462.png) | `#737462` | ![#C4CCBD](https://placehold.co/20x20/C4CCBD/C4CCBD.png) | `#C4CCBD` | ![#FDFFD7](https://placehold.co/20x20/FDFFD7/FDFFD7.png) | `#FDFFD7` |
| ![#150200](https://placehold.co/20x20/150200/150200.png) | `#150200` | ![#6D6149](https://placehold.co/20x20/6D6149/6D6149.png) | `#6D6149` | | | | |

#### 🟤 Toprak & Kahve Tonlar
Duvar dokuları, zemin blokları ve çevre detayları için kullanılan ılık toprak tonlar.

| Renk | Hex | Renk | Hex | Renk | Hex | Renk | Hex |
|:----:|:---:|:----:|:---:|:----:|:---:|:----:|:---:|
| ![#514837](https://placehold.co/20x20/514837/514837.png) | `#514837` | ![#8A774D](https://placehold.co/20x20/8A774D/8A774D.png) | `#8A774D` | ![#483804](https://placehold.co/20x20/483804/483804.png) | `#483804` | ![#4B4219](https://placehold.co/20x20/4B4219/4B4219.png) | `#4B4219` |
| ![#353200](https://placehold.co/20x20/353200/353200.png) | `#353200` | ![#262400](https://placehold.co/20x20/262400/262400.png) | `#262400` | ![#423E11](https://placehold.co/20x20/423E11/423E11.png) | `#423E11` | ![#615229](https://placehold.co/20x20/615229/615229.png) | `#615229` |
| ![#6A5D33](https://placehold.co/20x20/6A5D33/6A5D33.png) | `#6A5D33` | ![#786E4B](https://placehold.co/20x20/786E4B/786E4B.png) | `#786E4B` | ![#8A8657](https://placehold.co/20x20/8A8657/8A8657.png) | `#8A8657` | ![#948C68](https://placehold.co/20x20/948C68/948C68.png) | `#948C68` |
| ![#A19B6B](https://placehold.co/20x20/A19B6B/A19B6B.png) | `#A19B6B` | | | | | | |

#### 🟡 Altın Tonlar
Karakter, kılıç parlaması, kılıç izi (trail) ve vurgu elementleri için kullanılan ana altın renk ailesi.

| Renk | Hex | Renk | Hex | Renk | Hex | Renk | Hex |
|:----:|:---:|:----:|:---:|:----:|:---:|:----:|:---:|
| ![#A38A47](https://placehold.co/20x20/A38A47/A38A47.png) | `#A38A47` | ![#A4902D](https://placehold.co/20x20/A4902D/A4902D.png) | `#A4902D` | ![#B39825](https://placehold.co/20x20/B39825/B39825.png) | `#B39825` | ![#CFB46F](https://placehold.co/20x20/CFB46F/CFB46F.png) | `#CFB46F` |
| ![#D1B258](https://placehold.co/20x20/D1B258/D1B258.png) | `#D1B258` | ![#D3BF5C](https://placehold.co/20x20/D3BF5C/D3BF5C.png) | `#D3BF5C` | ![#D6C86F](https://placehold.co/20x20/D6C86F/D6C86F.png) | `#D6C86F` | ![#ECC03B](https://placehold.co/20x20/ECC03B/ECC03B.png) | `#ECC03B` |
| ![#DFCC3F](https://placehold.co/20x20/DFCC3F/DFCC3F.png) | `#DFCC3F` | ![#DFC749](https://placehold.co/20x20/DFC749/DFC749.png) | `#DFC749` | ![#E0CE48](https://placehold.co/20x20/E0CE48/E0CE48.png) | `#E0CE48` | ![#E1C851](https://placehold.co/20x20/E1C851/E1C851.png) | `#E1C851` |
| ![#E4CB54](https://placehold.co/20x20/E4CB54/E4CB54.png) | `#E4CB54` | ![#E5D050](https://placehold.co/20x20/E5D050/E5D050.png) | `#E5D050` | ![#E7C94F](https://placehold.co/20x20/E7C94F/E7C94F.png) | `#E7C94F` | ![#ECCC53](https://placehold.co/20x20/ECCC53/ECCC53.png) | `#ECCC53` |
| ![#F0C94A](https://placehold.co/20x20/F0C94A/F0C94A.png) | `#F0C94A` | ![#F0D059](https://placehold.co/20x20/F0D059/F0D059.png) | `#F0D059` | ![#F0DA50](https://placehold.co/20x20/F0DA50/F0DA50.png) | `#F0DA50` | ![#F6D85E](https://placehold.co/20x20/F6D85E/F6D85E.png) | `#F6D85E` |
| ![#F7CD3B](https://placehold.co/20x20/F7CD3B/F7CD3B.png) | `#F7CD3B` | | | | | | |
* **Çevre Tasarımı ve Tileset:**
  * Bölümler modüler, kare ızgara (grid) tabanlı bir tileset yapısıyla inşa edilmektedir.
  * **Zemin ve Duvar Dokuları:**
    * **Pürüzsüz Taşlar:** Standart zemin ve duvarları oluşturan sade altın gri bloklar.
    * **Aşınmış/Çatlak Bloklar:** Çevrenin terk edilmiş ve eski hissini güçlendiren kırık dökük detaylar.
    * **Organik Detaylar (Sarmaşık/Yosun):** Duvarlardan sarkan veya blokları kaplayan koyu gri tonlarındaki yosun ve sarmaşık dokuları, ortama zindan/harabe derinliği katmaktadır.
    * **Açık Renkli Sınırlar:** Platformların üst yüzeyleri (oyuncunun/düşmanların basabileceği alanlar) sınırları net belli etmek adına daha açık gri/beyaz hatlarla çizilmiştir.
* **Ses Tasarımı:** Oyunun ses dizaynı (kılıç kesik sesleri, dash rüzgarı, ortam ambiyansı vb.), ekibin tüm üyelerinin (Yiğit, Ege, Efe Cem, Ömer Efe) ortak sorumluluğundadır ve atmosferi destekleyecek şekilde entegre edilecektir.
