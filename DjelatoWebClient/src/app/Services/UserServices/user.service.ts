import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { CrudService } from '../crud.service';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private crudServices: CrudService
  ) { }

  header: HttpHeaders = new HttpHeaders()
  .set('Content-type', 'application/json');

  createUser(body: string){

    this.crudServices.post('/api/user', body, { headers: this.header }).subscribe((data) => {
      console.log("success")
    });
  }
}
