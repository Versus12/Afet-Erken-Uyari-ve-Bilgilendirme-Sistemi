using System;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// İş kuralı / doğrulama ihlallerinde fırlatılır. Mesajı doğrudan kullanıcıya gösterilebilir.
    /// (Teknik hatalar - örn. SqlException - bu sınıfla karıştırılmaz.)
    /// </summary>
    public class IsKuraliHatasi : Exception
    {
        public IsKuraliHatasi(string mesaj) : base(mesaj) { }
    }

    /// <summary>Oturum yok, süresi dolmuş ya da rol yetersizse fırlatılır.</summary>
    public class YetkisizErisimHatasi : Exception
    {
        public YetkisizErisimHatasi(string mesaj) : base(mesaj) { }
    }
}
