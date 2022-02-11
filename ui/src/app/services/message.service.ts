import { nullSafeIsEquivalent } from '@angular/compiler/src/output/output_ast';
import { Injectable } from '@angular/core';
import { Observable, Subject, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MessageService {
    private subject = new Subject<any>();
    private timerSubject = new Subject<any>();
    brandLogoSubject = new BehaviorSubject(null);

    sendMessage(message: any) {
        this.subject.next({ key: message });
    }

    clearMessages() {
        this.subject.next();
    }

    getMessage(): Observable<any> {
        return this.subject.asObservable();
    }

    sendTimerValue(timer: any) {
        this.timerSubject.next({ value: timer });
    }

    notifyTimerUpdate(): Observable<any>{
        return this.timerSubject.asObservable();
    }

    setBrandLogo(value) {
        this.brandLogoSubject.next(value);
    }
}