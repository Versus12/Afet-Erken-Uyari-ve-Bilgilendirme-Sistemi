// Silme / yayından kaldırma gibi geri alınamaz işlemlerden önce onay ister.
// Kullanım: <form data-onay="Emin misiniz?"> ... </form>
document.addEventListener("submit", function (e) {
    const form = e.target;
    const mesaj = form.getAttribute("data-onay");
    if (mesaj && !window.confirm(mesaj)) {
        e.preventDefault();
    }
});
