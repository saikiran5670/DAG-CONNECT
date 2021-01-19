import { Injectable } from '@angular/core';
	import { Observable, Subject, of, throwError } from 'rxjs';
	import { HttpClient, HttpErrorResponse } from '@angular/common/http';
	import { ConfigService } from '@ngx-config/core';
	import { delay, catchError } from 'rxjs/internal/operators';
	import { AuthServiceClient, ServiceError } from '../protos/Identity/Identity_pb_service';
	import { TokenResponse, UserRequest } from '../protos/Identity/Identity_pb';
	import { Empty } from 'google-protobuf/google/protobuf/empty_pb';

	@Injectable({
		providedIn: 'root'
	})
	export class IdentityGrpcService {
		private backendGrpc: string;
		gRpcClient: AuthServiceClient;
		req: any;

		constructor(private httpClient: HttpClient, private config: ConfigService) {
			this.backendGrpc = config.getSettings("foundationServices").backendGrpcServiceUrl;
			this.gRpcClient = new AuthServiceClient(this.backendGrpc);
			this.req = new UserRequest();
		}

		getGenerateToken(username: any, password: any): Promise<any> {
			this.req.setUsername(username);
			this.req.setPassword(password);
			return new Promise((resolve, reject) => {
				this.gRpcClient.generateToken(this.req, (err: ServiceError, response: TokenResponse) => {
					if (err) {
						console.log(`Error while invoking gRpc: ${err}`);
						return reject(err);
					}
					else{
						// let responseA = response.getAccesstoken();
						// let responseB = response.getTokentype();
						// let responseC = response.getRefreshtoken();
						return resolve(response.getRefreshtoken());
					}
				});
			});
		}
	}
