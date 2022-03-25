mixinArray.push({
    data: function () {
        return {
            //These are reactive.  Changing them will change whether 
            //the related button is displayed
            page: 1,
            pageSize: 10,
            eventItems: [],
            noMoreData: false
        };
    },
    methods: {


    }
});