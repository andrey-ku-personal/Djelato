import { Injectable } from '@angular/core';

import { CrudService } from '../crud.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class UserService {

  constructor(
    private crudServices: CrudService
  ) { }

  createUser(body: FormData): Observable<any> {
    return this.crudServices.post('/api/user', body);
  }

  sentConfirmKey(body: number): Observable<any> {
    return this.crudServices.post(`/api/user/confirmEmail/${body}`, null);
  }
}
