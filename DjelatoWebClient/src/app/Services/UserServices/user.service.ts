import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { CrudService } from '../crud.service';
import { UserModel } from 'src/app/sign-up/models/user-model'

@Injectable({
  providedIn: 'root'
})

export class UserService {

  constructor(
    private crudServices: CrudService
  ) { }

  createUser(body: UserModel) {
    return this.crudServices.post('/api/user', body);
  }

  sentConfirmKey(body: string) {
    return this.crudServices.post(`/api/user/confirmEmail/${body}`, null);
  }
}
