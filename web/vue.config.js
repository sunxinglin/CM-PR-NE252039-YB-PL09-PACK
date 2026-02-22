const path = require("path");
const webpack = require("webpack");
function resolve(dir) {
  return path.join(__dirname, "/", dir);
}
module.exports = {
  productionSourceMap: false,
  lintOnSave: process.env.NODE_ENV !== "production",
  devServer: {
    port: 1803,
    proxy: {
      "/api": {
        target: "http://127.0.0.1:8128/",
        changeOrigin: true,
      },
      "/productionHub": {
        target: "http://127.0.0.1:8128/",
        changeOrigin: true,
        ws: true,
      },
    },
    overlay: {
      warnings: true,
      errors: false,
    },
  },
  // svg配置
  chainWebpack(config) {
    config.plugin("provide").use(webpack.ProvidePlugin, [
      {
        $: "jquery",
        jquery: "jquery",
        jQuery: "jquery",
        "window.jQuery": "jquery",
      },
    ]);
    config.module.rule("svg").exclude.add(resolve("src/icons")).end();
    config.module
      .rule("icons")
      .test(/\.svg$/)
      .include.add(resolve("src/icons"))
      .end()
      .use("svg-sprite-loader")
      .loader("svg-sprite-loader")
      .options({
        symbolId: "icon-[name]",
      })
      .end();
  },
};
