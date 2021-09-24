import * as moment from 'moment-timezone';

const countriesData = require('moment-timezone/data/meta/latest.json');
const languageCodeData = require('../data.static/LanguageCodeAndCountryCodeMapping.json');

export class Util {
    public static convertDateToUtc(date: any){
        let _utc: any = moment.utc(date);
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
        if(date.includes('Z'))
        {
            date= date.replace('Z','+00:00');
        }      
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
        let _date: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format('YYYY/MM/DD HH:mm:ss');
        //let _date: any = moment.utc(_utc).tz(timeZone).format('YYYY/MM/DD hh:mm:ss');
        return (_date);
    }


    public static convertUtcToDateTZ(_utc: any, timeZone: any){ // with current TZ
        let _t = timeZone.split(')');
        let _timezone: any;
        if(_t.length > 0){
            _timezone = _t[1].trim();
        }
        let date: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format();
        let _time = moment(date).valueOf();
        let _addTz = (moment(date)['_tzm'])* 60000;
        let cTime = _time + _addTz;
        return cTime;
    }

    public static convertToDateTZ(cTime: any, dateFormat: any){ // with current TZ get date time
        let dFormat = 'dd/mm/yyyy';
        switch(dateFormat){
            case 'ddateformat_dd/mm/yyyy': {
                dFormat = 'DD/MM/YYYY';
              break;
            }
            case 'ddateformat_mm/dd/yyyy': {
                dFormat = 'MM/DD/YYYY';
              break;
            }
            case 'ddateformat_dd-mm-yyyy': {
                dFormat = 'DD-MM-YYYY';
      
              break;
            }
            case 'ddateformat_mm-dd-yyyy': {
                dFormat = 'MM-DD-YYYY';
      
              break;
            }
            default:{
                dFormat = 'DD/MM/YYYY';
      
            }
          }
        let _format =  dFormat ? dFormat + ' HH:mm:ss' : 'DD/MM/YYYY HH:mm:ss';
        let cDate = moment.utc(cTime).format(_format);
        return cDate;
    }
    
    public static convertUtcToDateNoFormat(_utc: any, timeZone: any){
        let _t = timeZone.split(')');
        let _timezone: any;
        if(_t.length > 0){
            _timezone = _t[1].trim();
        }
        let date: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format();
        //let _date: any = moment.utc(_utc).tz(timeZone);
        // return (_date._d);

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

    public static convertUtcToTimeStringFormat(_utc: any, timeZone: any){
        let _t = timeZone.split(')');
        let _timezone: any;
        if(_t.length > 0){
            _timezone = _t[1].trim();
        }
        let date: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format();
        //let _date: any = moment.utc(_utc).tz(timeZone);
        // return (_date._d);

        let _date= date.split("T")[0];
        let _time= (date.split("T")[1]).slice(0, -6);
         date=new Date();
         date.setDate(_date.split("-")[2]);
         date.setMonth(_date.split("-")[1]-1);
         date.setYear(_date.split("-")[0]);
         date.setHours(_time.split(":")[0]);
         date.setMinutes(_time.split(":")[1]);
         date.setSeconds(_time.split(":")[2]);
      
         let time =  _time.split(":")[0] + _time.split(":")[1] + _time.split(":")[2];
         return _time;
    }

    public static convertUtcToDateTimeStringFormat(_utc: any, timeZone: any, timeFormat? :any){
        let _t = timeZone.split(')');
        let _timezone: any;
        if(_t.length > 0){
            _timezone = _t[1].trim();
        }
        let _format =  timeFormat ? timeFormat + ' HH:mm:ss' : 'DD/MM/YYYY HH:mm:ss';
        let _date: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format(_format);
        //let _date: any = moment.utc(_utc).tz(timeZone).format('YYYY/MM/DD hh:mm:ss');
        return (_date);
    }


    public static convertUtcToDateFormat(_utc: any,_format, timeZone? : any){
        let _date: any;
        if (timeZone){
            let _t = timeZone.split(')');
            let _timezone: any;
            if(_t.length > 0){
                _timezone = _t[1].trim();
            }
            _date = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format(_format);
        }
        else{
        _date = moment.utc(_utc).format(_format);

        }
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

  public static getHhMmTimeFromMS(totalMilliSeconds: any){
    if(totalMilliSeconds < 0){
        return '00:00';
    }
    else{
        let seconds = totalMilliSeconds/ 1000;
        let time = this.getHhMmTime(seconds);
        return time;
    }
  }

  public static getHhMmSsTimeFromMS(totalMilliSeconds: any){
    let totalSeconds = totalMilliSeconds/ 1000;
    let data: any = "00:00";
    let hours = Math.floor(totalSeconds / 3600);
    totalSeconds %= 3600;
    let minutes = Math.floor(totalSeconds / 60);
    let seconds = totalSeconds % 60;
    data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}:${(seconds >= 10) ? seconds : ('0'+seconds)}`;
    return data;
  }
   
    public static getMillisecondsToUTCDate(_date: any, prefTimezone: any) {
        let _dateWithoutMiliSeconds: any = new Date(_date.setMilliseconds(0));
        let _t = prefTimezone.split(') ');
        let _timezone: any;
        if (_t.length > 0) {
            _timezone = _t[1].trim();
        }
        // if(moment().tz(_timezone).utcOffset() == moment().tz(moment.tz.guess()).utcOffset()) {​​​​​​
        // console.log(moment.utc( _date ).valueOf());
        // return _dateWithoutMiliSeconds.getTime(); 
        // }​​​​​​ 
        // else{
        console.log(_dateWithoutMiliSeconds.getTime() )
        let localTimeZoneOffset = moment().tz(moment.tz.guess()).utcOffset();
       // let gmt = moment(_dateWithoutMiliSeconds).utcOffset(localTimeZoneOffset);
        
        let PrefTzToGMT: any = moment().tz(_timezone).utcOffset() * -1;
        let diff = localTimeZoneOffset + PrefTzToGMT;
        let PrefTimeAsPerSelected = moment(_dateWithoutMiliSeconds).utcOffset(diff);       
        let _convertedUtc = PrefTimeAsPerSelected['_d'].getTime();
        console.log('_convertedUtc:' +_convertedUtc );
        return _convertedUtc;
        //}
        // let gmt_val:any =moment.utc(_dateWithoutMiliSeconds).valueOf();     
        // return gmt_val;
    }
        
        // let PrefTzToGMT: any = moment().tz(_timezone).utcOffset() * -1;
        // let PrefTimeAsPerSelected = moment(_dateWithoutMiliSeconds).utcOffset(PrefTzToGMT);
        // let _convertedUtc = PrefTimeAsPerSelected['_d'].getTime();
        // return _convertedUtc;
  
        // if(moment().tz(_timezone).utcOffset() == moment().tz(moment.tz.guess()).utcOffset()) {​​​​​​
        //     console.log(moment.utc( _date ).valueOf());
        //    return _dateWithoutMiliSeconds.getTime(); 
        //    }​​​​​​ 
       // let localTimeZoneOffset = moment().tz(moment.tz.guess()).utcOffset() * -1;
            // let gmt = moment(_dateWithoutMiliSeconds).utcOffset(localTimeZoneOffset);
 
            // let PrefTzToGMT: any = moment().tz(_timezone).utcOffset();
            // let PrefTimeAsPerSelected = moment(gmt['_d']).utcOffset(PrefTzToGMT);
 
            // let _convertedUtc = PrefTimeAsPerSelected['_d'].getTime();
            // return gmt['_d'].getTime();
    public static getMillisecondsToUTCDate1(_date: any, prefTimezone: any) {
        
        // //    console.log("_date", _date)
         let _dateWithoutMiliSeconds:any = new Date(_date.setMilliseconds(0));
        // //    console.log("_date without miliseconds", _dateWithoutMiliSeconds)
        // //    console.log("_date", moment(_date).millisecond(0))
        let _t = prefTimezone.split(') ');
        let _timezone: any;
        if (_t.length > 0) {
            _timezone = _t[1].trim();
        }
        let gmtTimeDiff = _dateWithoutMiliSeconds.getTimezoneOffset();// +5.30 diff
         console.log("gmtTimeDiff", gmtTimeDiff)  
        //let _gmt = moment(_dateWithoutMiliSeconds).utcOffset(gmtTimeDiff); // gmt time of selected from locale
        // console.log(" gmt time of selected from locale", _gmt)
        //let localeToGmtTz:any = _date.getTimezoneOffset();
        let PrefTzToGMT: any = moment().tz(_timezone).utcOffset() * -1;// diff selected timezone pref 
        
        console.log("diff selected timezone pref ", PrefTzToGMT);

        let PrefTimeAsPerSelected =  moment(_dateWithoutMiliSeconds).utcOffset(PrefTzToGMT);

        console.log("PrefTimeAsPerSelected ", PrefTimeAsPerSelected);

       // let UtcValToSendToAPI = moment(_gmt).utcOffset(PrefTzToGMT);
        let _convertedUtc = PrefTimeAsPerSelected['_d'].getTime();
        console.log('_convertedUtc==' + _convertedUtc);
        return _convertedUtc;

   }

  public static convertUtcToDateAndTimeFormat(_utc: any, timeZone: any, timeFormat? :any){
    let _t = timeZone.split(')');
    let _timezone: any;
    if(_t.length > 0){
        _timezone = _t[1].trim();
    }
    let _format =  timeFormat ? timeFormat : 'DD/MM/YYYY';
    let _tFormat =  'HH:mm';
    let _date: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format(_format);
    let _time: any = moment.utc(_utc).tz(_timezone ? _timezone : timeZone).format(_tFormat);

    return ([_date,_time]);
}

//   public static utcToDateConversionTimeZone(_utc: any, prefTimezone: any){
//     let _t = prefTimezone.split(') ');
//     let _timezone: any;
//     if(_t.length > 0){​​​​​​​
//         _timezone = _t[1].trim();
//     }​​​​​​​
//     let _date: any = moment.utc(_utc).tz(_timezone ? _timezone : prefTimezone).format("DD-MM-YYYY h:mm:ss A");
//     return _date;
// }
}


