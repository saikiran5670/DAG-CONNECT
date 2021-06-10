import * as moment from 'moment-timezone';

const countriesData = require('moment-timezone/data/meta/latest.json');
const languageCodeData = require('../data.static/LanguageCodeAndCountryCodeMapping.json');

export class Util {
    public static convertDateToUtc(date: any){
        let _utc: any = moment.utc(date)
        return _utc._d.getTime();
    }

    public static getUTCDate(){
        let date: any = moment.utc();
        return date._d;
    }

    public static convertUtcToDate(_utc: any){
        let _date: any = moment.utc(_utc);
        return _date._d;
    }
}