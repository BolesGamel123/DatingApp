import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  
  constructor(private accountService:AccountService , private toastr:ToastrService) {
  }

  canActivate(): Observable<boolean> {

   return this.accountService.currentUser$.pipe(

   map((user)=>{

    if (!user) return false;
    if (user.roles.includes('Admin')|| user.roles.includes('Moderator')) {
      return true;
    }
    
    else{
      this.toastr.error('You cannot enter this area');
        return false
    }
   }
   

   
   )
   )
  }
  
}
