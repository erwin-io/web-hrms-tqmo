var globalConfiguration = function(){
	var globalConfig = {};
	globalConfig.getUser = function() {
      globalConfig.User = localStorage.getItem('user');
      return globalConfig.User;
    }
	globalConfig.setUser = function(user) {
       localStorage.setItem('user',user);
    }
    return  globalConfig;
}
var globalConfig = new globalConfiguration;