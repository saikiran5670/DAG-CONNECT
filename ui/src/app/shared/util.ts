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
        let _date: any = moment.utc(_utc).tz(timeZone).format('YYYY/MM/DD hh:mm:ss');
        return (_date);
    }

    public static convertUtcToDateNoFormat(_utc: any, timeZone: any){
        let _date: any = moment.utc(_utc).tz(timeZone);
        return (_date._d);
    }

    public static convertUtcToDateFormat(_utc: any,_format){
        let _date: any = moment.utc(_utc).format(_format);
        return (_date);
    }

    
    public static getHhMmTime(totalSeconds: any){
    let data: any = "00:00";
    let hours = Math.floor(totalSeconds / 3600);
    totalSeconds %= 3600;
    let minutes = Math.floor(totalSeconds / 60);
    let seconds = totalSeconds % 60;
    data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}`;
    return data;
  }

}