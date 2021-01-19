import { HttpDataService } from './http-data.service';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PostModel } from '../../models/sampleModel/PostModel';
import { HttpErrorResponse } from '@angular/common/http';

fdescribe('HttpDataService', () => {
  let httpTestCtrl: HttpTestingController;
  let dataService: HttpDataService;

  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [HttpDataService],
    })
  );

  beforeEach(() => {
    dataService = TestBed.inject(HttpDataService);
    httpTestCtrl = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestCtrl.verify();
  });

  it('should test HttpClient.get', () => {
    const testPost: PostModel[] = [
      { id: 1, userId: 1, title: 'test title 1', body: 'sample body 1' },
      { id: 2, userId: 2, title: 'test title 2', body: 'sample body 2' },
    ];
    dataService.getPostList().subscribe((posts) => {
      expect(testPost).toBe(posts, 'should check mocked data');
    });

    const req = httpTestCtrl.expectOne(dataService.base_url + 'posts');
    expect(req.cancelled).toBeFalsy();
    expect(req.request.responseType).toEqual('json');
  });

  it('should add post and return added post', () => {
    const newPost: PostModel = {
      id: 1,
      userId: 1,
      title: 'New title',
      body: 'new Post',
    };
    dataService.createPost(newPost).subscribe((addedposts) => {
      expect(addedposts).toBe(newPost);
    });

    const req = httpTestCtrl.expectOne(dataService.base_url + 'posts');
    expect(req.cancelled).toBeFalsy();
    expect(req.request.responseType).toEqual('json');
  });

  it('should test 404 error', () => {
    const errorMsg = ' 404 error occured';
    dataService.getPostList().subscribe(
      (data) => {
        fail('failed with 404 error');
      },
      (err: HttpErrorResponse) => {
        expect(err.status).toEqual(404);
        expect(err.error).toEqual(errorMsg);
      }
    );

    const req = httpTestCtrl.expectOne(dataService.base_url + 'posts');
    req.flush(errorMsg, { status: 404, statusText: 'Not found' });
  });
});
