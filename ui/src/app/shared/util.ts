import * as moment from 'moment-timezone';

const countriesData = require('moment-timezone/data/meta/latest.json');
const languageCodeData = require('../data.static/LanguageCodeAndCountryCodeMapping.json');

export class Util {
    public static convertDateToUtc(date: any){
        let _utc: any = moment.utc(date)
        return _utc._d.getTime();
    }

    public static getUTCDate(prefTimezone){
        let date: any = moment.utc().tz(prefTimezone);
        return date._d;
    }

    public static convertUtcToDate(_utc: any, timeZone: any){
        let _date: any = moment.utc(_utc).tz(timeZone).format('YYYY/MM/DD HH:MM:SS');
        return (_date);
    }

    public static convertUtcToDateFormat(_utc: any,_format){
        let _date: any = moment.utc(_utc).format(_format);
        return (_date);
    }
}