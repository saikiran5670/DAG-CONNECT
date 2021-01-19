import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  HttpClient,  HttpHeaders} from '@angular/common/http';
import { PostModel } from '../../models/sampleModel/PostModel';

@Injectable()
export class HttpDataService {
  //This service & spec file is created for Unit testing purpose.
  //**************************************************** */
  base_url = 'https://jsonplaceholder.typicode.com/';
  constructor(private httpClient: HttpClient) {}

  public getPostList() {
    return this.httpClient.get<PostModel[]>(this.base_url + 'posts');
  }

  createPost(data): Observable<any> {
    const headers = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    };
    return this.httpClient.post<PostModel>(this.base_url + 'posts', data, headers);
  }
}
