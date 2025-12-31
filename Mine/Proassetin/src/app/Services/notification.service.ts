import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NotificationsService } from 'angular2-notifications';

@Injectable({
    providedIn: 'root'
})
export class NotificationService {

    constructor(private http: HttpClient, private _notification: NotificationsService) { }


    NotificationSuccess(msg:any) {
        
        let notcnfig = {
            timeOut: 5000,
            showProgressBar: true,
            pauseOnHover: true,
            clickToClose: true,
            animate: 'fromRight'
        }

        this._notification.success(msg, '', notcnfig)

    }

    NotificationWarning(msg:any) {
        let notcnfig = {
            timeOut: 5000,
            showProgressBar: true,
            pauseOnHover: true,
            clickToClose: true,
            animate: 'fromRight',
            toastClass: 'custom-toast-warning'
        }

        this._notification.warn(msg, '', notcnfig)

    }
    
    NotificationFailure(msg:any) {
        let notcnfig = {
            timeOut: 5000,
            showProgressBar: true,
            pauseOnHover: true,
            clickToClose: true,
            animate: 'fromRight'
        }

        this._notification.error(msg, '', notcnfig)

    }

    NotificationAlert(msg:any) {
        let notcnfig = {
            timeOut: 5000,
            showProgressBar: true,
            pauseOnHover: true,
            clickToClose: true,
            animate: 'fromRight'
        }

        this._notification.info(msg, '', notcnfig)

    }

    
   
}
