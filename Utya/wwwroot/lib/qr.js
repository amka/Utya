window.addEventListener("load", () => {
    const uri = document.getElementById("qrCodeData");
    if (!uri) return;
    new QRCode(document.getElementById("qrCode"),
        {
            text: uri.getAttribute('data-url'),
            width: 164,
            height: 164,
        });
});
