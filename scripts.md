Çok yerinde bir sorudan başlamışsın! Hangi fizik veya hareket kontrolcüsünü seçeceğin, oyunun tüm "hissiyatını" (game feel) ve hata (bug) oranını belirleyecek en kritik karardır.

Doğrudan cevap vereyim: Unity'de 2D bir oyun yapıyorsan ve işin içinde yüksek hızlı "dash", zaman yavaşlatma ve düşmanların içinden geçerek kesme ("slice") mekanikleri varsa, **kesinlikle `Rigidbody2D` kullanmalısın.** Unity'nin yerleşik `CharacterController` bileşeni 3D oyunlar için tasarlanmıştır. 2D için kendi özel kontrolcünü yazabilirsin, ancak bu tür fizikli "action-puzzle" oyunlarında `Rigidbody2D` sana çarpışma algılama, trigger'lar ve kuvvet (force) uygulama konusunda devasa bir avantaj sağlar.

İşte tam olarak kullanman gereken scriptlerin ve metotların teknik mimarisi:

### 1. `PlayerMovement` (Yürüme Kontrolü)

Bu script sadece kısıtlı/serbest yürüme ve zamana etki etme işini yapacak. `Rigidbody2D` bileşenine ihtiyaç duyar.

* **Kritik Ayar:** `Rigidbody2D` bileşenindeki "Collision Detection" ayarını **Continuous** yapmalısın. Aksi takdirde dash atarken duvarların içinden geçme (tunneling) hatası yaşarsın.
* `Update()`: Oyuncunun WASD inputlarını okuyacaksın. Aynı zamanda tuşa basılıp basılmadığına göre `TimeManager`'a "zamanı hızlandır" veya "dondur" komutunu göndereceğin yer burasıdır.
* `FixedUpdate()`: Fizik hesaplamaları burada yapılır. Inputtan aldığın Vector2 yönünü, `rb.MovePosition()` metodunu kullanarak karakteri hareket ettirmek için kullanmalısın. `MovePosition`, karakteri teleport etmez, fizik kurallarına uyarak o noktaya iter; böylece duvarlara takılmazsın.
* `HandleStamina()`: Eğer `isInfiniteStamina` `false` ise (yani çatışma odasındaysan), aldığın yolu hesaplayıp (örneğin başlangıç noktasından olan Vector2 uzaklığı ölçerek) maksimum 2 adım atıldığında input almayı kilitleyen özel bir metot.

### 2. `PlayerDash` (Atılma ve Kesme)

Dash mekaniği anlık ve çok hızlı olacağı için, hareketi ve kesme işlemini ikiye ayırmak en güvenlisidir.

* `Update()`: Sol tıklandığında `PerformDash()` metodunu tetikler ve dash cooldown/limit (hak) kontrolünü yapar.
* `PerformDash(Vector2 direction)`: Bu bir **Coroutine** (`IEnumerator`) olmalı. Dash başladığında `rb.velocity`'ye çok yüksek bir değer atayabilir ve 0.2 saniye (örnek) bekleyip hızı tekrar sıfırlayabilirsin.
* `CheckSliceHits(Vector2 direction)`: **Altın Değerinde İpucu:** Dash esnasında düşmanları fiziksel çarpışma (`OnCollisionEnter2D`) ile algılamaya çalışmak yüksek hızlarda bazen hatalara yol açar. Bunun yerine dash başladığı an, gideceğin yöne doğru bir **`Physics2D.CircleCast`** (görünmez bir silindir ışın) gönder. Bu ışın dash menzilin boyunca hangi düşmanlara çarpıyorsa, onların canını anında azalt (`enemy.TakeDamage()`). Görsel olarak karakterin onların içinden geçmesi sadece bir animasyon/hareket illüzyonu olur, ama arkada kod işi saniyesinde hatasız çözmüş olur.

### 3. `TimeManager` (Lineer Olmayan Zaman Eğrisi)

Zamanın mikro ayarlamalar için yavaşça hızlanması mekaniğini yönetecek beyin burası.

* **Değişkenler:** `public AnimationCurve timeCurve;` (Bunu Unity arayüzünde S şeklinde bir grafik olarak ayarlayacaksın), `float timePressedTimer;`
* `UpdateTimeScale(bool isMoving)`: Bu metot `PlayerMovement` tarafından her karede çağrılır.
* Eğer `isMoving` `true` ise: `timePressedTimer += Time.unscaledDeltaTime;` (zaman dursa bile artan gerçek zaman). Sonra `Time.timeScale = timeCurve.Evaluate(timePressedTimer);`
* Eğer `isMoving` `false` ise: `timePressedTimer = 0f;` ve `Time.timeScale = 0.05f;` (tam 0 yapmak yerine 0.05 yapmak, animasyonların çok çok yavaş da olsa akmasını sağlar, oyun donmuş gibi hissettirmez).



### 4. `CombatZoneTrigger` (Oda Yönetimi)

GDD'de bahsettiğiniz "odalara girince stamina kapanacak" mantığını yönetecek basit ama etkili script. Odanın girişine görünmez bir `BoxCollider2D` (IsTrigger = true) koyacaksın.

* `OnTriggerEnter2D(Collider2D col)`: Eğer çarpan objenin tag'i "Player" ise, `PlayerMovement` scriptine ulaşıp `isInfiniteStamina = false` yapar. İsteğe bağlı olarak arkanızdan bir kapı kapatma animasyonunu da burada tetikleyebilirsiniz.
* `OnRoomCleared()`: `LevelManager` veya `GameManager` odadaki tüm düşmanların öldüğünü doğruladığında bu metot çalışır. `isInfiniteStamina = true` yapar ve kendini kapatır (`gameObject.SetActive(false)`).

### 5. `PlayerAiming` (İzleme Hattı)

Mouse'un olduğu yere doğru bir nişan alma çizgisi çıkartır.

* `UpdateAimLine()`: Karakterin pozisyonu ile `Camera.main.ScreenToWorldPoint(Input.mousePosition)` arasında yönü hesaplar. Bunu Unity'nin kendi **`LineRenderer`** bileşenine iki nokta (başlangıç ve bitiş) vererek çizebilirsin. GDD'de bahsettiğiniz "Özel Düşman" kesildiğinde menzil artarsa, bu çizginin maksimum uzunluğunu da aynı oranda uzatmalısın.













1. Oyuncu Kontrolü ve Hareket Sistemleri
Bu bölüm, oyuncunun hareketlerini, atılma mekaniğini ve nişan alma sistemini yönetir.

PlayerMovement: Yön tuşları veya WASD ile yapılan temel hareketi sağlar.

Mantık: Fiziksel hareket FixedUpdate içerisinde Rigidbody2D.MovePosition ile yapılır. Böylece karakter ışınlanmaz, fizik kurallarına uyarak kayar ve duvarlara takılmaz.

Stamina/Limit Entegrasyonu: İçerisinde bir isInfiniteStamina durumu barındırır. Serbest moddayken limitsiz yürüyüş sağlar; kısıtlı moda geçildiğinde ise kat edilen mesafeyi (Vector2 uzaklığı) ölçerek maksimum 1-2 adımdan sonra input almayı kilitler.

Zaman Tetikleyicisi: Karakter hareket ettiği veya durduğu anda TimeManager ile haberleşerek zaman ivmelenmesini başlatır/bitirir.

PlayerDash: Fare tıklamasıyla tetiklenen "slice-dash" hareketini yönetir.

Fiziksel Atılma: Hareketi anlık hızlandırmak için Rigidbody2D.velocity'ye çok yüksek bir değer atanır ve kısa bir süre (örn. 0.2s) sonra sıfırlanır.

Kesme (Slice) Mantığı: Çarpışma algılamak için fiziksel temas (OnCollisionEnter2D) kullanmak yerine, dash başladığı an gidilecek yöne doğru görünmez bir silindir ışın (Physics2D.CircleCast) fırlatılır. Bu ışın menzil boyunca hangi düşmanlara çarpıyorsa, anında canlarını azaltır. Karakterin düşmanların içinden geçmesi yalnızca görsel bir illüzyon olarak kalır; arka planda sistem hatasız işler.

PlayerAiming: Planlama aşaması için ekranda hayali bir izleme hattı oluşturur.

Mantık: Unity'nin LineRenderer bileşenini kullanarak karakterin pozisyonu ile farenin pozisyonu arasına bir çizgi çeker. Özel düşmanlar kesilip menzil arttığında, bu çizginin ulaştığı maksimum uzunluk da dinamik olarak güncellenir.

2. Zaman Manipülasyonu ve Geri Sarma
Zamanın oyuncunun hareketlerine göre akması ve hatalı hamlelerde geri alınması sistemidir.

TimeManager: Zamanın lineer olmayan (easing/acceleration) akışını kontrol eder.

İvmelenme Eğrisi: Anlık dur/kalk yerine Unity Inspector'da düzenlenebilen bir AnimationCurve kullanır. Oyuncu hareket tuşuna bastığında bir sayaç başlar ve Time.timeScale bu eğriye göre yavaşça artarak normal hızına ulaşır (micro-adjustment imkanı).

Dondurma Mantığı: Tuş bırakıldığı an sayaç sıfırlanır ve zaman anında dondurulur (veya animasyonların çok yavaş akması için 0.05 gibi bir değere çekilir).

RewindManager: R tuşuyla veya dash hakkı bittiğinde devreye girer. Karakterin ve düşmanların geçmiş pozisyonlarını, can durumlarını kaydedip geri yüklemekten sorumludur.

3. Oda ve Çatışma Yönetimi
"Bulmaca odaları" ve öğretici alanlar arasındaki geçişleri ve kısıtlamaları kontrol eder.

CombatZoneTrigger: Odaya girince kilit noktalarda takılmamak için eklenen sistemdir.

Tetiklenme (Giriş): Odaların girişlerine yerleştirilen görünmez bir BoxCollider2D (IsTrigger) kullanır. Oyuncu odaya girdiğinde OnTriggerEnter2D çalışır ve PlayerMovement içerisindeki isInfiniteStamina opsiyonunu kapatarak oyuncuyu GDD'deki kısıtlı adım moduna geçirir.

Çözülme (Çıkış): Odadaki tüm düşmanlar öldüğünde sistemden (GameManager/LevelManager) sinyal alır, oyuncuya tekrar sonsuz yürüme hakkı verir ve kendini kapatır.

4. Düşman Sınıflandırması
Zamanın durduğu bu bulmaca dünyasındaki hedeflerin mimarisidir.

EnemyBase: Tüm düşmanların can durumlarını ve "slice-dash" ile kesildiklerindeki ortak yok olma/ölüm davranışlarını içeren temel (base) sınıftır.

StandardEnemy: Sadece EnemyBase'den türer; ekstra bir mekaniği yoktur, tek atılma ile kesilip yok olan temel hedeflerdir.

SpecialEnemy: EnemyBase'den türer. Kendine has Die() fonksiyonu vardır. Katledildiğinde, bir kereye mahsus olmak üzere oyuncu scriptlerine sinyal göndererek hareket alanını (dash menzilini) +1 birim artırır.

5. Oyun Akışı ve Görsel/İşitsel Yönetim
Oyunun genel ilerleyişini, UI ve atmosfer öğelerini kontrol eder.

GameManager / LevelManager: Sınırlı dash haklarıyla bölümlerin temizlenip temizlenmediğini takip eder. Kazanma/kaybetme durumlarını yönetir. Bölüm sıfırlandığında düşmanları kare ızgara (grid) tabanlı haritada orijinal pozisyonlarına döndürür.

MainMenuController: Sol taraftaki menü tuşlarını ve sağ taraftaki karakter silüetinin nefes alma/rüzgar animasyonlarını kontrol eder.

VFXManager: Monokrom gri ve altın tonlu çevrede parlayacak kontrast kılıç izlerini (trail), kan efektlerini ve Rewind esnasındaki ekran post-processing efektlerini yönetir.

AudioManager: Tüm kılıç kesik sesleri, dash rüzgarı ve eski taş bloklardan oluşan atmosferik ortam ambiyansının senkronize çalınmasını sağlar.