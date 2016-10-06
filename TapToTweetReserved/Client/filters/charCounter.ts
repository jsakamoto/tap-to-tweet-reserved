function charCounter(input: string, max: number): number {
    input = input || '';
    var regxp = /((https?:\/\/)?([a-z0-9.\-_]+\.[a-z]{2,3})([a-z0-9.%\-_\+/~])*(\?[a-z0-9=&%\-\+_!/~]*)?(\#[a-z0-9=&%\-\+_!/~]*)?)([^a-z.]|$)/ig;
    input = input.replace(regxp, function () {
        return new Array(22 + 1).join('-') + arguments[7];
    });
    return max - input.length;
}

app.filter('charCounter', () => charCounter);
