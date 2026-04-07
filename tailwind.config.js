const lumexui = require("lumexui/theme/plugin");

module.exports = {
    content: [
        "./**/*.razor",
        "./**/*.cshtml",
        "./**/*.html",
    ],
    theme: { extend: {} },
    plugins: [lumexui],
};