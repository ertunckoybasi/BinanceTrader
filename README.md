# BinanceTrader

# C# ile Binance Üzerinde Algoritmik Trade Robotu

## Proje Tanımı
Bu proje, C# ile Binance üzerinde çalışan bir algoritmik trade robotu geliştirmeyi amaçlamaktadır. Temel konuları ve stratejileri içeren örnek bir çalışma sunmaktadır.

## Genel Bilgi
Algoritmik trade, finansal piyasalarda ticaret stratejilerinin otomatik olarak uygulanmasını sağlar. Binance ise kripto para ticareti yapmak için kullanılan en büyük platformlardan biridir.

## Özellikler
1. **Algoritmik Trade Robotu**: Binance üzerinde çalışan bir trade robotu.
2. **Stratejiler**: İçerisinde basit olarak Super Trend gibi iki adet strateji barındırır.
3. **Emirler**: Buy, Take Profit ve Stop Loss emirleri verilmektedir.
4. **Kütüphaneler**: Binance wrapper olarak [Binance.Net](https://github.com/JKorf/binance.net) ve indikatörler için [Stock.Indicators](https://dotnet.stockindicators.dev/indicators/) kütüphanesi kullanılmıştır.
5. **Strateji Deseni**: Strateji seçimi için Factory Pattern kullanımı.
6. **Ayarlar**: Parite, lot ve strateji seçimi yapılan bir settings class'ı bulunmaktadır.
7. **Teknoloji**: .NET 8 ile console uygulaması olarak yazılmıştır.
8. **Örnek Çalışma**: Algoritmik trade ile ilgilenenler ve C# bilenler için örnek bir çalışma.
9. **Veri Kullanımı**: İlgili sembolün geçmiş verileri çekilerek indikatörlerde istenen timeframe'de kullanımı sağlanmıştır.

## Kullanım
Projeyi kullanmadan önce aşağıdaki adımları izleyin:
1. [Binance.Net](https://github.com/JKorf/binance.net) ve [Stock.Indicators](https://dotnet.stockindicators.dev/indicators/) kütüphanelerini kurun.
2. Parite, lot ve strateji seçimlerinizi settings class'ı üzerinden yapın.
3. Trade robotunuzu çalıştırın ve sonuçları gözlemleyin.

## Uyarı
**Robotu doğrudan hesaplarınızda çalıştırmayınız.** Bu tavsiyeler mali durumunuz ile risk ve getiri tercihlerinize uygun olmayabilir. Sadece burada yer alan bilgilere dayanarak yatırım kararı verilmesi beklentilerinize uygun sonuçlar doğurmayabilir.

## Katkıda Bulunma
Katkıda bulunmak isterseniz, lütfen bir pull request gönderin. Her türlü geri bildirime açığım.

## Lisans
Bu proje MIT lisansı altında lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına bakın.

---

Umarım bu içerikler işinize yarar! Eğer başka bir konuda daha yardıma ihtiyacınız olursa, bana her zaman sorabilirsiniz.
