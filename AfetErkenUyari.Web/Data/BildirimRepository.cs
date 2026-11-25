using AfetErkenUyari.Web.Entities;
using Microsoft.Data.SqlClient;

namespace AfetErkenUyari.Web.Data;

/// <summary>Bildirimler tablosu: e-posta kuyruğu ve gönderim kaydı (Hafta 9).</summary>
public class BildirimRepository(VeritabaniBaglanti db)
{
    /// <summary>
    /// Uyarının ulaşacağı ve E-posta kanalını seçmiş aboneleri kuyruğa ekler.
    /// Uyarı ile AYNI transaction içinde çağrılır: uyarı kaydedilemezse kuyruk da oluşmaz.
    /// Kullanıcı birden çok hedef bölgeye abone olsa bile tek e-posta alır (DISTINCT + UQ_Bildirimler).
    /// </summary>
    public static int KuyrugaEkle(SqlConnection con, SqlTransaction tr, int uyariId)
    {
        using var cmd = new SqlCommand(@"
            INSERT INTO dbo.Bildirimler (UyariId, KullaniciId, Kanal, Alici)
            SELECT DISTINCT @uid, k.Id, N'Eposta', k.Eposta
            FROM dbo.UyariBolgeleri ub
            JOIN dbo.Uyarilar u     ON u.UyariId = ub.UyariId
            JOIN dbo.Abonelikler a  ON a.BolgeId = ub.BolgeId
            JOIN dbo.Kullanicilar k ON k.Id = a.KullaniciId
            WHERE ub.UyariId = @uid
              AND a.AktifMi = 1
              AND a.BildirimKanali = N'Eposta'
              AND u.Seviye >= a.MinSeviye
              AND k.AktifMi = 1
              AND NOT EXISTS (SELECT 1 FROM dbo.Bildirimler b
                              WHERE b.UyariId = @uid AND b.KullaniciId = k.Id AND b.Kanal = N'Eposta');", con, tr);
        cmd.Parameters.AddWithValue("@uid", uyariId);
        return cmd.ExecuteNonQuery();
    }

    /// <summary>Gönderilmeyi bekleyen kayıtlar.</summary>
    public List<Bildirim> Bekleyenler(int uyariId) => Sorgula("WHERE b.UyariId = @uid AND b.Durum = N'Bekliyor'", uyariId);

    /// <summary>Bir uyarının tüm gönderim kayıtları (yönetici ekranı).</summary>
    public List<Bildirim> Listele(int uyariId) => Sorgula("WHERE b.UyariId = @uid", uyariId);

    /// <summary>Başarısız kayıtları yeniden denemek için tekrar kuyruğa alır.</summary>
    public int BasarisizlariKuyrugaAl(int uyariId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            "UPDATE dbo.Bildirimler SET Durum = N'Bekliyor' WHERE UyariId = @uid AND Durum = N'Basarisiz'", con);
        cmd.Parameters.AddWithValue("@uid", uyariId);
        return cmd.ExecuteNonQuery();
    }

    /// <summary>Bir gönderim denemesinin sonucunu yazar.</summary>
    public void SonucYaz(int bildirimId, string durum, int denemeSayisi, string? hata, string? saglayiciMesajId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"UPDATE dbo.Bildirimler
              SET Durum = @durum, DenemeSayisi = @deneme, SonHata = @hata, SaglayiciMesajId = @mid,
                  GonderimTarihi = CASE WHEN @durum IN (N'Gonderildi', N'DenemeModu') THEN GETDATE() ELSE GonderimTarihi END
              WHERE BildirimId = @id", con);
        cmd.Parameters.AddWithValue("@durum", durum);
        cmd.Parameters.AddWithValue("@deneme", Math.Min(denemeSayisi, 255));
        cmd.Parameters.AddWithValue("@hata", VeritabaniBaglanti.DbDegeri(Kisalt(hata, 500)));
        cmd.Parameters.AddWithValue("@mid", VeritabaniBaglanti.DbDegeri(Kisalt(saglayiciMesajId, 200)));
        cmd.Parameters.AddWithValue("@id", bildirimId);
        cmd.ExecuteNonQuery();
    }

    private List<Bildirim> Sorgula(string kosul, int uyariId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"SELECT b.BildirimId, b.UyariId, b.KullaniciId, b.Kanal, b.Alici, b.Durum, b.DenemeSayisi,
                     b.SonHata, b.SaglayiciMesajId, b.OlusturmaTarihi, b.GonderimTarihi, k.KullaniciAdi
              FROM dbo.Bildirimler b
              JOIN dbo.Kullanicilar k ON k.Id = b.KullaniciId " + kosul + @"
              ORDER BY b.BildirimId", con);
        cmd.Parameters.AddWithValue("@uid", uyariId);
        using var dr = cmd.ExecuteReader();
        var liste = new List<Bildirim>();
        while (dr.Read())
        {
            liste.Add(new Bildirim
            {
                BildirimId = (int)dr["BildirimId"],
                UyariId = (int)dr["UyariId"],
                KullaniciId = (int)dr["KullaniciId"],
                Kanal = (string)dr["Kanal"],
                Alici = (string)dr["Alici"],
                Durum = (string)dr["Durum"],
                DenemeSayisi = dr.Tamsayi("DenemeSayisi"),
                SonHata = dr["SonHata"] as string,
                SaglayiciMesajId = dr["SaglayiciMesajId"] as string,
                OlusturmaTarihi = (DateTime)dr["OlusturmaTarihi"],
                GonderimTarihi = dr.TarihVeyaNull("GonderimTarihi"),
                KullaniciAdi = (string)dr["KullaniciAdi"]
            });
        }
        return liste;
    }

    private static string? Kisalt(string? s, int uzunluk) => s is null || s.Length <= uzunluk ? s : s[..uzunluk];
}
