import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { UserService } from '../Services/UserServices/user.service';
import { IResponseContent } from '../shared/models/response-content';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  message: string;

  constructor(
    private route: ActivatedRoute,
    private userServices: UserService
  ) { }

  ngOnInit(
  ) {
    this.route.paramMap.pipe(
      switchMap(params => {
        const key: string = params.get('key');
        console.log(key);
        return this.userServices.sentConfirmKey(key);
      })).subscribe(
        (data) => {
          console.log(data);          
        }
        // , 
        // error => {
        //   let errorContent: IResponseContent = error.error;  
        // }
      );


  }

}
