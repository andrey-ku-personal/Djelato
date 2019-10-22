import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { UserService } from '../Services/UserServices/user.service';
import { IResponseContent } from '../shared/models/response-content';
import { MatDialog } from '@angular/material';
import { EmailConfirmedComponent } from '../email-confirmed/email-confirmed.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent implements OnInit {

constructor(
    private route: ActivatedRoute,
    private userServices: UserService,
    private dialog: MatDialog
  ) { }

ngOnInit(
  ) {
    const data = this.route.paramMap.pipe(
      switchMap(params => {
        const key: string = params.get('key');
        return this.userServices.sentConfirmKey(key);
      }));

      data.subscribe((data: IResponseContent) => {
        if (data.isSucceeded){
          this.openDialog(data.message);
        }
      }, error =>{
        let errorContent: IResponseContent = error.error;
        this.openDialog(errorContent.errorMessage);
      });
  }

  openDialog(message:string){
    this.dialog.open(EmailConfirmedComponent, {
      autoFocus: false,
      data: { message: message }
    });
  }
}