var dataFormatter = function() {

	var getMultiSelectData = (format, data)=>{
        var opt = [];
        for(var i in data){
            if(data[i].id != undefined){
                var obj = {};
                obj[format.keyId] = data[i].id;
                obj[format.keyName] = data[i].name;
                opt.push(obj);
            }
        }
        return opt;
    }

    var formatMultiSelectData = (format1, format2, data)=>{
        var opt = [];
        for(var i in data){
            if(data[i].id != undefined){
                var obj = {};
                obj[format2.keyId] = data[i].format1.keyId;
                obj[format2.keyName] = data[i].format1.keyId;
                opt.push(obj);
            }
        }
        return opt;
    }
	return {
        getMultiSelectData : getMultiSelectData,
		formatMultiSelectData : formatMultiSelectData,
	};
};
var dataFormat = new dataFormatter;
