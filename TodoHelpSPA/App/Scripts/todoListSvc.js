'use strict';
angular.module('todoApp')
.factory('todoListSvc', ['$http', 'config', function ($http, config) {
    return {
        getItems : function(){
            return $http.get(config.apiURL + '/api/TodoList');
        },
        getItem : function(id){
            return $http.get(config.apiURL + '/api/TodoList/' + id);
        },
        postItem : function(item){
            return $http.post(config.apiURL + '/api/TodoList/', item);
        },
        putItem : function(item){
            return $http.put(config.apiURL + '/api/TodoList/', item);
        },
        deleteItem : function(id){
            return $http({
                method: 'DELETE',
                url: config.apiURL + '/api/TodoList/' + id
            });
        }
    };
}]);