import { Injectable } from '@angular/core';

import { CrudService } from '../CRUD/crud.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class UserService {

  private createUserUrl: string = '/api/user';
  private confirmEmailUrl: string = '/api/user/confirmEmail/';

  constructor(
    private crudServices: CrudService
  ) { }

  createUser(body: FormData): Observable<any> {
    return this.crudServices.post(this.createUserUrl, body);
  }

  sentConfirmKey(body: number): Observable<any> {
    return this.crudServices.post(`${this.confirmEmailUrl}${body}`, null);
  }
}
