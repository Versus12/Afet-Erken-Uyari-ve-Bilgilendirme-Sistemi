using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AfetErkenUyari.Web.Models;

/// <summary>
/// Enlem/boylam gibi ondalık alanlarda hem "38,88" hem "38.88" yazımını kabul eder.
/// (Türkçe kültürde varsayılan bağlayıcı noktayı binlik ayıracı sanıp yanlış okur.)
/// </summary>
public class OndalikModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext ctx)
    {
        var deger = ctx.ValueProvider.GetValue(ctx.ModelName);
        if (deger == ValueProviderResult.None) return Task.CompletedTask;
        ctx.ModelState.SetModelValue(ctx.ModelName, deger);

        string metin = (deger.FirstValue ?? "").Trim();
        if (metin.Length == 0)
        {
            // Boş alan: nullable ise null, değilse hata
            if (Nullable.GetUnderlyingType(ctx.ModelType) != null) ctx.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        if (decimal.TryParse(metin.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal sonuc))
            ctx.Result = ModelBindingResult.Success(Math.Round(sonuc, 6));
        else
            ctx.ModelState.TryAddModelError(ctx.ModelName, $"'{metin}' geçerli bir sayı değil.");
        return Task.CompletedTask;
    }
}

public class OndalikModelBinderSaglayici : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext ctx) =>
        ctx.Metadata.ModelType == typeof(decimal) || ctx.Metadata.ModelType == typeof(decimal?)
            ? new OndalikModelBinder()
            : null;
}
