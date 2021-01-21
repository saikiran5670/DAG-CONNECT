import { Injectable } from '@angular/core';
import { Observable, Subject, of, throwError } from 'rxjs';
import { ConfigService } from '@ngx-config/core';
import { delay, catchError } from 'rxjs/internal/operators';
//import { GreeterClient, ServiceError } from '../protos/Greet/greet_pb_service';
//import { HelloReply, HelloRequest } from '../protos/Greet/greet_pb';
import { AccountServiceClient, ServiceError } from '../protos/Account/account_pb_service';
import { AccountDataList, AccountFilter } from '../protos/Account/account_pb';


	@Injectable({
		providedIn: 'root'
	})
	export class AccountGrpcService {
		private backendGrpc: string;
		
		constructor(private config: ConfigService, 
					private gRpcClient: AccountServiceClient,
					//private gRpcClient: GreeterClient
		) {
			this.backendGrpc = config.getSettings("foundationServices").accountGrpcServiceUrl;
			this.gRpcClient = new AccountServiceClient(this.backendGrpc);
			//this.gRpcClient = new GreeterClient(this.backendGrpc);
		}

		// getGreet(){
		// 	let req = new HelloRequest();
		// 	req.setName('Vishal');
		// 	return new Promise((resolve, reject) => {
		// 		this.gRpcClient.sayHello(req, (err: ServiceError, response: HelloReply) => {
		// 			if (err) {
		// 				console.log(`Error while invoking gRpc: ${err}`);
		// 				return reject(err);
		// 			}
		// 			else{
		// 				console.log("response:: ", response);
		// 				return resolve(response);
		// 			}
		// 		});
		// 	});
		// }

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
