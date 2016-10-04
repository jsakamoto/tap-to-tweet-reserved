app.filter('htmlLineBreak', ['$injector', ($injector) => {
    var domElem: JQuery = null;
    var sce = null;
    return (input: string) => {
        if (input == null) return null;
        sce = sce || $injector.get('$sce');
        domElem = domElem || $(window.document.createElement('div'));
        return sce.trustAsHtml(
            domElem.text(input).html()
                .replace(/(\n)|(\r\n)|(\r)/ig, '<br/>')
                .replace(/ /ig, '&nbsp;'));
    };
}]);
