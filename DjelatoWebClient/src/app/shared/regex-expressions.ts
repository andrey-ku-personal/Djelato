export class RegexExpressions {
    static nameRgx: RegExp = /^[a-zA-Z0-9_ -]+$/;
    static emailRgx: RegExp = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    static passwordRgx: RegExp = /^(?=.*[A-Z])(?=.*\d)(.{8,100})$/
}
