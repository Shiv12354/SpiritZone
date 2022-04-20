
var requireJsConfig = {
  host: "ec2amaz-j836kgo.qonverse.ai",
    prefix: '/',
    port: '',//443,
    isSecure: true
};

  this.require.config({
    paths: {
      'qlik': (requireJsConfig.isSecure ? "https://" : "http://") + requireJsConfig.host + (requireJsConfig.port ? ":" + requireJsConfig.port : "") + requireJsConfig.prefix + "resources/js/qlik"
    },
    baseUrl: (this.requireJsConfig.isSecure ? 'https://' : 'http://') + requireJsConfig.host + requireJsConfig.prefix + 'resources'
  });
  
  var loadQlik;
  
  this.require(['js/qlik'], (qlik) => {
    console.log(qlik);
    loadQlik = qlik;
    qlikLoad = qlik;
  });
