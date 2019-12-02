import { Injectable } from '@angular/core';
import { CrudService } from '../CRUD/crud.service';
import { LoginModel } from 'src/app/models/login-model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  private authUrl: string = '/api/auth';

  constructor(
    private crudServices: CrudService
  ) { }

  login(body: LoginModel ): Observable<any> {
    return this.crudServices.post(this.authUrl, body)
  }

}
