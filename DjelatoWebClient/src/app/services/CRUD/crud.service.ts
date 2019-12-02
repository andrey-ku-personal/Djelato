import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { IResponseContent } from '../../shared/models/response-content';

@Injectable({
  providedIn: 'root'
})

export class CrudService {

  private url: string = environment.apiUrl; 

  constructor(
    private http: HttpClient
    ) { }

  post(path: string, body: any, options?: any): Observable<any> {
    return this.http.post(`${this.url}${path}`, body, options );
  }  
}
