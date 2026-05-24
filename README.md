Game Design Document 

Proje ve Ekip Bilgileri 

* 
**Takım Adı:** Icy Tux 


* 
**GitHub Bağlantısı:** [https://github.com/EgeKaban/Icy](https://github.com/EgeKaban/Icy) 



**Üyeler ve Roller:**

| İsim | Rol |
| --- | --- |
| Yiğit Dutar | Game Artist / Sound 

 |
| Ege Kaban | Programmer / Sound 

 |
| Efe Cem Köseoğlu | Game Designer / Sound 

 |
| Ömer Efe Harbili | Game Designer / Level Designer / Sound 

 |

---

1. Özet 

* 
**Oyunun Adı:** ???????? 


* 
**Tür:** 2D Action-Puzzle 


* 
**Elevator Pitch:** Oyuncu, zamanın sadece kendisi hareket ettiğinde aktığı bir dünyada katanalı bir dövüşçüyü kontrol eder. Her hamle, sınırlı bir "atılma" (dash) hakkı ile stratejik bir bulmacaya dönüşür. Yanlış bir hamle zamanı geri sarmayı gerektirirken, doğru zamanlama ve düşman kullanımı oyuncuyu durdurulamaz bir ölüm makinesine dönüştürür.



---

2. Oynanış 

### Zaman Mekaniği

Oyun dünyasında zaman durağandır. Zamanın akması tamamen karakterin hareketine bağlıdır. Karakter durduğunda her şey donar; bu, oyuncuya bir sonraki "slice-dash" yörüngesini planlaması için sınırsız düşünme süresi tanır.

### İlerleme ve Öğretici

Oyuncunun mekaniklere alışması için kademeli bir yapı öngörülmüştür:

* 
**Yürüme:** harrket kısıtlı olucak 1 2 adım maksimum şeklinde ve hareket etmediği süre boyunca zaman tamamıyla duruyor olacak :/ 


* 
**2 Kısa Bölüm:** Temel kontroller ve zaman mekaniğinin öğretilmesi.


* 
**1 Uzun Bölüm:** Öğrenilen tüm mekaniklerin (dash, zaman ve düşman etkileşimi) birleştirildiği kapsamlı bir test alanı.


* 
**Bölüm Sayısı:** Toplam 4 bölüm 



### Kazanma/Kaybetme

Sınırlı dash ile tüm hedefleri temizlemek zaferdir. Boşa atılan bir dash veya hakların bitmesi "Zamanı Geri Sar" (Rewind) komutunu tetikler.

Temel Döngü (Core Loop) 

1. Düşmanların yerleşimini ve bölüm tasarımını (level layout) gözlemle.


2. Atılma (dash) yörüngesini planla.


3. Düşmanları kesmek için atılmayı (slice-dash) gerçekleştir.


4. Bölümü atılma limiti sınırları altında tamamla.



---

3. Mekanikler 

Kontroller 

* 
**-Fare Hareketi):** Atılmanın (dash) yönünü nişan almayı sağlar.


* 
**Sol Fare Tıklaması:** Kesik-atılma (slice-dash) hareketini gerçekleştirir.


* 
**WASD / Yön Tuşları:** Atılmalar arasında, sınırlı bir menzil içinde ihtiyaç duyulursa temel hareket etmeyi sağlar.


* 
**R Tuşu:** Bölümü yeniden başlatır (Restart level) veya zamanı geri sarar (Rewind time).



### Özel Düşman Etkileşimi

Seviyelerde bulunan "Özel Düşmanlar" (Special Enemies) öldürüldüğünde, karakterin hareket alanını (dash menzilini) kalıcı olmayan, bir kereye mahsus +1 birim artırır. Bu özellik stratejik olarak en uzak hedeflere ulaşmak için kullanılmalıdır. (tahmini bir sayı 1 olarak dedim) .

---

4. Oyun Unsurları 

Düşman Varyasyonları 

* 
**Standart Düşman:** Tek dash ile kesilen hedefler.


* 
**Özel Düşman:** Katledildiğinde oyuncuya ekstra hareket alanı (bonus range) sağlayan stratejik hedefler.



### Tasarım ve Görsel Unsurlar

* 
**Bölüm Tasarımı:** Zamanın durma özelliğini zorunlu kılan, mermilerin veya hareketli engellerin olduğu "bulmaca odaları". Düşman yerleşimlerinin, oyuncunun yörünge planlamasını test edeceği ve "puzzle" yapısını oluşturduğu stratejik alanlar.


* 
**Görsel Sunum:** Karakter, çevredeki gri tonların aksine (belki kıyafetindeki bir detay veya kılıç parlamasıyla) oyuncunun gözlemini kolaylaştıracak şekilde tasarlanmıştır. Belki kontrast bir renk tonu.


* 
**Hassas Konumlanma:** Dash hamleleri arasında, oyuncunun bir sonraki saldırı açısını mükemmelleştirmesi için kısıtlı bir alanda yürüme yeteneği.


* 
**Atmosferik Dünya:** Oyun, gri ve monokrom altın tonların hakim olduğu, terk edilmiş ve yosun tutmuş antik taş bloklardan oluşan bir evrende geçer.


* 
**İzleme Hattı:** Fare ile nişan alırken görüken hayali dash çizgisi oyuncunun planlama aşaması için.


* 
**Zaman Manipülasyonu Görseli:** Geri sarma esnasında devreye giren ekran efektleri.



Giriş Ekranı 

* menü tuşları solda olucak şekilde 3 tuş olucak 


* Oyna - Ayarlar - Çıkış 


* ana menüdeki tuşların sağ tarafında karakterin silüeti yada kendisi nefe alıp verme yada rüzgardan saşçılan saçları vs gibi ana menü girişi.



---

5. Varlıklar 

### Sanat ve Atmosfer

* 
**Görsel Sanat Tarzı:** 2D Pixel Art 


* 
**Renk Paleti ve Atmosfer:** Oyun genel olarak karanlık, soğuk ve monokromatik (gri, siyah ve beyaz tonları) bir renk paleti kullanmaktadır. Bu renksiz ve kasvetli arka plan, "slice-dash" sırasında çıkacak kan, kılıç izi (trail) veya düşman indikatörleri gibi parlak renkli elementlerin oyuncunun gözünde anında öne çıkmasını sağlayacaktır. Renk paledimiz daha altın tonu olucak şekile gölgelendirmeler bırakan bir yapı.



Çevre Tasarımı ve Tileset 

Bölümler modüler, kare ızgara (grid) tabanlı bir tileset yapısıyla inşa edilmektedir.

**Zemin ve Duvar Dokuları:** 

* 
**Pürüzsüz Taşlar:** Standart zemin ve duvarları oluşturan sade altın gri bloklar.


* 
**Aşınmış/Çatlak Bloklar:** Çevrenin terk edilmiş ve eski hissini güçlendiren kırık dökük detaylar.


* 
**Organik Detaylar (Sarmaşık/Yosun):** Duvarlardan sarkan veya blokları kaplayan koyu gri tonlarındaki yosun ve sarmaşık dokuları, ortama zindan/harabe derinliği katmaktadır.


* 
**Açık Renkli Sınırlar:** Platformların üst yüzeyleri (oyuncunun/düşmanların basabileceği alanlar) sınırları net belli etmek adına daha açık gri/beyaz hatlarla çizilmiştir.



Ses Tasarımı 

Oyunun ses dizaynı (kılıç kesik sesleri, dash rüzgarı, ortam ambiyansı vb.), ekibin tüm üyelerinin (Yiğit, Ege, Efe Cem, Ömer Efe) ortak sorumluluğundadır ve atmosferi destekleyecek şekilde entegre edilecektir.
