// Global helper to handle flag image loading errors gracefully with SVG placeholders
function handleFlagError(img, size) {
    img.onerror = null;
    var width = size === 'sm' ? 36 : 140;
    var height = size === 'sm' ? 24 : 90;
    var fontSize = size === 'sm' ? 8 : 16;
    img.src = "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='" + width + "' height='" + height + "' viewBox='0 0 " + width + " " + height + "'><rect width='100%' height='100%' fill='%23edeae4'/><text x='50%' y='55%' font-family='sans-serif' font-size='" + fontSize + "' fill='%238d8a80' text-anchor='middle' dominant-baseline='middle'>N/D</text></svg>";
}
