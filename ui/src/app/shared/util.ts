import * as moment from 'moment-timezone';

const countriesData = require('moment-timezone/data/meta/latest.json');
const languageCodeData = require('../data.static/LanguageCodeAndCountryCodeMapping.json');

export class Util {
    public static convertDateToUtc(date: any){
        let _utc: any = moment.utc(date)
        return _utc._d.getTime();
    }

    // public static getUTCDate(prefTimezone: any){
    //     let _t = prefTimezone.split(')');
    //     let _timezone: any;
    //     if(_t.length > 0){
    //         _timezone = _t[1].trim();
    //     }
    //     let date: any = moment.utc().tz(_timezone ? _timezone : prefTimezone);
    //     //let date: any = moment.utc().tz(prefTimezone);
    //     return date._d;
    // }

    public static getUTCDate(prefTimezone: any){
        let _t = prefTimezone.split(') ');
        let _timezone: any;
        if(_t.length > 0){
            _timezone = _t[1].trim();
        }
        let date: any = moment().tz(_timezone ? _timezone : prefTimezone).format();
      
        let _date= date.split("T")[0];
        let _time= (date.split("T")[1]).slice(0, -6);
         date=new Date();
         date.setDate(_date.split("-")[2]);
         date.setMonth(_date.split("-")[1]-1);
         date.setYear(_date.split("-")[0]);
         date.setHours(_time.split(":")[0]);
         date.setMinutes(_time.split(":")[1]);
         date.setSeconds(_time.split(":")[2]);
      
         return date;
    }

    public static convertUtcToDate(_utc: any, timeZone: any){
        let _t = timeZone.split(')');
        let _timezone: any;
        if(_t.length > 0){
            _timezone = _t[1].trim();
        }
        let _date: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format('YYYY/MM/DD hh:mm:ss');
        //let _date: any = moment.utc(_utc).tz(timeZone).format('YYYY/MM/DD hh:mm:ss');
        return (_date);
    }

    public static convertUtcToDateNoFormat(_utc: any, timeZone: any){
        let _t = timeZone.split(')');
        let _timezone: any;
        if(_t.length > 0){
            _timezone = _t[1].trim();
        }
        let _date: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone);
        //let _date: any = moment.utc(_utc).tz(timeZone);
        return (_date._d);
    }

    public static convertUtcToDateFormat(_utc: any,_format){
        let _date: any = moment.utc(_utc).format(_format);
        return (_date);
    }

    public static utcToDateConversion(utc: any){
        let _date: any = moment.utc(utc);
        return (_date._d);
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

  public static getHhMmTimeFromMS(totalSeconds: any){
    let data: any = "00:00";
    let hours = Math.floor(totalSeconds / 3600000);
    totalSeconds %= 3600;
    let minutes = Math.floor(totalSeconds / 60);
    let seconds = totalSeconds % 60;
    data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}`;
    return data;
  }

}