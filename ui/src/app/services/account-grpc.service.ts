import { Injectable } from '@angular/core';
import { Observable, Subject, of, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ConfigService } from '@ngx-config/core';
import { delay, catchError } from 'rxjs/internal/operators';
import { AccountServiceClient, ServiceError } from '../protos/Account/account_pb_service';
import { AccountDataList, AccountFilter } from '../protos/Account/account_pb';

	@Injectable({
		providedIn: 'root'
	})
	export class AccountGrpcService {
		private backendGrpc: string;
		
		constructor(private httpClient: HttpClient, private config: ConfigService, private gRpcClient: AccountServiceClient) {
			this.backendGrpc = config.getSettings("foundationServices").accountGrpcServiceUrl;
			this.gRpcClient = new AccountServiceClient(this.backendGrpc);
		}

        getAllAccounts(){
            let req = new AccountFilter();
            req.setId(0);
            req.setOrganizationid(35);
            req.setName("");
            req.setEmail("");
            req.setAccountids("");

            return new Promise((resolve, reject) => {
				this.gRpcClient.get(req, (err: ServiceError, response: AccountDataList) => {
					if (err) {
						console.log(`Error while invoking gRpc: ${err}`);
						return reject(err);
					}
					else{
                        console.log(response);
						return resolve(response);
					}
				});
			});
		}
		
		
	}
