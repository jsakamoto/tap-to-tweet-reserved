app.filter('htmlLineBreak', ['$injector', function ($injector) {
        var domElem = null;
        var sce = null;
        return function (input) {
            if (input == null)
                return null;
            sce = sce || $injector.get('$sce');
            domElem = domElem || $(window.document.createElement('div'));
            return sce.trustAsHtml(domElem.text(input).html()
                .replace(/(\n)|(\r\n)|(\r)/ig, '<br/>')
                .replace(/ /ig, '&nbsp;'));
        };
    }]);
//# sourceMappingURL=htmlLineBreak.js.map