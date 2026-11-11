using Microsoft.Data.SqlClient;

namespace AfetErkenUyari.Web.Data;

/// <summary>
/// Veri katmanının tek bağlantı noktası. Bağlantı cümlesi koda gömülmez;
/// appsettings.json içindeki "ConnectionStrings:AfetDB" kaydından okunur
/// (ya da ConnectionStrings__AfetDB ortam değişkeninden).
/// </summary>
public class VeritabaniBaglanti
{
    private readonly string _baglantiCumlesi;

    public VeritabaniBaglanti(IConfiguration yapilandirma)
    {
        _baglantiCumlesi = yapilandirma.GetConnectionString("AfetDB")
            ?? throw new InvalidOperationException("appsettings.json içinde 'ConnectionStrings:AfetDB' bulunamadı.");
    }

    /// <summary>Açılmış yeni bir bağlantı döndürür. Çağıran 'using' ile kapatmalıdır.</summary>
    public SqlConnection BaglantiAc()
    {
        var baglanti = new SqlConnection(_baglantiCumlesi);
        baglanti.Open();
        return baglanti;
    }

    public static object DbDegeri(object? deger) => deger ?? DBNull.Value;
}

/// <summary>SqlDataReader için küçük okuma yardımcıları.</summary>
internal static class OkuyucuUzantilari
{
    public static decimal? OndalikVeyaNull(this SqlDataReader dr, string sutun) =>
        dr[sutun] == DBNull.Value ? null : (decimal)dr[sutun];

    public static DateTime? TarihVeyaNull(this SqlDataReader dr, string sutun) =>
        dr[sutun] == DBNull.Value ? null : (DateTime)dr[sutun];

    public static int Tamsayi(this SqlDataReader dr, string sutun) => Convert.ToInt32(dr[sutun]);
}
